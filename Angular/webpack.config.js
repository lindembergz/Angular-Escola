const path = require('path');

module.exports = {
  optimization: {
    splitChunks: {
      chunks: 'all',
      cacheGroups: {
        // Vendor chunk for third-party libraries
        vendor: {
          test: /[\\/]node_modules[\\/]/,
          name: 'vendors',
          chunks: 'all',
          priority: 20,
          reuseExistingChunk: true,
          enforce: true
        },
        // PrimeNG specific chunk
        primeng: {
          test: /[\\/]node_modules[\\/]primeng[\\/]/,
          name: 'primeng',
          chunks: 'all',
          priority: 30,
          reuseExistingChunk: true,
          enforce: true
        },
        // Angular core libraries chunk
        angular: {
          test: /[\\/]node_modules[\\/]@angular[\\/]/,
          name: 'angular',
          chunks: 'all',
          priority: 25,
          reuseExistingChunk: true,
          enforce: true
        },
        // NgRx chunk
        ngrx: {
          test: /[\\/]node_modules[\\/]@ngrx[\\/]/,
          name: 'ngrx',
          chunks: 'all',
          priority: 25,
          reuseExistingChunk: true,
          enforce: true
        },
        // Common chunk for shared application code
        common: {
          name: 'common',
          minChunks: 2,
          chunks: 'all',
          priority: 10,
          reuseExistingChunk: true,
          enforce: true
        },
        // Default chunk
        default: {
          minChunks: 2,
          priority: 5,
          reuseExistingChunk: true
        }
      }
    },
    // Enable tree shaking and dead code elimination
    usedExports: true,
    sideEffects: false,
    // Minimize bundle size
    minimize: true,
    // Remove empty chunks
    removeEmptyChunks: true,
    // Merge duplicate chunks
    mergeDuplicateChunks: true,
    // Remove available modules
    removeAvailableModules: true,
    // Flag chunks as entry points
    flagIncludedChunks: true,
    // Optimize module ids
    moduleIds: 'deterministic',
    // Optimize chunk ids
    chunkIds: 'deterministic'
  },
  resolve: {
    // Optimize module resolution
    extensions: ['.ts', '.js', '.json'],
    modules: ['node_modules'],
    // Enable tree shaking for ES modules
    mainFields: ['es2015', 'browser', 'module', 'main'],
    // Alias for better tree shaking
    alias: {
      '@': path.resolve(__dirname, 'src'),
      '@shared': path.resolve(__dirname, 'src/app/shared'),
      '@features': path.resolve(__dirname, 'src/app/features'),
      '@core': path.resolve(__dirname, 'src/app/core')
    }
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        use: [
          {
            loader: '@angular-devkit/build-angular/src/babel/webpack-loader',
            options: {
              aot: true,
              optimize: true
            }
          }
        ]
      }
    ]
  }
};