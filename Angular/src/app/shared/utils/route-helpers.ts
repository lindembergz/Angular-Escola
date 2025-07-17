import { Type } from '@angular/core';

/**
 * Helper function to create lazy loaded routes with error handling
 */
export function createLazyRoute<T>(
  importFn: () => Promise<{ [key: string]: Type<T> }>,
  componentKey: string,
  moduleName: string
): () => Promise<Type<T>> {
  return () => importFn()
    .then(m => {
      if (m[componentKey]) {
        return m[componentKey];
      }
      throw new Error(`Component ${componentKey} not found in ${moduleName} module`);
    })
    .catch(error => {
      console.error(`Error loading ${moduleName} component:`, error);
      return import('../components/lazy-loading-error/lazy-loading-error.component')
        .then(errorModule => errorModule.LazyLoadingErrorComponent as Type<T>);
    });
}

/**
 * Helper function to create lazy loaded route children with error handling
 */
export function createLazyChildren(
  importFn: () => Promise<{ [key: string]: any }>,
  routesKey: string,
  moduleName: string
): () => Promise<any> {
  return () => importFn()
    .then(m => m[routesKey])
    .catch(error => {
      console.error(`Error loading ${moduleName} routes:`, error);
      // Return a fallback route that shows error component
      return [
        {
          path: '',
          loadComponent: () => import('../components/lazy-loading-error/lazy-loading-error.component')
            .then(m => m.LazyLoadingErrorComponent)
        },
        {
          path: '**',
          redirectTo: ''
        }
      ];
    });
}

/**
 * Retry mechanism for failed lazy loading
 */
export function retryLazyLoad<T>(
  importFn: () => Promise<T>,
  maxRetries: number = 3,
  delay: number = 1000
): Promise<T> {
  return new Promise((resolve, reject) => {
    let attempts = 0;

    const attemptLoad = () => {
      attempts++;
      importFn()
        .then(resolve)
        .catch(error => {
          if (attempts < maxRetries) {
            console.warn(`Lazy load attempt ${attempts} failed, retrying in ${delay}ms...`);
            setTimeout(attemptLoad, delay * attempts); // Exponential backoff
          } else {
            console.error(`Lazy load failed after ${maxRetries} attempts:`, error);
            reject(error);
          }
        });
    };

    attemptLoad();
  });
}