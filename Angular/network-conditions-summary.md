# Network Conditions Test Report

## Summary
- **Date**: 17/07/2025, 16:50:25
- **Total Profiles Tested**: 4
- **Successful**: 0
- **Failed**: 4

## Performance Matrix

| Profile | Bundle Load Time | Lazy Loading Score | Error Handling Score | Overall Score |
|---------|------------------|-------------------|---------------------|---------------|
| fast-3g | 0ms | 0.0% | 0.0% | 0% |
| slow-3g | 0ms | 0.0% | 0.0% | 0% |
| 2g | 0ms | 0.0% | 0.0% | 0% |
| offline | 0ms | 0.0% | 0.0% | 0% |

## Detailed Results


### FAST-3G
- **Status**: ❌ FAILED
- **Duration**: 6ms
- **Bundle Load Time**: 0ms
- **Network Config**:
  - Download: 204.8 KB/s
  - Upload: 93.8 KB/s
  - Latency: 150ms



**Errors**: Bundle information not available


### SLOW-3G
- **Status**: ❌ FAILED
- **Duration**: 5ms
- **Bundle Load Time**: 0ms
- **Network Config**:
  - Download: 62.5 KB/s
  - Upload: 62.5 KB/s
  - Latency: 400ms



**Errors**: Bundle information not available


### 2G
- **Status**: ❌ FAILED
- **Duration**: 6ms
- **Bundle Load Time**: 0ms
- **Network Config**:
  - Download: 35 KB/s
  - Upload: 32 KB/s
  - Latency: 800ms



**Errors**: Bundle information not available


### OFFLINE
- **Status**: ❌ FAILED
- **Duration**: 5ms
- **Bundle Load Time**: 0ms
- **Network Config**:
  - Download: 0 B/s
  - Upload: 0 B/s
  - Latency: 0ms



**Errors**: Bundle information not available


## Recommendations


### RELIABILITY (MEDIUM Priority)
Some network profiles failed. Improve error handling and retry mechanisms.
- **Affected Profiles**: fast-3g, slow-3g, 2g, offline


---
*Generated by Network Conditions Test Runner*
