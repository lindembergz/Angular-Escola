const fs = require('fs');
const path = require('path');

/**
 * Bundle Report Generator
 * Generates detailed bundle analysis report from webpack stats
 */

const STATS_FILE = 'dist/sistema-gestao-escolar-frontend/stats.json';
const REPORT_FILE = 'bundle-report.json';

function formatBytes(bytes) {
  if (bytes === 0) return '0 Bytes';
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

function convertStatsFormat(rawStats) {
  // Convert Angular CLI esbuild stats to webpack-like format
  const assets = [];
  const chunks = [];
  
  if (rawStats.outputs) {
    // Process outputs to create assets and chunks
    Object.entries(rawStats.outputs).forEach(([filename, output]) => {
      const size = output.bytes || 0;
      
      assets.push({
        name: filename,
        size: size
      });
      
      // Determine if it's an initial chunk based on filename patterns
      const isInitial = !filename.includes('chunk-') || 
                       filename.includes('main-') || 
                       filename.includes('polyfills-') ||
                       filename.includes('styles-');
      
      chunks.push({
        initial: isInitial,
        files: [filename]
      });
    });
  }
  
  return {
    assets: assets,
    chunks: chunks
  };
}

function analyzeBundleStats() {
  try {
    if (!fs.existsSync(STATS_FILE)) {
      console.error('❌ Stats file not found. Run "npm run build:stats" first.');
      process.exit(1);
    }

    const rawStats = JSON.parse(fs.readFileSync(STATS_FILE, 'utf8'));
    const stats = convertStatsFormat(rawStats);
    
    // Extract bundle information
    const assets = stats.assets || [];
    const chunks = stats.chunks || [];
    
    // Calculate total sizes
    const totalSize = assets.reduce((sum, asset) => sum + asset.size, 0);
    const jsAssets = assets.filter(asset => asset.name.endsWith('.js'));
    const cssAssets = assets.filter(asset => asset.name.endsWith('.css'));
    
    const jsSize = jsAssets.reduce((sum, asset) => sum + asset.size, 0);
    const cssSize = cssAssets.reduce((sum, asset) => sum + asset.size, 0);
    
    // Find initial bundle
    const initialChunks = chunks.filter(chunk => chunk.initial);
    const initialSize = initialChunks.reduce((sum, chunk) => {
      return sum + chunk.files.reduce((fileSum, fileName) => {
        const asset = assets.find(a => a.name === fileName);
        return fileSum + (asset ? asset.size : 0);
      }, 0);
    }, 0);
    
    // Identify largest assets
    const largestAssets = assets
      .sort((a, b) => b.size - a.size)
      .slice(0, 10)
      .map(asset => ({
        name: asset.name,
        size: asset.size,
        sizeFormatted: formatBytes(asset.size)
      }));
    
    // Generate report
    const report = {
      timestamp: new Date().toISOString(),
      summary: {
        totalSize: totalSize,
        totalSizeFormatted: formatBytes(totalSize),
        initialBundleSize: initialSize,
        initialBundleSizeFormatted: formatBytes(initialSize),
        jsSize: jsSize,
        jsSizeFormatted: formatBytes(jsSize),
        cssSize: cssSize,
        cssSizeFormatted: formatBytes(cssSize),
        budgetStatus: {
          initialBudget: 1024 * 1024, // 1MB
          isOverBudget: initialSize > 1024 * 1024,
          overageBytes: Math.max(0, initialSize - 1024 * 1024),
          overageFormatted: formatBytes(Math.max(0, initialSize - 1024 * 1024))
        }
      },
      assets: {
        total: assets.length,
        largest: largestAssets
      },
      chunks: {
        total: chunks.length,
        initial: initialChunks.length,
        lazy: chunks.filter(chunk => !chunk.initial).length
      },
      recommendations: generateRecommendations(initialSize, largestAssets)
    };
    
    // Save report
    fs.writeFileSync(REPORT_FILE, JSON.stringify(report, null, 2));
    
    // Display summary
    console.log('\n📊 Bundle Analysis Report');
    console.log('========================');
    console.log(`📦 Total Bundle Size: ${report.summary.totalSizeFormatted}`);
    console.log(`🚀 Initial Bundle Size: ${report.summary.initialBundleSizeFormatted}`);
    console.log(`📄 JavaScript: ${report.summary.jsSizeFormatted}`);
    console.log(`🎨 CSS: ${report.summary.cssSizeFormatted}`);
    console.log(`📁 Total Assets: ${report.assets.total}`);
    console.log(`🧩 Total Chunks: ${report.chunks.total} (${report.chunks.initial} initial, ${report.chunks.lazy} lazy)`);
    
    if (report.summary.budgetStatus.isOverBudget) {
      console.log(`\n⚠️  BUDGET EXCEEDED`);
      console.log(`   Target: 1 MB`);
      console.log(`   Current: ${report.summary.initialBundleSizeFormatted}`);
      console.log(`   Overage: ${report.summary.budgetStatus.overageFormatted}`);
    } else {
      console.log(`\n✅ Within Budget (1 MB)`);
    }
    
    console.log('\n🔍 Largest Assets:');
    report.assets.largest.forEach((asset, index) => {
      console.log(`   ${index + 1}. ${asset.name} - ${asset.sizeFormatted}`);
    });
    
    if (report.recommendations.length > 0) {
      console.log('\n💡 Recommendations:');
      report.recommendations.forEach((rec, index) => {
        console.log(`   ${index + 1}. ${rec}`);
      });
    }
    
    console.log(`\n📋 Full report saved to: ${REPORT_FILE}`);
    
    return report;
    
  } catch (error) {
    console.error('❌ Error analyzing bundle:', error.message);
    process.exit(1);
  }
}

function generateRecommendations(initialSize, largestAssets) {
  const recommendations = [];
  
  if (initialSize > 1024 * 1024) {
    recommendations.push('Initial bundle exceeds 1MB - implement more aggressive lazy loading');
  }
  
  // Check for large vendor libraries
  const primeNGAssets = largestAssets.filter(asset => 
    asset.name.includes('primeng') || asset.size > 400 * 1024
  );
  
  if (primeNGAssets.length > 0) {
    recommendations.push('Large UI library detected - optimize PrimeNG imports');
  }
  
  // Check for potential code splitting opportunities
  const largeChunks = largestAssets.filter(asset => asset.size > 200 * 1024);
  if (largeChunks.length > 2) {
    recommendations.push('Multiple large chunks detected - consider additional code splitting');
  }
  
  return recommendations;
}

// Run analysis
if (require.main === module) {
  analyzeBundleStats();
}

module.exports = { analyzeBundleStats, formatBytes };