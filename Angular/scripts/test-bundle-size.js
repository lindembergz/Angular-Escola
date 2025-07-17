#!/usr/bin/env node

/**
 * Bundle Size Test Runner
 * Node.js-based test runner for bundle size validation
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

class BundleSizeValidator {
  constructor() {
    this.STATS_FILE = path.join(process.cwd(), 'dist/sistema-gestao-escolar-frontend/stats.json');
    this.BASELINE_FILE = path.join(process.cwd(), 'baseline-bundle-report.json');
    this.BUDGET = {
      initialBundle: 1024 * 1024, // 1MB
      totalBundle: 3 * 1024 * 1024, // 3MB
      maxChunkSize: 500 * 1024, // 500KB
      maxAssetSize: 1024 * 1024 // 1MB
    };
    
    this.stats = null;
    this.baseline = null;
    this.testResults = {
      passed: 0,
      failed: 0,
      warnings: 0
    };
  }

  async runTests() {
    logHeader('Bundle Size Validation Tests');
    
    try {
      this.loadData();
      
      await this.testInitialBundleSize();
      await this.testTotalBundleSize();
      await this.testAssetSizes();
      await this.testChunkSizes();
      await this.testLazyLoading();
      await this.testBaselineComparison();
      
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
    
    const rawStats = JSON.parse(fs.readFileSync(this.STATS_FILE, 'utf8'));
    
    // Convert Angular CLI stats format to expected format
    this.stats = this.convertStatsFormat(rawStats);
    
    if (fs.existsSync(this.BASELINE_FILE)) {
      this.baseline = JSON.parse(fs.readFileSync(this.BASELINE_FILE, 'utf8'));
    }
    
    log(`📊 Loaded stats: ${this.stats.assets.length} assets, ${this.stats.chunks.length} chunks`);
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

  async testInitialBundleSize() {
    const testName = 'Initial Bundle Size';
    log(`\n🧪 Testing: ${testName}`);
    
    const initialChunks = this.stats.chunks.filter(chunk => chunk.initial);
    const initialSize = initialChunks.reduce((sum, chunk) => {
      return sum + chunk.files.reduce((fileSum, fileName) => {
        const asset = this.stats.assets.find(a => a.name === fileName);
        return fileSum + (asset ? asset.size : 0);
      }, 0);
    }, 0);

    const sizeFormatted = this.formatBytes(initialSize);
    const budgetFormatted = this.formatBytes(this.BUDGET.initialBundle);
    
    if (initialSize <= this.BUDGET.initialBundle) {
      logSuccess(`${testName}: ${sizeFormatted} (within ${budgetFormatted} budget)`);
      this.testResults.passed++;
    } else {
      const overage = initialSize - this.BUDGET.initialBundle;
      const overageFormatted = this.formatBytes(overage);
      logError(`${testName}: ${sizeFormatted} exceeds budget by ${overageFormatted}`);
      this.testResults.failed++;
    }
  }

  async testTotalBundleSize() {
    const testName = 'Total Bundle Size';
    log(`\n🧪 Testing: ${testName}`);
    
    const totalSize = this.stats.assets.reduce((sum, asset) => sum + asset.size, 0);
    const sizeFormatted = this.formatBytes(totalSize);
    const budgetFormatted = this.formatBytes(this.BUDGET.totalBundle);
    
    if (totalSize <= this.BUDGET.totalBundle) {
      logSuccess(`${testName}: ${sizeFormatted} (within ${budgetFormatted} budget)`);
      this.testResults.passed++;
    } else {
      logError(`${testName}: ${sizeFormatted} exceeds ${budgetFormatted} budget`);
      this.testResults.failed++;
    }
  }

  async testAssetSizes() {
    const testName = 'Individual Asset Sizes';
    log(`\n🧪 Testing: ${testName}`);
    
    const oversizedAssets = this.stats.assets.filter(asset => asset.size > this.BUDGET.maxAssetSize);
    
    if (oversizedAssets.length === 0) {
      logSuccess(`${testName}: All assets within ${this.formatBytes(this.BUDGET.maxAssetSize)} limit`);
      this.testResults.passed++;
    } else {
      logError(`${testName}: ${oversizedAssets.length} assets exceed size limit:`);
      oversizedAssets.forEach(asset => {
        log(`  • ${asset.name}: ${this.formatBytes(asset.size)}`, COLORS.red);
      });
      this.testResults.failed++;
    }
  }

  async testChunkSizes() {
    const testName = 'Chunk Sizes';
    log(`\n🧪 Testing: ${testName}`);
    
    const oversizedChunks = this.stats.chunks.filter(chunk => {
      const chunkSize = chunk.files.reduce((sum, fileName) => {
        const asset = this.stats.assets.find(a => a.name === fileName);
        return sum + (asset ? asset.size : 0);
      }, 0);
      return chunkSize > this.BUDGET.maxChunkSize && !chunk.initial; // Allow initial chunks to be larger
    });

    if (oversizedChunks.length === 0) {
      logSuccess(`${testName}: All lazy chunks within ${this.formatBytes(this.BUDGET.maxChunkSize)} limit`);
      this.testResults.passed++;
    } else {
      logWarning(`${testName}: ${oversizedChunks.length} lazy chunks exceed size limit`);
      this.testResults.warnings++;
    }
  }

  async testLazyLoading() {
    const testName = 'Lazy Loading Implementation';
    log(`\n🧪 Testing: ${testName}`);
    
    const totalChunks = this.stats.chunks.length;
    const initialChunks = this.stats.chunks.filter(chunk => chunk.initial).length;
    const lazyChunks = totalChunks - initialChunks;
    const lazyRatio = lazyChunks / totalChunks;
    
    if (lazyRatio >= 0.5) {
      logSuccess(`${testName}: ${Math.round(lazyRatio * 100)}% lazy chunks (${lazyChunks}/${totalChunks})`);
      this.testResults.passed++;
    } else {
      logWarning(`${testName}: Only ${Math.round(lazyRatio * 100)}% lazy chunks - consider more lazy loading`);
      this.testResults.warnings++;
    }
  }

  async testBaselineComparison() {
    const testName = 'Baseline Comparison';
    log(`\n🧪 Testing: ${testName}`);
    
    if (!this.baseline) {
      logWarning(`${testName}: No baseline found - skipping comparison`);
      this.testResults.warnings++;
      return;
    }

    const currentInitialSize = this.getCurrentInitialBundleSize();
    const baselineSize = this.baseline.summary?.initialBundleSize || 0;
    
    if (baselineSize === 0) {
      logWarning(`${testName}: Invalid baseline data`);
      this.testResults.warnings++;
      return;
    }

    const improvement = baselineSize - currentInitialSize;
    const improvementPercent = ((improvement / baselineSize) * 100).toFixed(1);
    
    if (improvement >= 0) {
      logSuccess(`${testName}: ${this.formatBytes(Math.abs(improvement))} improvement (${improvementPercent}%)`);
      this.testResults.passed++;
    } else {
      const increase = Math.abs(improvement);
      const increasePercent = Math.abs(parseFloat(improvementPercent));
      
      if (increasePercent <= 5) {
        logWarning(`${testName}: ${this.formatBytes(increase)} increase (${increasePercent}%) - within tolerance`);
        this.testResults.warnings++;
      } else {
        logError(`${testName}: ${this.formatBytes(increase)} increase (${increasePercent}%) - exceeds 5% threshold`);
        this.testResults.failed++;
      }
    }
  }

  getCurrentInitialBundleSize() {
    const initialChunks = this.stats.chunks.filter(chunk => chunk.initial);
    return initialChunks.reduce((sum, chunk) => {
      return sum + chunk.files.reduce((fileSum, fileName) => {
        const asset = this.stats.assets.find(a => a.name === fileName);
        return fileSum + (asset ? asset.size : 0);
      }, 0);
    }, 0);
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
      logSuccess('🎉 All critical tests passed!');
    } else {
      logError('💥 Some tests failed - bundle optimization needed');
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
  const validator = new BundleSizeValidator();
  validator.runTests().catch(error => {
    logError(`Unexpected error: ${error.message}`);
    process.exit(1);
  });
}

module.exports = BundleSizeValidator;