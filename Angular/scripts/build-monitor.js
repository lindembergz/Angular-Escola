#!/usr/bin/env node

/**
 * Build Monitor Script
 * Enhanced build process with bundle size monitoring and warnings
 */

const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');
const { analyzeBundleStats, formatBytes } = require('./bundle-report');

const BUDGET_LIMITS = {
  initial: 1024 * 1024, // 1MB
  warning: 950 * 1024,  // 950KB
  total: 2 * 1024 * 1024, // 2MB
  chunk: 500 * 1024,    // 500KB
  asset: 400 * 1024     // 400KB
};

const COLORS = {
  red: '\x1b[31m',
  yellow: '\x1b[33m',
  green: '\x1b[32m',
  blue: '\x1b[34m',
  reset: '\x1b[0m',
  bold: '\x1b[1m'
};

function log(message, color = COLORS.reset) {
  console.log(`${color}${message}${COLORS.reset}`);
}

function logHeader(message) {
  log(`\n${COLORS.bold}${COLORS.blue}${message}${COLORS.reset}`);
  log('='.repeat(message.length), COLORS.blue);
}

function logSuccess(message) {
  log(`✅ ${message}`, COLORS.green);
}

function logWarning(message) {
  log(`⚠️  ${message}`, COLORS.yellow);
}

function logError(message) {
  log(`❌ ${message}`, COLORS.red);
}

function runBuild() {
  logHeader('Building Application with Bundle Monitoring');
  
  try {
    // Run the build with stats
    log('📦 Building application...');
    execSync('npm run build:stats', { stdio: 'inherit' });
    
    // Analyze the bundle
    log('\n📊 Analyzing bundle...');
    const report = analyzeBundleStats();
    
    // Check budgets and provide warnings
    checkBudgets(report);
    
    // Generate recommendations
    generateRecommendations(report);
    
    // Save build metrics
    saveBuildMetrics(report);
    
    logSuccess('Build completed successfully!');
    
  } catch (error) {
    logError(`Build failed: ${error.message}`);
    process.exit(1);
  }
}

function checkBudgets(report) {
  logHeader('Budget Analysis');
  
  let hasWarnings = false;
  let hasErrors = false;
  
  // Check initial bundle size
  const initialSize = report.summary.initialBundleSize;
  if (initialSize > BUDGET_LIMITS.initial) {
    logError(`Initial bundle exceeds 1MB limit: ${formatBytes(initialSize)}`);
    hasErrors = true;
  } else if (initialSize > BUDGET_LIMITS.warning) {
    logWarning(`Initial bundle approaching limit: ${formatBytes(initialSize)} (>${formatBytes(BUDGET_LIMITS.warning)})`);
    hasWarnings = true;
  } else {
    logSuccess(`Initial bundle within budget: ${formatBytes(initialSize)}`);
  }
  
  // Check total bundle size
  const totalSize = report.summary.totalSize;
  if (totalSize > BUDGET_LIMITS.total) {
    logError(`Total bundle exceeds 2MB limit: ${formatBytes(totalSize)}`);
    hasErrors = true;
  } else {
    logSuccess(`Total bundle within budget: ${formatBytes(totalSize)}`);
  }
  
  // Check individual assets
  const oversizedAssets = report.assets.largest.filter(asset => asset.size > BUDGET_LIMITS.asset);
  if (oversizedAssets.length > 0) {
    logWarning(`${oversizedAssets.length} assets exceed 400KB:`);
    oversizedAssets.forEach(asset => {
      log(`  • ${asset.name}: ${asset.sizeFormatted}`, COLORS.yellow);
    });
    hasWarnings = true;
  }
  
  // Performance score
  const performanceScore = calculatePerformanceScore(report);
  log(`\n🎯 Performance Score: ${performanceScore}/100`, 
      performanceScore >= 80 ? COLORS.green : 
      performanceScore >= 60 ? COLORS.yellow : COLORS.red);
  
  if (hasErrors) {
    logError('Build has budget violations that must be addressed!');
    process.exit(1);
  } else if (hasWarnings) {
    logWarning('Build has warnings - consider optimizing further');
  }
}

function calculatePerformanceScore(report) {
  let score = 100;
  
  // Deduct points for bundle size
  const initialSize = report.summary.initialBundleSize;
  if (initialSize > BUDGET_LIMITS.warning) {
    const overage = (initialSize - BUDGET_LIMITS.warning) / BUDGET_LIMITS.warning;
    score -= Math.min(30, overage * 100); // Max 30 points deduction
  }
  
  // Deduct points for poor chunk distribution
  const lazyRatio = report.chunks.lazy / report.chunks.total;
  if (lazyRatio < 0.5) {
    score -= 20; // 20 points for poor lazy loading
  }
  
  // Deduct points for large assets
  const largestAsset = report.assets.largest[0];
  if (largestAsset && largestAsset.size > BUDGET_LIMITS.asset) {
    score -= 15; // 15 points for oversized assets
  }
  
  return Math.max(0, Math.round(score));
}

function generateRecommendations(report) {
  logHeader('Optimization Recommendations');
  
  const recommendations = [];
  
  // Bundle size recommendations
  if (report.summary.initialBundleSize > BUDGET_LIMITS.warning) {
    recommendations.push('🔧 Implement more aggressive lazy loading for feature modules');
    recommendations.push('🔧 Consider code splitting for large components');
  }
  
  // Chunk recommendations
  const lazyRatio = report.chunks.lazy / report.chunks.total;
  if (lazyRatio < 0.6) {
    recommendations.push('🔧 Increase lazy loading - only ' + Math.round(lazyRatio * 100) + '% of chunks are lazy');
  }
  
  // Asset recommendations
  const largestAssets = report.assets.largest.slice(0, 3);
  const primeNGAssets = largestAssets.filter(asset => 
    asset.name.toLowerCase().includes('primeng') || asset.size > 300 * 1024
  );
  
  if (primeNGAssets.length > 0) {
    recommendations.push('🔧 Optimize PrimeNG imports - use standalone components');
  }
  
  // Tree shaking recommendations
  if (report.summary.totalSize > 1.5 * 1024 * 1024) {
    recommendations.push('🔧 Enable more aggressive tree shaking');
    recommendations.push('🔧 Remove unused dependencies');
  }
  
  if (recommendations.length === 0) {
    logSuccess('No optimization recommendations - bundle is well optimized!');
  } else {
    recommendations.forEach(rec => log(rec, COLORS.blue));
  }
}

function saveBuildMetrics(report) {
  const metricsFile = path.join(process.cwd(), 'build-metrics.json');
  
  const metrics = {
    timestamp: new Date().toISOString(),
    buildNumber: process.env.BUILD_NUMBER || Date.now(),
    gitCommit: getGitCommit(),
    bundleSize: report.summary.totalSize,
    initialBundleSize: report.summary.initialBundleSize,
    chunkCount: report.chunks.total,
    lazyChunkCount: report.chunks.lazy,
    performanceScore: calculatePerformanceScore(report),
    budgetStatus: {
      withinBudget: report.summary.initialBundleSize <= BUDGET_LIMITS.initial,
      overage: Math.max(0, report.summary.initialBundleSize - BUDGET_LIMITS.initial)
    }
  };
  
  // Read existing metrics
  let history = [];
  if (fs.existsSync(metricsFile)) {
    try {
      history = JSON.parse(fs.readFileSync(metricsFile, 'utf8'));
    } catch (error) {
      log('Could not read existing metrics file', COLORS.yellow);
    }
  }
  
  // Add current metrics
  history.push(metrics);
  
  // Keep only last 100 builds
  if (history.length > 100) {
    history = history.slice(-100);
  }
  
  // Save metrics
  fs.writeFileSync(metricsFile, JSON.stringify(history, null, 2));
  
  log(`📈 Build metrics saved (${history.length} builds tracked)`);
}

function getGitCommit() {
  try {
    return execSync('git rev-parse HEAD', { encoding: 'utf8' }).trim();
  } catch (error) {
    return 'unknown';
  }
}

// CI/CD Integration helpers
function generateCIReport() {
  const reportFile = path.join(process.cwd(), 'bundle-report.json');
  
  if (!fs.existsSync(reportFile)) {
    logError('Bundle report not found. Run build first.');
    return;
  }
  
  const report = JSON.parse(fs.readFileSync(reportFile, 'utf8'));
  
  // Generate CI-friendly output
  const ciReport = {
    status: report.summary.budgetStatus.isOverBudget ? 'FAIL' : 'PASS',
    initialBundleSize: report.summary.initialBundleSizeFormatted,
    totalBundleSize: report.summary.totalSizeFormatted,
    budgetStatus: report.summary.budgetStatus,
    recommendations: report.recommendations,
    performanceScore: calculatePerformanceScore(report)
  };
  
  // Output for CI systems
  console.log('::group::Bundle Analysis Report');
  console.log(JSON.stringify(ciReport, null, 2));
  console.log('::endgroup::');
  
  // Set GitHub Actions outputs if available
  if (process.env.GITHUB_ACTIONS) {
    console.log(`::set-output name=bundle-size::${report.summary.initialBundleSize}`);
    console.log(`::set-output name=budget-status::${ciReport.status}`);
    console.log(`::set-output name=performance-score::${ciReport.performanceScore}`);
  }
  
  return ciReport;
}

// Main execution
if (require.main === module) {
  const command = process.argv[2];
  
  switch (command) {
    case 'build':
      runBuild();
      break;
    case 'ci-report':
      generateCIReport();
      break;
    default:
      runBuild();
  }
}

module.exports = {
  runBuild,
  checkBudgets,
  generateRecommendations,
  calculatePerformanceScore,
  generateCIReport
};