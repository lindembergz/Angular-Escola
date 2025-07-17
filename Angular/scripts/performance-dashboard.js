#!/usr/bin/env node

/**
 * Performance Monitoring Dashboard
 * Generates HTML dashboard for ongoing bundle performance tracking
 */

const fs = require('fs');
const path = require('path');

const DASHBOARD_FILE = 'performance-dashboard.html';
const METRICS_FILE = 'build-metrics.json';
const PERFORMANCE_HISTORY_FILE = 'performance-history.json';

function generateDashboard() {
  console.log('📊 Generating Performance Dashboard...');
  
  // Read metrics data
  const buildMetrics = readMetricsFile(METRICS_FILE);
  const performanceHistory = readMetricsFile(PERFORMANCE_HISTORY_FILE);
  
  // Generate HTML dashboard
  const html = generateDashboardHTML(buildMetrics, performanceHistory);
  
  // Write dashboard file
  fs.writeFileSync(DASHBOARD_FILE, html);
  
  console.log(`✅ Performance dashboard generated: ${DASHBOARD_FILE}`);
  console.log(`🌐 Open in browser: file://${path.resolve(DASHBOARD_FILE)}`);
}

function readMetricsFile(filename) {
  const filePath = path.join(process.cwd(), filename);
  
  if (!fs.existsSync(filePath)) {
    console.warn(`⚠️ ${filename} not found, using empty data`);
    return [];
  }
  
  try {
    return JSON.parse(fs.readFileSync(filePath, 'utf8'));
  } catch (error) {
    console.warn(`⚠️ Could not read ${filename}:`, error.message);
    return [];
  }
}

function generateDashboardHTML(buildMetrics, performanceHistory) {
  const latestBuild = buildMetrics.length > 0 ? buildMetrics[buildMetrics.length - 1] : null;
  const chartData = prepareChartData(buildMetrics, performanceHistory);
  
  return `<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Bundle Performance Dashboard</title>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: #f5f7fa;
            color: #2d3748;
            line-height: 1.6;
        }
        
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }
        
        .header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            border-radius: 12px;
            margin-bottom: 30px;
            text-align: center;
        }
        
        .header h1 {
            font-size: 2.5rem;
            margin-bottom: 10px;
        }
        
        .header p {
            font-size: 1.1rem;
            opacity: 0.9;
        }
        
        .metrics-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }
        
        .metric-card {
            background: white;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            border-left: 4px solid #667eea;
        }
        
        .metric-card h3 {
            font-size: 0.9rem;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            color: #718096;
            margin-bottom: 10px;
        }
        
        .metric-value {
            font-size: 2rem;
            font-weight: bold;
            color: #2d3748;
            margin-bottom: 5px;
        }
        
        .metric-change {
            font-size: 0.9rem;
            font-weight: 500;
        }
        
        .metric-change.positive {
            color: #38a169;
        }
        
        .metric-change.negative {
            color: #e53e3e;
        }
        
        .metric-change.neutral {
            color: #718096;
        }
        
        .chart-container {
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            margin-bottom: 30px;
        }
        
        .chart-container h2 {
            margin-bottom: 20px;
            color: #2d3748;
        }
        
        .status-indicator {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        
        .status-success {
            background: #c6f6d5;
            color: #22543d;
        }
        
        .status-warning {
            background: #faf089;
            color: #744210;
        }
        
        .status-error {
            background: #fed7d7;
            color: #742a2a;
        }
        
        .recommendations {
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }
        
        .recommendations h2 {
            margin-bottom: 20px;
            color: #2d3748;
        }
        
        .recommendation-item {
            padding: 15px;
            margin-bottom: 10px;
            background: #f7fafc;
            border-left: 4px solid #4299e1;
            border-radius: 4px;
        }
        
        .footer {
            text-align: center;
            margin-top: 40px;
            padding: 20px;
            color: #718096;
            font-size: 0.9rem;
        }
        
        @media (max-width: 768px) {
            .container {
                padding: 10px;
            }
            
            .header h1 {
                font-size: 2rem;
            }
            
            .metrics-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>📊 Bundle Performance Dashboard</h1>
            <p>Sistema de Gestão Escolar - Frontend Bundle Monitoring</p>
            <p>Last updated: ${new Date().toLocaleString()}</p>
        </div>
        
        ${generateMetricsSection(latestBuild, buildMetrics)}
        
        ${generateChartsSection(chartData)}
        
        ${generateRecommendationsSection(latestBuild)}
        
        <div class="footer">
            <p>Generated automatically by Bundle Performance Monitor</p>
            <p>Refresh this page after new builds to see updated metrics</p>
        </div>
    </div>
    
    <script>
        ${generateChartScripts(chartData)}
    </script>
</body>
</html>`;
}

function generateMetricsSection(latestBuild, buildMetrics) {
  if (!latestBuild) {
    return `
        <div class="metrics-grid">
            <div class="metric-card">
                <h3>No Data Available</h3>
                <div class="metric-value">-</div>
                <div class="metric-change neutral">Run a build to see metrics</div>
            </div>
        </div>
    `;
  }
  
  const previousBuild = buildMetrics.length > 1 ? buildMetrics[buildMetrics.length - 2] : null;
  
  return `
        <div class="metrics-grid">
            <div class="metric-card">
                <h3>Initial Bundle Size</h3>
                <div class="metric-value">${formatBytes(latestBuild.initialBundleSize)}</div>
                <div class="metric-change ${getBudgetStatus(latestBuild.budgetStatus.withinBudget)}">
                    ${latestBuild.budgetStatus.withinBudget ? '✅ Within Budget' : '❌ Over Budget'}
                    ${latestBuild.budgetStatus.overage > 0 ? ` (+${formatBytes(latestBuild.budgetStatus.overage)})` : ''}
                </div>
            </div>
            
            <div class="metric-card">
                <h3>Total Bundle Size</h3>
                <div class="metric-value">${formatBytes(latestBuild.bundleSize)}</div>
                <div class="metric-change ${getChangeClass(latestBuild.bundleSize, previousBuild?.bundleSize)}">
                    ${getChangeText(latestBuild.bundleSize, previousBuild?.bundleSize)}
                </div>
            </div>
            
            <div class="metric-card">
                <h3>Performance Score</h3>
                <div class="metric-value">${latestBuild.performanceScore}/100</div>
                <div class="metric-change ${getScoreClass(latestBuild.performanceScore)}">
                    ${getScoreText(latestBuild.performanceScore)}
                </div>
            </div>
            
            <div class="metric-card">
                <h3>Lazy Loading</h3>
                <div class="metric-value">${Math.round((latestBuild.lazyChunkCount / latestBuild.chunkCount) * 100)}%</div>
                <div class="metric-change neutral">
                    ${latestBuild.lazyChunkCount}/${latestBuild.chunkCount} chunks lazy
                </div>
            </div>
        </div>
    `;
}

function generateChartsSection(chartData) {
  return `
        <div class="chart-container">
            <h2>Bundle Size Trend</h2>
            <canvas id="bundleSizeChart" width="400" height="200"></canvas>
        </div>
        
        <div class="chart-container">
            <h2>Performance Score History</h2>
            <canvas id="performanceChart" width="400" height="200"></canvas>
        </div>
    `;
}

function generateRecommendationsSection(latestBuild) {
  if (!latestBuild) {
    return '';
  }
  
  const recommendations = generateRecommendations(latestBuild);
  
  if (recommendations.length === 0) {
    return `
        <div class="recommendations">
            <h2>🎉 Optimization Status</h2>
            <div class="recommendation-item">
                <strong>Excellent!</strong> Your bundle is well optimized. No immediate recommendations.
            </div>
        </div>
    `;
  }
  
  return `
        <div class="recommendations">
            <h2>💡 Optimization Recommendations</h2>
            ${recommendations.map(rec => `
                <div class="recommendation-item">
                    ${rec}
                </div>
            `).join('')}
        </div>
    `;
}

function generateRecommendations(build) {
  const recommendations = [];
  
  if (!build.budgetStatus.withinBudget) {
    recommendations.push('🚨 <strong>Critical:</strong> Initial bundle exceeds 1MB budget. Implement aggressive lazy loading.');
  }
  
  if (build.performanceScore < 70) {
    recommendations.push('⚠️ <strong>Performance:</strong> Score below 70. Consider code splitting and tree shaking optimizations.');
  }
  
  const lazyRatio = build.lazyChunkCount / build.chunkCount;
  if (lazyRatio < 0.6) {
    recommendations.push('📦 <strong>Lazy Loading:</strong> Only ' + Math.round(lazyRatio * 100) + '% of chunks are lazy. Increase lazy loading coverage.');
  }
  
  if (build.initialBundleSize > 900 * 1024) {
    recommendations.push('🔧 <strong>Bundle Size:</strong> Approaching budget limit. Review and optimize large dependencies.');
  }
  
  return recommendations;
}

function prepareChartData(buildMetrics, performanceHistory) {
  const last20Builds = buildMetrics.slice(-20);
  
  return {
    labels: last20Builds.map((build, index) => `Build ${index + 1}`),
    bundleSizes: last20Builds.map(build => Math.round(build.initialBundleSize / 1024)), // KB
    totalSizes: last20Builds.map(build => Math.round(build.bundleSize / 1024)), // KB
    performanceScores: last20Builds.map(build => build.performanceScore || 0)
  };
}

function generateChartScripts(chartData) {
  return `
        // Bundle Size Chart
        const bundleCtx = document.getElementById('bundleSizeChart').getContext('2d');
        new Chart(bundleCtx, {
            type: 'line',
            data: {
                labels: ${JSON.stringify(chartData.labels)},
                datasets: [{
                    label: 'Initial Bundle (KB)',
                    data: ${JSON.stringify(chartData.bundleSizes)},
                    borderColor: '#667eea',
                    backgroundColor: 'rgba(102, 126, 234, 0.1)',
                    tension: 0.4,
                    fill: true
                }, {
                    label: 'Total Bundle (KB)',
                    data: ${JSON.stringify(chartData.totalSizes)},
                    borderColor: '#764ba2',
                    backgroundColor: 'rgba(118, 75, 162, 0.1)',
                    tension: 0.4,
                    fill: false
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'top',
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Size (KB)'
                        }
                    }
                }
            }
        });
        
        // Performance Score Chart
        const perfCtx = document.getElementById('performanceChart').getContext('2d');
        new Chart(perfCtx, {
            type: 'line',
            data: {
                labels: ${JSON.stringify(chartData.labels)},
                datasets: [{
                    label: 'Performance Score',
                    data: ${JSON.stringify(chartData.performanceScores)},
                    borderColor: '#38a169',
                    backgroundColor: 'rgba(56, 161, 105, 0.1)',
                    tension: 0.4,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'top',
                    }
                },
                scales: {
                    y: {
                        min: 0,
                        max: 100,
                        title: {
                            display: true,
                            text: 'Score'
                        }
                    }
                }
            }
        });
    `;
}

// Helper functions
function formatBytes(bytes) {
  if (bytes === 0) return '0 B';
  const k = 1024;
  const sizes = ['B', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i];
}

function getBudgetStatus(withinBudget) {
  return withinBudget ? 'positive' : 'negative';
}

function getChangeClass(current, previous) {
  if (!previous) return 'neutral';
  if (current < previous) return 'positive';
  if (current > previous) return 'negative';
  return 'neutral';
}

function getChangeText(current, previous) {
  if (!previous) return 'No previous data';
  
  const change = current - previous;
  const changePercent = ((change / previous) * 100).toFixed(1);
  
  if (change === 0) return 'No change';
  
  const direction = change > 0 ? '↗️' : '↘️';
  const sign = change > 0 ? '+' : '';
  
  return `${direction} ${sign}${formatBytes(Math.abs(change))} (${sign}${changePercent}%)`;
}

function getScoreClass(score) {
  if (score >= 80) return 'positive';
  if (score >= 60) return 'neutral';
  return 'negative';
}

function getScoreText(score) {
  if (score >= 90) return '🎉 Excellent';
  if (score >= 80) return '✅ Good';
  if (score >= 70) return '⚠️ Fair';
  if (score >= 60) return '🔶 Poor';
  return '❌ Critical';
}

// Main execution
if (require.main === module) {
  generateDashboard();
}

module.exports = {
  generateDashboard,
  prepareChartData,
  generateRecommendations
};