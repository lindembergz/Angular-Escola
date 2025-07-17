import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { LazyLoadingService } from '../services/lazy-loading.service';

@Injectable()
export class LazyLoadingInterceptor implements HttpInterceptor {
  private activeRequests = 0;

  constructor(private lazyLoadingService: LazyLoadingService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Only handle requests that might be related to lazy loading
    if (this.isLazyLoadingRequest(req)) {
      this.activeRequests++;
      this.updateLoadingState();
    }

    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (this.isLazyLoadingRequest(req)) {
          this.handleLazyLoadingError(error);
        }
        return throwError(() => error);
      }),
      finalize(() => {
        if (this.isLazyLoadingRequest(req)) {
          this.activeRequests--;
          this.updateLoadingState();
        }
      })
    );
  }

  private isLazyLoadingRequest(req: HttpRequest<any>): boolean {
    // Check if the request is for JavaScript chunks or modules
    return req.url.includes('.js') || 
           req.url.includes('chunk') || 
           req.url.includes('module') ||
           req.headers.has('X-Lazy-Loading');
  }

  private updateLoadingState(): void {
    // Update loading state based on active requests
    if (this.activeRequests > 0) {
      // Set loading state if not already set
    } else {
      // Clear loading state when no active requests
      this.lazyLoadingService.clearState();
    }
  }

  private handleLazyLoadingError(error: HttpErrorResponse): void {
    console.error('Lazy loading HTTP error:', error);
    
    if (error.status === 0 || error.status >= 500) {
      // Network or server errors
      console.warn('Network error during lazy loading, might need retry');
    } else if (error.status === 404) {
      // Resource not found - might be a chunk loading issue
      console.warn('Resource not found during lazy loading:', error.url);
    }
  }
}