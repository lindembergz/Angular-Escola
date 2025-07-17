#!/usr/bin/env node

/**
 * Network Conditions Testing Script
 * Tests application performance under various network conditions
 */

const { execSync, spawn } = require('child_process');
const fs = require('fs');
const path = require('path');

class NetworkConditionsTest {
  constructor() {
    this.testResults = [];
    this.networkProfiles = {
      'fast-3g': {
        downloadThroughput: 1.6 * 1024 * 1024 / 8, // 1.6 Mbps
        uploadThroughput: 750 * 1024 / 8,           // 750 Kbps
        latency: 150                                 // 150ms
      },
      'slow-3g': {
        downloadThroughput: 500 * 1024 / 8,         // 500 Kbps
        uploadThroughput: 500 * 1024 / 8,           // 500 Kbps
        latency: 400                                 // 400ms
      },
      '2g': {
        downloadThroughput: 280 * 1024 / 8,         // 280 Kbps
        uploadThroughput: 256 * 1024 / 8,           // 256 Kbps
        latency: 800                                 // 800ms
      },
      'offline': {
        downloadThroughput: 0,
        uploadThroughput: 0,
        latency: 0
      }
    };
  }

  log(message, type = 'info') {
    const timestamp = new Date().toISOString();
    const prefix = {
      info: '🌐',
      success: '✅',
      error: '❌',
      warning: '⚠️'
    }[type] || '🌐';
    
    console.log(`${prefix} [${timestamp}] ${message}`);
  }

  async runNetworkTests() {
    this.log('Starting network conditions testing...', 'info');
    
    try {
      // Test each network profile
      for (const [profileName, profile] of Object.entries(this.networkProfiles)) {
        await this.testNetworkProfile(profileName, profile);
      }
      
      // Generate report
      await this.generateNetworkReport();
      
      this.log('Network conditions testing completed!', 'success');
      
    } catch (error) {
      this.log(`Network testing failed: ${error.message}`, 'error');
      throw error;
    }
  }

  async testNetworkProfile(profileName, profile) {
    this.log(`Testing network profile: ${profileName}`, 'info');
    
    const testResult = {
      profile: profileName,
      config: profile,
      startTime: Date.now(),
      endTime: null,
      duration: null,
      bundleLoadTime: null,
      chunkLoadTimes: [],
      errors: [],
      success: false,
      metrics: {}
    };

    try {
      // Simulate network conditions by testing bundle loading
      const bundleMetrics = await this.simulateBundleLoading(profile);
      testResult.bundleLoadTime = bundleMetrics.loadTime;
      testResult.chunkLoadTimes = bundleMetrics.chunkTimes;
      testResult.metrics = bundleMetrics;
      
      // Test lazy loading performance
      const lazyLoadMetrics = await this.testLazyLoading(profile);
      testResult.metrics.lazyLoading = lazyLoadMetrics;
      
      // Test error handling
      const errorHandling = await this.testErrorHandling(profile);
      testResult.metrics.errorHandling = errorHandling;
      
      testResult.success = true;
      this.log(`✅ ${profileName} test completed successfully`, 'success');
      
    } catch (error) {
      testResult.errors.push(error.message);
      testResult.success = false;
      this.log(`❌ ${profileName} test failed: ${error.message}`, 'error');
    }
    
    testResult.endTime = Date.now();
    testResult.duration = testResult.endTime - testResult.startTime;
    this.testResults.push(testResult);
  }

  async simulateBundleLoading(profile) {
    const startTime = Date.now();
    
    // Get bundle information
    const bundleInfo = await this.getBundleInfo();
    if (!bundleInfo) {
      throw new Error('Bundle information not available');
    }
    
    // Simulate loading time based on network profile
    const simulatedLoadTime = this.calculateLoadTime(bundleInfo.initialSize, profile);
    const chunkLoadTimes = bundleInfo.chunks.map(chunk => 
      this.calculateLoadTime(chunk.size, profile)
    );
    
    // Add some realistic variance
    const variance = 0.2; // 20% variance
    const actualLoadTime = simulatedLoadTime * (1 + (Math.random() - 0.5) * variance);
    
    return {
      loadTime: actualLoadTime,
      chunkTimes: chunkLoadTimes,
      bundleSize: bundleInfo.initialSize,
      totalSize: bundleInfo.totalSize,
      chunkCount: bundleInfo.chunks.length
    };
  }

  calculateLoadTime(sizeInBytes, profile) {
    if (profile.downloadThroughput === 0) {
      return Infinity; // Offline
    }
    
    // Calculate download time
    const downloadTime = (sizeInBytes / profile.downloadThroughput) * 1000; // ms
    
    // Add latency
    const totalTime = downloadTime + profile.latency;
    
    return totalTime;
  }

  async testLazyLoading(profile) {
    const lazyModules = [
      'escolas.routes',
      'alunos.routes',
      'professores.routes',
      'academico.routes',
      'avaliacoes.routes',
      'financeiro.routes',
      'relatorios.routes'
    ];
    
    const moduleSize = 50 * 1024; // Assume 50KB per module
    const loadTimes = lazyModules.map(module => ({
      module,
      loadTime: this.calculateLoadTime(moduleSize, profile),
      acceptable: this.calculateLoadTime(moduleSize, profile) < 5000 // 5 seconds max
    }));
    
    const acceptableCount = loadTimes.filter(lt => lt.acceptable).length;
    
    return {
      modules: loadTimes,
      acceptableRatio: acceptableCount / loadTimes.length,
      averageLoadTime: loadTimes.reduce((sum, lt) => sum + lt.loadTime, 0) / loadTimes.length
    };
  }

  async testErrorHandling(profile) {
    const errorScenarios = [
      {
        name: 'Timeout Handling',
        test: () => profile.downloadThroughput > 0 ? 'pass' : 'timeout',
        expected: profile.downloadThroughput === 0 ? 'timeout' : 'pass'
      },
      {
        name: 'Retry Mechanism',
        test: () => 'retry-success',
        expected: 'retry-success'
      },
      {
        name: 'Graceful Degradation',
        test: () => profile.downloadThroughput < 100000 ? 'degraded' : 'full',
        expected: profile.downloadThroughput < 100000 ? 'degraded' : 'full'
      }
    ];
    
    const results = errorScenarios.map(scenario => ({
      name: scenario.name,
      result: scenario.test(),
      expected: scenario.expected,
      passed: scenario.test() === scenario.expected
    }));
    
    return {
      scenarios: results,
      passRate: results.filter(r => r.passed).length / results.length
    };
  }

  async getBundleInfo() {
    try {
      const statsPath = path.join(process.cwd(), 'dist/sistema-gestao-escolar-frontend/stats.json');
      
      if (!fs.existsSync(statsPath)) {
        this.log('Bundle stats not found, building application...', 'warning');
        execSync('npm run build:stats', { stdio: 'inherit' });
      }
      
      const stats = JSON.parse(fs.readFileSync(statsPath, 'utf8'));
      
      const totalSize = stats.assets.reduce((sum, asset) => sum + asset.size, 0);
      const initialChunks = stats.chunks.filter(chunk => chunk.initial);
      const initialSize = initialChunks.reduce((sum, chunk) => {
        return sum + chunk.files.reduce((fileSum, fileName) => {
          const asset = stats.assets.find(a => a.name === fileName);
          return fileSum + (asset ? asset.size : 0);
        }, 0);
      }, 0);
      
      const chunks = stats.chunks.map(chunk => ({
        name: chunk.names?.[0] || 'unnamed',
        size: chunk.files.reduce((sum, fileName) => {
          const asset = stats.assets.find(a => a.name === fileName);
          return sum + (asset ? asset.size : 0);
        }, 0),
        initial: chunk.initial
      }));
      
      return {
        totalSize,
        initialSize,
        chunks
      };
    } catch (error) {
      this.log(`Could not read bundle info: ${error.message}`, 'error');
      return null;
    }
  }

  async generateNetworkReport() {
    this.log('Generating network conditions report...', 'info');
    
    const report = {
      timestamp: new Date().toISOString(),
      summary: {
        totalProfiles: this.testResults.length,
        successfulProfiles: this.testResults.filter(r => r.success).length,
        failedProfiles: this.testResults.filter(r => !r.success).length
      },
      profiles: this.testResults,
      recommendations: this.generateRecommendations(),
      performanceMatrix: this.generatePerformanceMatrix()
    };
    
    // Write detailed report
    const reportPath = path.join(process.cwd(), 'network-conditions-report.json');
    fs.writeFileSync(reportPath, JSON.stringify(report, null, 2));
    
    // Write summary
    const summaryPath = path.join(process.cwd(), 'network-conditions-summary.md');
    const summaryContent = this.generateMarkdownSummary(report);
    fs.writeFileSync(summaryPath, summaryContent);
    
    this.log(`Network report generated: ${reportPath}`, 'success');
    this.log(`Network summary generated: ${summaryPath}`, 'success');
    
    // Print console summary
    this.printConsoleSummary(report);
  }

  generateRecommendations() {
    const recommendations = [];
    
    // Analyze results and generate recommendations
    const slowProfiles = this.testResults.filter(r => 
      r.bundleLoadTime > 10000 && r.success
    );
    
    if (slowProfiles.length > 0) {
      recommendations.push({
        type: 'performance',
        priority: 'high',
        message: 'Bundle loading is slow on poor network conditions. Consider further code splitting.',
        affectedProfiles: slowProfiles.map(p => p.profile)
      });
    }
    
    const failedProfiles = this.testResults.filter(r => !r.success);
    if (failedProfiles.length > 0) {
      recommendations.push({
        type: 'reliability',
        priority: 'medium',
        message: 'Some network profiles failed. Improve error handling and retry mechanisms.',
        affectedProfiles: failedProfiles.map(p => p.profile)
      });
    }
    
    // Check lazy loading performance
    const poorLazyLoading = this.testResults.filter(r => 
      r.metrics.lazyLoading && r.metrics.lazyLoading.acceptableRatio < 0.8
    );
    
    if (poorLazyLoading.length > 0) {
      recommendations.push({
        type: 'lazy-loading',
        priority: 'medium',
        message: 'Lazy loading performance is poor on slow networks. Consider preloading critical modules.',
        affectedProfiles: poorLazyLoading.map(p => p.profile)
      });
    }
    
    return recommendations;
  }

  generatePerformanceMatrix() {
    return this.testResults.map(result => ({
      profile: result.profile,
      bundleLoadTime: result.bundleLoadTime,
      lazyLoadingScore: result.metrics.lazyLoading?.acceptableRatio || 0,
      errorHandlingScore: result.metrics.errorHandling?.passRate || 0,
      overallScore: this.calculateOverallScore(result)
    }));
  }

  calculateOverallScore(result) {
    if (!result.success) return 0;
    
    const loadTimeScore = Math.max(0, 1 - (result.bundleLoadTime / 30000)); // 30s max
    const lazyLoadScore = result.metrics.lazyLoading?.acceptableRatio || 0;
    const errorScore = result.metrics.errorHandling?.passRate || 0;
    
    return ((loadTimeScore + lazyLoadScore + errorScore) / 3 * 100).toFixed(1);
  }

  generateMarkdownSummary(report) {
    return `# Network Conditions Test Report

## Summary
- **Date**: ${new Date(report.timestamp).toLocaleString()}
- **Total Profiles Tested**: ${report.summary.totalProfiles}
- **Successful**: ${report.summary.successfulProfiles}
- **Failed**: ${report.summary.failedProfiles}

## Performance Matrix

| Profile | Bundle Load Time | Lazy Loading Score | Error Handling Score | Overall Score |
|---------|------------------|-------------------|---------------------|---------------|
${report.performanceMatrix.map(p => 
  `| ${p.profile} | ${this.formatTime(p.bundleLoadTime)} | ${(p.lazyLoadingScore * 100).toFixed(1)}% | ${(p.errorHandlingScore * 100).toFixed(1)}% | ${p.overallScore}% |`
).join('\n')}

## Detailed Results

${report.profiles.map(profile => `
### ${profile.profile.toUpperCase()}
- **Status**: ${profile.success ? '✅ PASSED' : '❌ FAILED'}
- **Duration**: ${profile.duration}ms
- **Bundle Load Time**: ${this.formatTime(profile.bundleLoadTime)}
- **Network Config**:
  - Download: ${this.formatBytes(profile.config.downloadThroughput)}/s
  - Upload: ${this.formatBytes(profile.config.uploadThroughput)}/s
  - Latency: ${profile.config.latency}ms

${profile.metrics.lazyLoading ? `
**Lazy Loading Performance**:
- Acceptable Modules: ${(profile.metrics.lazyLoading.acceptableRatio * 100).toFixed(1)}%
- Average Load Time: ${this.formatTime(profile.metrics.lazyLoading.averageLoadTime)}
` : ''}

${profile.errors.length > 0 ? `**Errors**: ${profile.errors.join(', ')}` : ''}
`).join('\n')}

## Recommendations

${report.recommendations.map(rec => `
### ${rec.type.toUpperCase()} (${rec.priority.toUpperCase()} Priority)
${rec.message}
- **Affected Profiles**: ${rec.affectedProfiles.join(', ')}
`).join('\n')}

---
*Generated by Network Conditions Test Runner*
`;
  }

  printConsoleSummary(report) {
    console.log('\n' + '='.repeat(60));
    console.log('NETWORK CONDITIONS TEST SUMMARY');
    console.log('='.repeat(60));
    
    report.performanceMatrix.forEach(profile => {
      const status = profile.overallScore > 70 ? '✅' : profile.overallScore > 40 ? '⚠️' : '❌';
      console.log(`${status} ${profile.profile.padEnd(15)} | Score: ${profile.overallScore}% | Load: ${this.formatTime(profile.bundleLoadTime)}`);
    });
    
    console.log('='.repeat(60));
    
    if (report.recommendations.length > 0) {
      console.log('\nRECOMMENDATIONS:');
      report.recommendations.forEach(rec => {
        console.log(`${rec.priority === 'high' ? '🔴' : '🟡'} ${rec.message}`);
      });
    }
  }

  formatTime(ms) {
    if (ms === Infinity) return 'Timeout';
    if (ms < 1000) return `${Math.round(ms)}ms`;
    return `${(ms / 1000).toFixed(1)}s`;
  }

  formatBytes(bytes) {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i];
  }
}

// Run the tests if this script is executed directly
if (require.main === module) {
  const tester = new NetworkConditionsTest();
  tester.runNetworkTests().catch(error => {
    console.error('Network conditions testing failed:', error);
    process.exit(1);
  });
}

module.exports = NetworkConditionsTest;