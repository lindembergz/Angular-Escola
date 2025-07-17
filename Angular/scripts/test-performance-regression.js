#!/usr/bin/env node

/**
 * Performance Regression Test Runner
 * Node.js-based test runner for performance regression validation
 */

const fs = require('fs');
const path = require('path');

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

function logSuccess(message) {
  log(`✅ ${message}`, COLORS.green);
}

function logWarning(message) {
  log(`⚠️  ${message}`, COLORS.yellow);
}

function logError(message) {
  log(`❌ ${message}`, COLORS.red);
}

function logHeader(message) {
  log(`\n${COLORS.bold}${COLORS.blue}${message}${COLORS.reset}`);
  log('='.repeat(message.length), COLORS.blue);
}

class PerformanceRegressionValidator {
  constructor() {
    this.STATS_FILE = path.join(process.cwd(), 'dist/sistema-gestao-escolar-frontend/stats.json');
    this.BASELINE_FILE = path.join(process.cwd(), 'baseline-bundle-report.json');
    this.PERFORMANCE_LOG_FILE = path.join(process.cwd(), 'performance-history.json');
    
    this.THRESHOLDS = {
      bundleSizeIncrease: 5, // Max 5% increase
      performanceDegradation: 10 // Max 10% degradation
    };
    
    this.currentMetrics = null;
    this.baselineMetrics = null;
    this.testResults = {
      passed: 0,
      failed: 0,
      warnings: 0
    };
  }

  async runTests() {
    logHeader('Performance Regression Tests');
    
    try {
      this.loadData();
      
      await this.testBundleSizeRegression();
      await this.testInitialBundleRegression();
      await this.testChunkSplittingRegression();
      await this.testPerformanceMetricsLogging();
      await this.testPerformanceTrends();
      await this.testAssetSizeDistribution();
      
      this.printSummary();
      
      if (this.testResults.failed > 0) {
        process.exit(1);
      }
      
    } catch (error) {
      logError(`Test execution failed: ${error.message}`);
      process.exit(1);
    }
  }

  loadData() {
    if (!fs.existsSync(this.STATS_FILE)) {
      throw new Error('Bundle stats file not found. Run "npm run build:stats" first.');
    }

    this.currentMetrics = this.extractMetricsFromStats();
    
    if (fs.existsSync(this.BASELINE_FILE)) {
      const baseline = JSON.parse(fs.readFileSync(this.BASELINE_FILE, 'utf8'));
      this.baselineMetrics = this.extractMetricsFromBaseline(baseline);
    }
    
    log(`📊 Current metrics loaded: ${this.formatBytes(this.currentMetrics.bundleSize)} total bundle`);
  }

  convertStatsFormat(rawStats) {
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

  async testBundleSizeRegression() {
    const testName = 'Bundle Size Regression';
    log(`\n🧪 Testing: ${testName}`);
    
    if (!this.baselineMetrics) {
      logWarning(`${testName}: No baseline found - skipping regression test`);
      this.testResults.warnings++;
      return;
    }

    const currentSize = this.currentMetrics.bundleSize;
    const baselineSize = this.baselineMetrics.bundleSize;
    const increasePercent = ((currentSize - baselineSize) / baselineSize) * 100;

    if (increasePercent <= this.THRESHOLDS.bundleSizeIncrease) {
      if (increasePercent <= 0) {
        logSuccess(`${testName}: Bundle size improved by ${Math.abs(increasePercent).toFixed(1)}%`);
      } else {
        logSuccess(`${testName}: Bundle size increase ${increasePercent.toFixed(1)}% (within ${this.THRESHOLDS.bundleSizeIncrease}% threshold)`);
      }
      this.testResults.passed++;
    } else {
      logError(`${testName}: Bundle size increased by ${increasePercent.toFixed(1)}% (exceeds ${this.THRESHOLDS.bundleSizeIncrease}% threshold)`);
      this.testResults.failed++;
    }
  }

  async testInitialBundleRegression() {
    const testName = 'Initial Bundle Regression';
    log(`\n🧪 Testing: ${testName}`);
    
    if (!this.baselineMetrics) {
      logWarning(`${testName}: No baseline found - skipping initial bundle regression test`);
      this.testResults.warnings++;
      return;
    }

    const currentSize = this.currentMetrics.initialBundleSize;
    const baselineSize = this.baselineMetrics.initialBundleSize;
    const increasePercent = ((currentSize - baselineSize) / baselineSize) * 100;

    log(`📊 Initial bundle: ${this.formatBytes(currentSize)} (${increasePercent > 0 ? '+' : ''}${increasePercent.toFixed(1)}%)`);

    if (increasePercent <= this.THRESHOLDS.bundleSizeIncrease) {
      logSuccess(`${testName}: Initial bundle change within acceptable range`);
      this.testResults.passed++;
    } else {
      logError(`${testName}: Initial bundle increased by ${increasePercent.toFixed(1)}% (exceeds ${this.THRESHOLDS.bundleSizeIncrease}% threshold)`);
      this.testResults.failed++;
    }
  }

  async testChunkSplittingRegression() {
    const testName = 'Chunk Splitting Regression';
    log(`\n🧪 Testing: ${testName}`);
    
    if (!this.baselineMetrics) {
      logWarning(`${testName}: No baseline found - skipping chunk splitting regression test`);
      this.testResults.warnings++;
      return;
    }

    const currentLazyRatio = this.currentMetrics.lazyChunkCount / this.currentMetrics.chunkCount;
    const baselineLazyRatio = this.baselineMetrics.lazyChunkCount / this.baselineMetrics.chunkCount;

    log(`📊 Lazy chunks: ${this.currentMetrics.lazyChunkCount}/${this.currentMetrics.chunkCount} (${(currentLazyRatio * 100).toFixed(1)}%)`);

    // Lazy chunk ratio should not decrease significantly
    if (currentLazyRatio >= baselineLazyRatio * 0.9) {
      logSuccess(`${testName}: Lazy loading ratio maintained or improved`);
      this.testResults.passed++;
    } else {
      logError(`${testName}: Lazy loading ratio decreased significantly`);
      this.testResults.failed++;
    }
  }

  async testPerformanceMetricsLogging() {
    const testName = 'Performance Metrics Logging';
    log(`\n🧪 Testing: ${testName}`);
    
    try {
      const performanceEntry = {
        timestamp: new Date().toISOString(),
        metrics: this.currentMetrics,
        buildInfo: {
          nodeVersion: process.version,
          platform: process.platform
        }
      };

      // Read existing history
      let history = [];
      if (fs.existsSync(this.PERFORMANCE_LOG_FILE)) {
        try {
          history = JSON.parse(fs.readFileSync(this.PERFORMANCE_LOG_FILE, 'utf8'));
        } catch (error) {
          logWarning('Could not read performance history file');
        }
      }

      // Add current entry
      history.push(performanceEntry);

      // Keep only last 50 entries
      if (history.length > 50) {
        history = history.slice(-50);
      }

      // Write back to file
      fs.writeFileSync(this.PERFORMANCE_LOG_FILE, JSON.stringify(history, null, 2));

      logSuccess(`${testName}: Performance metrics logged (${history.length} entries in history)`);
      this.testResults.passed++;
      
    } catch (error) {
      logError(`${testName}: Failed to log performance metrics - ${error.message}`);
      this.testResults.failed++;
    }
  }

  async testPerformanceTrends() {
    const testName = 'Performance Trends Analysis';
    log(`\n🧪 Testing: ${testName}`);
    
    if (!fs.existsSync(this.PERFORMANCE_LOG_FILE)) {
      logWarning(`${testName}: No performance history found`);
      this.testResults.warnings++;
      return;
    }

    const history = JSON.parse(fs.readFileSync(this.PERFORMANCE_LOG_FILE, 'utf8'));
    
    if (history.length < 3) {
      logWarning(`${testName}: Insufficient history for trend analysis (${history.length} entries)`);
      this.testResults.warnings++;
      return;
    }

    // Analyze last 5 builds
    const recentBuilds = history.slice(-5);
    const bundleSizes = recentBuilds.map(entry => entry.metrics.bundleSize);
    
    // Check if there's a consistent upward trend
    let increasingTrend = 0;
    for (let i = 1; i < bundleSizes.length; i++) {
      if (bundleSizes[i] > bundleSizes[i - 1]) {
        increasingTrend++;
      }
    }

    const trendPercentage = (increasingTrend / (bundleSizes.length - 1)) * 100;
    
    if (trendPercentage < 80) {
      logSuccess(`${testName}: No concerning upward trend detected (${trendPercentage.toFixed(0)}% of recent builds increased)`);
      this.testResults.passed++;
    } else {
      logError(`${testName}: Concerning upward trend detected (${trendPercentage.toFixed(0)}% of recent builds increased)`);
      this.testResults.failed++;
    }
  }

  async testAssetSizeDistribution() {
    const testName = 'Asset Size Distribution';
    log(`\n🧪 Testing: ${testName}`);
    
    const maxReasonableSize = 800 * 1024; // 800KB
    
    if (this.currentMetrics.largestAssetSize < maxReasonableSize) {
      logSuccess(`${testName}: Largest asset ${this.formatBytes(this.currentMetrics.largestAssetSize)} within reasonable limit`);
      this.testResults.passed++;
    } else {
      logWarning(`${testName}: Largest asset ${this.formatBytes(this.currentMetrics.largestAssetSize)} exceeds 800KB`);
      this.testResults.warnings++;
    }
    
    if (this.baselineMetrics) {
      const increasePercent = ((this.currentMetrics.largestAssetSize - this.baselineMetrics.largestAssetSize) / this.baselineMetrics.largestAssetSize) * 100;
      
      if (increasePercent <= this.THRESHOLDS.performanceDegradation) {
        log(`  Asset size change: ${increasePercent > 0 ? '+' : ''}${increasePercent.toFixed(1)}% (within threshold)`);
      } else {
        logWarning(`  Asset size increased by ${increasePercent.toFixed(1)}% (exceeds ${this.THRESHOLDS.performanceDegradation}% threshold)`);
      }
    }
  }

  extractMetricsFromStats() {
    const rawStats = JSON.parse(fs.readFileSync(this.STATS_FILE, 'utf8'));
    const stats = this.convertStatsFormat(rawStats);
    
    const totalSize = stats.assets.reduce((sum, asset) => sum + asset.size, 0);
    const initialChunks = stats.chunks.filter(chunk => chunk.initial);
    const initialSize = initialChunks.reduce((sum, chunk) => {
      return sum + chunk.files.reduce((fileSum, fileName) => {
        const asset = stats.assets.find(a => a.name === fileName);
        return fileSum + (asset ? asset.size : 0);
      }, 0);
    }, 0);
    
    const largestAsset = stats.assets.reduce((largest, asset) => 
      asset.size > largest.size ? asset : largest, { size: 0 }
    );

    return {
      bundleSize: totalSize,
      initialBundleSize: initialSize,
      chunkCount: stats.chunks.length,
      lazyChunkCount: stats.chunks.filter(chunk => !chunk.initial).length,
      largestAssetSize: largestAsset.size,
      timestamp: new Date().toISOString()
    };
  }

  extractMetricsFromBaseline(baseline) {
    return {
      bundleSize: baseline.summary?.totalSize || 0,
      initialBundleSize: baseline.summary?.initialBundleSize || 0,
      chunkCount: baseline.chunks?.total || 0,
      lazyChunkCount: baseline.chunks?.lazy || 0,
      largestAssetSize: baseline.assets?.largest?.[0]?.size || 0,
      timestamp: baseline.timestamp || ''
    };
  }

  printSummary() {
    logHeader('Test Summary');
    
    const total = this.testResults.passed + this.testResults.failed + this.testResults.warnings;
    
    logSuccess(`Passed: ${this.testResults.passed}/${total}`);
    
    if (this.testResults.warnings > 0) {
      logWarning(`Warnings: ${this.testResults.warnings}/${total}`);
    }
    
    if (this.testResults.failed > 0) {
      logError(`Failed: ${this.testResults.failed}/${total}`);
    }
    
    if (this.testResults.failed === 0) {
      logSuccess('🎉 All critical performance regression tests passed!');
    } else {
      logError('💥 Performance regression detected - optimization needed');
    }
  }

  formatBytes(bytes) {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i];
  }
}

// Main execution
if (require.main === module) {
  const validator = new PerformanceRegressionValidator();
  validator.runTests().catch(error => {
    logError(`Unexpected error: ${error.message}`);
    process.exit(1);
  });
}

module.exports = PerformanceRegressionValidator;