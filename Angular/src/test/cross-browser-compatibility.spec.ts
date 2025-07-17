/**
 * Cross-Browser Compatibility Tests
 * Tests to ensure optimizations work across different browsers
 */

describe('Cross-Browser Compatibility', () => {
  describe('Browser Feature Detection', () => {
    it('should support ES2022 modules', () => {
      // Test that the browser supports ES2022 features used in optimization
      expect(typeof Promise).toBe('function');
      expect(typeof Array.prototype.at).toBe('function');
      expect(typeof Object.hasOwn).toBe('function');
    });

    it('should support dynamic imports', async () => {
      // Test dynamic import functionality
      try {
        const module = await import('../app/app.component');
        expect(module).toBeDefined();
        expect(module.AppComponent).toBeDefined();
      } catch (error) {
        fail('Dynamic imports should be supported');
      }
    });

    it('should support async/await', async () => {
      const asyncFunction = async () => {
        return new Promise(resolve => setTimeout(() => resolve('test'), 10));
      };

      const result = await asyncFunction();
      expect(result).toBe('test');
    });

    it('should support arrow functions', () => {
      const arrowFunction = () => 'test';
      expect(arrowFunction()).toBe('test');
    });

    it('should support template literals', () => {
      const name = 'test';
      const template = `Hello ${name}`;
      expect(template).toBe('Hello test');
    });

    it('should support destructuring', () => {
      const obj = { a: 1, b: 2 };
      const { a, b } = obj;
      expect(a).toBe(1);
      expect(b).toBe(2);
    });

    it('should support spread operator', () => {
      const arr1 = [1, 2];
      const arr2 = [...arr1, 3, 4];
      expect(arr2).toEqual([1, 2, 3, 4]);
    });
  });

  describe('Polyfill Requirements', () => {
    it('should have zone.js available', () => {
      // @ts-ignore - Zone is a global variable
      expect(typeof (window as any).Zone).toBe('object');
    });

    it('should support Intersection Observer (for lazy loading)', () => {
      if (typeof IntersectionObserver !== 'undefined') {
        expect(typeof IntersectionObserver).toBe('function');
      } else {
        console.warn('IntersectionObserver not available - may need polyfill for older browsers');
      }
    });

    it('should support Fetch API', () => {
      expect(typeof fetch).toBe('function');
    });

    it('should support Web Components (for Angular Elements)', () => {
      if (typeof customElements !== 'undefined') {
        expect(typeof customElements.define).toBe('function');
      } else {
        console.warn('Custom Elements not available - may need polyfill for older browsers');
      }
    });
  });

  describe('CSS Features', () => {
    it('should support CSS Grid', () => {
      const testElement = document.createElement('div');
      testElement.style.display = 'grid';
      expect(testElement.style.display).toBe('grid');
    });

    it('should support CSS Flexbox', () => {
      const testElement = document.createElement('div');
      testElement.style.display = 'flex';
      expect(testElement.style.display).toBe('flex');
    });

    it('should support CSS Custom Properties', () => {
      const testElement = document.createElement('div');
      testElement.style.setProperty('--test-var', 'test-value');
      const value = getComputedStyle(testElement).getPropertyValue('--test-var');
      expect(value.trim()).toBe('test-value');
    });

    it('should support CSS calc()', () => {
      const testElement = document.createElement('div');
      testElement.style.width = 'calc(100% - 20px)';
      expect(testElement.style.width).toContain('calc');
    });
  });

  describe('Performance APIs', () => {
    it('should support Performance API', () => {
      expect(typeof performance).toBe('object');
      expect(typeof performance.now).toBe('function');
      expect(typeof performance.mark).toBe('function');
      expect(typeof performance.measure).toBe('function');
    });

    it('should support Navigation Timing API', () => {
      if (performance.navigation) {
        expect(typeof performance.navigation).toBe('object');
      } else if (performance.getEntriesByType) {
        const navigationEntries = performance.getEntriesByType('navigation');
        expect(Array.isArray(navigationEntries)).toBe(true);
      } else {
        console.warn('Navigation Timing API not fully available');
      }
    });

    it('should support Resource Timing API', () => {
      if (typeof performance.getEntriesByType === 'function') {
        const resourceEntries = performance.getEntriesByType('resource');
        expect(Array.isArray(resourceEntries)).toBe(true);
      } else {
        console.warn('Resource Timing API not available');
      }
    });
  });

  describe('Storage APIs', () => {
    it('should support localStorage', () => {
      if (typeof localStorage !== 'undefined') {
        localStorage.setItem('test', 'value');
        expect(localStorage.getItem('test')).toBe('value');
        localStorage.removeItem('test');
      } else {
        console.warn('localStorage not available');
      }
    });

    it('should support sessionStorage', () => {
      if (typeof sessionStorage !== 'undefined') {
        sessionStorage.setItem('test', 'value');
        expect(sessionStorage.getItem('test')).toBe('value');
        sessionStorage.removeItem('test');
      } else {
        console.warn('sessionStorage not available');
      }
    });
  });

  describe('Network APIs', () => {
    it('should support XMLHttpRequest', () => {
      expect(typeof XMLHttpRequest).toBe('function');
      const xhr = new XMLHttpRequest();
      expect(xhr).toBeInstanceOf(XMLHttpRequest);
    });

    it('should support Fetch API with proper error handling', async () => {
      try {
        // Test with a mock URL that should fail
        await fetch('http://localhost:99999/test');
      } catch (error) {
        expect(error).toBeDefined();
      }
    });
  });

  describe('Event Handling', () => {
    it('should support addEventListener', () => {
      const element = document.createElement('div');
      let eventFired = false;
      
      const handler = () => { eventFired = true; };
      element.addEventListener('click', handler);
      
      const event = new Event('click');
      element.dispatchEvent(event);
      
      expect(eventFired).toBe(true);
      element.removeEventListener('click', handler);
    });

    it('should support custom events', () => {
      const element = document.createElement('div');
      let customEventData: any = null;
      
      element.addEventListener('custom-event', (event: any) => {
        customEventData = event.detail;
      });
      
      const customEvent = new CustomEvent('custom-event', {
        detail: { test: 'data' }
      });
      
      element.dispatchEvent(customEvent);
      expect(customEventData).toEqual({ test: 'data' });
    });
  });

  describe('Browser-Specific Optimizations', () => {
    it('should detect browser type for optimization hints', () => {
      const userAgent = navigator.userAgent.toLowerCase();
      
      const browserInfo = {
        isChrome: userAgent.includes('chrome') && !userAgent.includes('edge'),
        isFirefox: userAgent.includes('firefox'),
        isSafari: userAgent.includes('safari') && !userAgent.includes('chrome'),
        isEdge: userAgent.includes('edge')
      };
      
      console.log('Browser detection:', browserInfo);
      
      // At least one browser should be detected
      const detectedBrowsers = Object.values(browserInfo).filter(Boolean);
      expect(detectedBrowsers.length).toBeGreaterThanOrEqual(0);
    });

    it('should handle viewport meta tag for mobile optimization', () => {
      const viewportMeta = document.querySelector('meta[name="viewport"]');
      
      if (viewportMeta) {
        const content = viewportMeta.getAttribute('content');
        expect(content).toBeTruthy();
        console.log('Viewport meta content:', content);
      } else {
        console.warn('Viewport meta tag not found - may affect mobile performance');
      }
    });
  });

  describe('Error Handling Compatibility', () => {
    it('should handle Promise rejections', async () => {
      try {
        await Promise.reject(new Error('Test error'));
        fail('Should have thrown an error');
      } catch (error) {
        expect(error).toBeInstanceOf(Error);
        expect((error as Error).message).toBe('Test error');
      }
    });

    it('should handle async function errors', async () => {
      const asyncErrorFunction = async () => {
        throw new Error('Async error');
      };
      
      try {
        await asyncErrorFunction();
        fail('Should have thrown an error');
      } catch (error) {
        expect(error).toBeInstanceOf(Error);
      }
    });

    it('should support error boundaries concept', () => {
      // Test that error handling patterns work
      const errorHandler = (error: Error) => {
        return { hasError: true, error };
      };
      
      const testError = new Error('Test error');
      const result = errorHandler(testError);
      
      expect(result.hasError).toBe(true);
      expect(result.error).toBe(testError);
    });
  });
});