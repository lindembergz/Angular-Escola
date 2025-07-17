#!/usr/bin/env node

/**
 * Comprehensive Optimization Validation Script
 * Validates that all bundle optimizations maintain functionality
 */

const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

class OptimizationValidator {
  constructor() {
    this.results = {
      bundleSize: null,
      performance: null,
      lazyLoading: null,
      primeNGOptimization: null,
      crossBrowser: null,
      networkConditions: null,
      overall: null
    };
    this.startTime = Date.now();
  }

  log(message, type = 'info') {
    const timestamp = new Date().toISOString();
    const prefix = {
      info: '🔍',
      success: '✅',
      error: '❌',
      warning: '⚠️'
    }[type] || '🔍';
    
    console.log(`${prefix} [${timestamp}] ${message}`);
  }

  async validateOptimizations() {
    this.log('Starting comprehensive optimization validation...', 'info');
    
    try {
      // 1. Validate bundle size optimizations
      await this.validateBundleSize();
      
      // 2. Validate performance improvements
      await this.validatePerformance();
      
      // 3. Validate lazy loading implementation
      await this.validateLazyLoading();
      
      // 4. Validate PrimeNG optimizations
      await this.validatePrimeNGOptimizations();
      
      // 5. Validate cross-browser compatibility
      await this.validateCrossBrowserCompatibility();
      
      // 6. Validate network conditions handling
      await this.validateNetworkConditions();
      
      // 7. Generate comprehensive report
      await this.generateValidationReport();
      
      this.log('Optimization validation completed!', 'success');
      
    } catch (error) {
      this.log(`Validation failed: ${error.message}`, 'error');
      process.exit(1);
    }
  }

  async validateBundleSize() {
    this.log('Validating bundle size optimizations...', 'info');
    
    try {
      const output = execSync('node scripts/test-bundle-size.js', {
        encoding: 'utf8',
        cwd: process.cwd()
      });
      
      // Parse output to determine success
      const passed = output.includes('✅ Passed:');
      const failed = output.includes('❌ Failed:');
      
      this.results.bundleSize = {
        passed: passed && !failed,
        output: output,
        metrics: this.extractBundleMetrics(output)
      };
      
      if (this.results.bundleSize.passed) {
        this.log('Bundle size validation passed', 'success');
      } else {
        this.log('Bundle size validation failed', 'error');
      }
      
    } catch (error) {
      this.results.bundleSize = {
        passed: false,
        output: null,
        error: error.message,
        metrics: null
      };
      this.log('Bundle size validation failed', 'error');
    }
  }

  async validatePerformance() {
    this.log('Validating performance improvements...', 'info');
    
    try {
      const output = execSync('node scripts/test-performance-regression.js', {
        encoding: 'utf8',
        cwd: process.cwd()
      });
      
      const passed = output.includes('✅ Passed:');
      const failed = output.includes('❌ Failed:');
      
      this.results.performance = {
        passed: passed && !failed,
        output: output,
        metrics: this.extractPerformanceMetrics(output)
      };
      
      if (this.results.performance.passed) {
        this.log('Performance validation passed', 'success');
      } else {
        this.log('Performance validation failed', 'error');
      }
      
    } catch (error) {
      this.results.performance = {
        passed: false,
        output: null,
        error: error.message,
        metrics: null
      };
      this.log('Performance validation failed', 'error');
    }
  }

  async validateLazyLoading() {
    this.log('Validating lazy loading implementation...', 'info');
    
    try {
      // Check route configuration
      const routesPath = path.join(process.cwd(), 'src/app/app.routes.ts');
      const routesContent = fs.readFileSync(routesPath, 'utf8');
      
      const lazyLoadingChecks = {
        hasLoadComponent: routesContent.includes('loadComponent'),
        hasLoadChildren: routesContent.includes('loadChildren'),
        hasErrorHandling: routesContent.includes('catch'),
        hasGuards: routesContent.includes('canActivate')
      };
      
      const passedChecks = Object.values(lazyLoadingChecks).filter(Boolean).length;
      const totalChecks = Object.keys(lazyLoadingChecks).length;
      
      this.results.lazyLoading = {
        passed: passedChecks >= totalChecks * 0.75, // 75% of checks must pass
        checks: lazyLoadingChecks,
        score: (passedChecks / totalChecks * 100).toFixed(1)
      };
      
      if (this.results.lazyLoading.passed) {
        this.log(`Lazy loading validation passed (${this.results.lazyLoading.score}%)`, 'success');
      } else {
        this.log(`Lazy loading validation failed (${this.results.lazyLoading.score}%)`, 'error');
      }
      
    } catch (error) {
      this.results.lazyLoading = {
        passed: false,
        error: error.message,
        checks: null,
        score: '0'
      };
      this.log('Lazy loading validation failed', 'error');
    }
  }

  async validatePrimeNGOptimizations() {
    this.log('Validating PrimeNG optimizations...', 'info');
    
    try {
      // Check for optimized imports in source files
      const srcPath = path.join(process.cwd(), 'src');
      const primeNGChecks = await this.checkPrimeNGOptimizations(srcPath);
      
      this.results.primeNGOptimization = {
        passed: primeNGChecks.optimizedImports > primeNGChecks.moduleImports,
        checks: primeNGChecks,
        score: primeNGChecks.optimizedImports > 0 ? 
          ((primeNGChecks.optimizedImports / (primeNGChecks.optimizedImports + primeNGChecks.moduleImports)) * 100).toFixed(1) : '0'
      };
      
      if (this.results.primeNGOptimization.passed) {
        this.log(`PrimeNG optimization validation passed (${this.results.primeNGOptimization.score}%)`, 'success');
      } else {
        this.log(`PrimeNG optimization validation failed (${this.results.primeNGOptimization.score}%)`, 'warning');
      }
      
    } catch (error) {
      this.results.primeNGOptimization = {
        passed: false,
        error: error.message,
        checks: null,
        score: '0'
      };
      this.log('PrimeNG optimization validation failed', 'error');
    }
  }

  async validateCrossBrowserCompatibility() {
    this.log('Validating cross-browser compatibility...', 'info');
    
    try {
      // Check build configuration for browser support
      const angularJsonPath = path.join(process.cwd(), 'angular.json');
      const angularJson = JSON.parse(fs.readFileSync(angularJsonPath, 'utf8'));
      
      const buildConfig = angularJson.projects['sistema-gestao-escolar-frontend'].architect.build;
      const hasOptimization = buildConfig.configurations.production.optimization;
      const hasPolyfills = buildConfig.options.polyfills && buildConfig.options.polyfills.length > 0;
      
      // Check for modern JavaScript features
      const tsConfigPath = path.join(process.cwd(), 'tsconfig.json');
      const tsConfig = JSON.parse(fs.readFileSync(tsConfigPath, 'utf8'));
      const targetES = tsConfig.compilerOptions.target;
      
      const compatibilityChecks = {
        hasOptimization: !!hasOptimization,
        hasPolyfills: hasPolyfills,
        modernTarget: targetES && (targetES.includes('ES2022') || targetES.includes('ES2020')),
        hasSourceMaps: buildConfig.configurations.development.sourceMap
      };
      
      const passedChecks = Object.values(compatibilityChecks).filter(Boolean).length;
      const totalChecks = Object.keys(compatibilityChecks).length;
      
      this.results.crossBrowser = {
        passed: passedChecks >= totalChecks * 0.75,
        checks: compatibilityChecks,
        score: (passedChecks / totalChecks * 100).toFixed(1)
      };
      
      if (this.results.crossBrowser.passed) {
        this.log(`Cross-browser compatibility validation passed (${this.results.crossBrowser.score}%)`, 'success');
      } else {
        this.log(`Cross-browser compatibility validation failed (${this.results.crossBrowser.score}%)`, 'warning');
      }
      
    } catch (error) {
      this.results.crossBrowser = {
        passed: false,
        error: error.message,
        checks: null,
        score: '0'
      };
      this.log('Cross-browser compatibility validation failed', 'error');
    }
  }

  async validateNetworkConditions() {
    this.log('Validating network conditions handling...', 'info');
    
    try {
      // Check for network-aware optimizations
      const checks = {
        hasLazyLoading: this.results.lazyLoading?.passed || false,
        hasCodeSplitting: this.results.bundleSize?.metrics?.lazyChunks > 0,
        hasErrorHandling: this.results.lazyLoading?.checks?.hasErrorHandling || false,
        hasOptimizedBundle: this.results.bundleSize?.metrics?.initialSize < 1024 * 1024 // < 1MB
      };
      
      const passedChecks = Object.values(checks).filter(Boolean).length;
      const totalChecks = Object.keys(checks).length;
      
      this.results.networkConditions = {
        passed: passedChecks >= totalChecks * 0.75,
        checks: checks,
        score: (passedChecks / totalChecks * 100).toFixed(1)
      };
      
      if (this.results.networkConditions.passed) {
        this.log(`Network conditions validation passed (${this.results.networkConditions.score}%)`, 'success');
      } else {
        this.log(`Network conditions validation failed (${this.results.networkConditions.score}%)`, 'warning');
      }
      
    } catch (error) {
      this.results.networkConditions = {
        passed: false,
        error: error.message,
        checks: null,
        score: '0'
      };
      this.log('Network conditions validation failed', 'error');
    }
  }

  async checkPrimeNGOptimizations(srcPath) {
    const checks = {
      optimizedImports: 0,
      moduleImports: 0,
      files: []
    };
    
    const checkFile = (filePath) => {
      if (!filePath.endsWith('.ts') && !filePath.endsWith('.js')) return;
      
      try {
        const content = fs.readFileSync(filePath, 'utf8');
        
        // Count optimized imports (specific component imports)
        const optimizedMatches = content.match(/import\s*{\s*\w+\s*}\s*from\s*['"]primeng\/\w+['"]/g);
        if (optimizedMatches) {
          checks.optimizedImports += optimizedMatches.length;
        }
        
        // Count module imports (should be avoided)
        const moduleMatches = content.match(/import\s*{\s*\w+Module\s*}\s*from\s*['"]primeng\/\w+['"]/g);
        if (moduleMatches) {
          checks.moduleImports += moduleMatches.length;
        }
        
        if (optimizedMatches || moduleMatches) {
          checks.files.push({
            path: filePath,
            optimized: optimizedMatches?.length || 0,
            modules: moduleMatches?.length || 0
          });
        }
      } catch (error) {
        // Ignore file read errors
      }
    };
    
    const walkDir = (dir) => {
      try {
        const files = fs.readdirSync(dir);
        files.forEach(file => {
          const filePath = path.join(dir, file);
          const stat = fs.statSync(filePath);
          
          if (stat.isDirectory() && !file.includes('node_modules')) {
            walkDir(filePath);
          } else if (stat.isFile()) {
            checkFile(filePath);
          }
        });
      } catch (error) {
        // Ignore directory read errors
      }
    };
    
    walkDir(srcPath);
    return checks;
  }

  extractBundleMetrics(output) {
    const metrics = {};
    
    // Extract initial bundle size
    const initialMatch = output.match(/Initial Bundle Size: ([\d.]+)\s*(\w+)/);
    if (initialMatch) {
      const size = parseFloat(initialMatch[1]);
      const unit = initialMatch[2];
      metrics.initialSize = unit === 'MB' ? size * 1024 * 1024 : size * 1024;
    }
    
    // Extract lazy chunks count
    const lazyMatch = output.match(/(\d+)% lazy chunks/);
    if (lazyMatch) {
      metrics.lazyChunks = parseInt(lazyMatch[1]);
    }
    
    return metrics;
  }

  extractPerformanceMetrics(output) {
    const metrics = {};
    
    // Extract improvement percentage
    const improvementMatch = output.match(/Bundle size improved by ([\d.]+)%/);
    if (improvementMatch) {
      metrics.improvement = parseFloat(improvementMatch[1]);
    }
    
    return metrics;
  }

  async generateValidationReport() {
    this.log('Generating validation report...', 'info');
    
    const duration = Date.now() - this.startTime;
    const passedTests = Object.values(this.results).filter(r => r && r.passed).length;
    const totalTests = Object.values(this.results).filter(r => r !== null).length;
    
    this.results.overall = {
      passed: passedTests,
      total: totalTests,
      successRate: totalTests > 0 ? ((passedTests / totalTests) * 100).toFixed(1) : '0',
      duration: duration
    };
    
    const report = {
      timestamp: new Date().toISOString(),
      duration: duration,
      summary: this.results.overall,
      details: this.results,
      recommendations: this.generateRecommendations()
    };
    
    // Write detailed report
    const reportPath = path.join(process.cwd(), 'optimization-validation-report.json');
    fs.writeFileSync(reportPath, JSON.stringify(report, null, 2));
    
    // Write summary
    const summaryPath = path.join(process.cwd(), 'optimization-validation-summary.md');
    const summaryContent = this.generateMarkdownSummary(report);
    fs.writeFileSync(summaryPath, summaryContent);
    
    this.log(`Validation report generated: ${reportPath}`, 'success');
    this.log(`Validation summary generated: ${summaryPath}`, 'success');
    
    // Print console summary
    this.printConsoleSummary(report);
  }

  generateRecommendations() {
    const recommendations = [];
    
    if (!this.results.bundleSize?.passed) {
      recommendations.push({
        category: 'Bundle Size',
        priority: 'high',
        message: 'Bundle size exceeds target. Consider further code splitting and tree shaking.',
        action: 'Review large chunks and optimize imports'
      });
    }
    
    if (!this.results.performance?.passed) {
      recommendations.push({
        category: 'Performance',
        priority: 'high',
        message: 'Performance regression detected. Review recent changes.',
        action: 'Analyze performance metrics and optimize critical paths'
      });
    }
    
    if (!this.results.lazyLoading?.passed) {
      recommendations.push({
        category: 'Lazy Loading',
        priority: 'medium',
        message: 'Lazy loading implementation needs improvement.',
        action: 'Add more lazy loaded routes and error handling'
      });
    }
    
    if (!this.results.primeNGOptimization?.passed) {
      recommendations.push({
        category: 'PrimeNG',
        priority: 'medium',
        message: 'PrimeNG imports are not fully optimized.',
        action: 'Convert module imports to specific component imports'
      });
    }
    
    return recommendations;
  }

  generateMarkdownSummary(report) {
    return `# Optimization Validation Report

## Summary
- **Date**: ${new Date(report.timestamp).toLocaleString()}
- **Duration**: ${(report.duration / 1000).toFixed(2)} seconds
- **Tests Passed**: ${report.summary.passed}/${report.summary.total}
- **Success Rate**: ${report.summary.successRate}%

## Test Results

### Bundle Size Optimization
- **Status**: ${report.details.bundleSize?.passed ? '✅ PASSED' : '❌ FAILED'}
- **Initial Bundle**: ${report.details.bundleSize?.metrics?.initialSize ? this.formatBytes(report.details.bundleSize.metrics.initialSize) : 'N/A'}
- **Lazy Chunks**: ${report.details.bundleSize?.metrics?.lazyChunks || 'N/A'}%

### Performance Validation
- **Status**: ${report.details.performance?.passed ? '✅ PASSED' : '❌ FAILED'}
- **Improvement**: ${report.details.performance?.metrics?.improvement || 'N/A'}%

### Lazy Loading Implementation
- **Status**: ${report.details.lazyLoading?.passed ? '✅ PASSED' : '❌ FAILED'}
- **Score**: ${report.details.lazyLoading?.score || 'N/A'}%

### PrimeNG Optimization
- **Status**: ${report.details.primeNGOptimization?.passed ? '✅ PASSED' : '⚠️ NEEDS IMPROVEMENT'}
- **Score**: ${report.details.primeNGOptimization?.score || 'N/A'}%

### Cross-Browser Compatibility
- **Status**: ${report.details.crossBrowser?.passed ? '✅ PASSED' : '⚠️ NEEDS IMPROVEMENT'}
- **Score**: ${report.details.crossBrowser?.score || 'N/A'}%

### Network Conditions Handling
- **Status**: ${report.details.networkConditions?.passed ? '✅ PASSED' : '⚠️ NEEDS IMPROVEMENT'}
- **Score**: ${report.details.networkConditions?.score || 'N/A'}%

## Recommendations

${report.recommendations.map(rec => `
### ${rec.category} (${rec.priority.toUpperCase()} Priority)
${rec.message}
- **Action**: ${rec.action}
`).join('\n')}

---
*Generated by Optimization Validator*
`;
  }

  printConsoleSummary(report) {
    console.log('\n' + '='.repeat(60));
    console.log('OPTIMIZATION VALIDATION SUMMARY');
    console.log('='.repeat(60));
    console.log(`Tests Passed: ${report.summary.passed}/${report.summary.total}`);
    console.log(`Success Rate: ${report.summary.successRate}%`);
    console.log(`Duration: ${(report.duration / 1000).toFixed(2)}s`);
    console.log('='.repeat(60));
    
    Object.entries(report.details).forEach(([key, result]) => {
      if (result && key !== 'overall') {
        const status = result.passed ? '✅' : '❌';
        const score = result.score ? ` (${result.score}%)` : '';
        console.log(`${status} ${key.replace(/([A-Z])/g, ' $1').trim()}${score}`);
      }
    });
    
    console.log('='.repeat(60));
    
    if (report.recommendations.length > 0) {
      console.log('\nRECOMMENDATIONS:');
      report.recommendations.forEach(rec => {
        const priority = rec.priority === 'high' ? '🔴' : '🟡';
        console.log(`${priority} ${rec.category}: ${rec.message}`);
      });
    }
  }

  formatBytes(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }
}

// Run the validation if this script is executed directly
if (require.main === module) {
  const validator = new OptimizationValidator();
  validator.validateOptimizations().catch(error => {
    console.error('Optimization validation failed:', error);
    process.exit(1);
  });
}

module.exports = OptimizationValidator;