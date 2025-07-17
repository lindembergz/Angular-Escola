import { Injectable, ErrorHandler } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class ChunkErrorHandlerService implements ErrorHandler {
  private retryAttempts = new Map<string, number>();
  private readonly MAX_RETRIES = 3;

  constructor(private router: Router) {}

  handleError(error: any): void {
    console.error('Global error handler:', error);

    if (this.isChunkLoadError(error)) {
      this.handleChunkLoadError(error);
    } else {
      // Handle other types of errors
      console.error('Non-chunk error:', error);
    }
  }

  private isChunkLoadError(error: any): boolean {
    return (
      error?.name === 'ChunkLoadError' ||
      error?.message?.includes('Loading chunk') ||
      error?.message?.includes('Loading CSS chunk') ||
      error?.message?.includes('Failed to import')
    );
  }

  private handleChunkLoadError(error: any): void {
    const currentUrl = this.router.url;
    const retryKey = `${currentUrl}_${error.message}`;
    const currentRetries = this.retryAttempts.get(retryKey) || 0;

    if (currentRetries < this.MAX_RETRIES) {
      // Increment retry count
      this.retryAttempts.set(retryKey, currentRetries + 1);
      
      // Show user-friendly message
      console.warn(`Chunk loading failed, retrying... (${currentRetries + 1}/${this.MAX_RETRIES})`);
      
      // Attempt to reload the current route after a delay
      setTimeout(() => {
        window.location.reload();
      }, 1000 * (currentRetries + 1)); // Exponential backoff
      
    } else {
      // Max retries reached, redirect to dashboard with error message
      console.error('Max retries reached for chunk loading');
      this.retryAttempts.delete(retryKey);
      
      this.router.navigate(['/'], {
        queryParams: {
          error: 'chunk_load_failed',
          message: 'Erro ao carregar recursos. Tente atualizar a página.'
        }
      });
    }
  }

  /**
   * Clear retry attempts for a specific URL
   */
  clearRetryAttempts(url?: string): void {
    if (url) {
      const keysToDelete = Array.from(this.retryAttempts.keys())
        .filter(key => key.startsWith(url));
      keysToDelete.forEach(key => this.retryAttempts.delete(key));
    } else {
      this.retryAttempts.clear();
    }
  }

  /**
   * Get current retry count for a URL
   */
  getRetryCount(url: string): number {
    const keys = Array.from(this.retryAttempts.keys())
      .filter(key => key.startsWith(url));
    return keys.length > 0 ? Math.max(...keys.map(key => this.retryAttempts.get(key) || 0)) : 0;
  }
}