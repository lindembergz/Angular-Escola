import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';

export interface LazyLoadingState {
  isLoading: boolean;
  error?: string;
  retryCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class LazyLoadingService {
  private loadingState = new BehaviorSubject<LazyLoadingState>({
    isLoading: false,
    error: undefined,
    retryCount: 0
  });

  public loadingState$ = this.loadingState.asObservable();

  constructor(private router: Router) {}

  /**
   * Handles lazy loading with retry logic and error handling
   */
  loadComponent<T>(
    importFn: () => Promise<T>,
    maxRetries: number = 3,
    retryDelay: number = 1000
  ): Promise<T> {
    this.setLoadingState({ isLoading: true, error: undefined, retryCount: 0 });

    return this.executeWithRetry(importFn, maxRetries, retryDelay)
      .then(result => {
        this.setLoadingState({ isLoading: false, error: undefined, retryCount: 0 });
        return result;
      })
      .catch(error => {
        const errorMessage = this.getErrorMessage(error);
        this.setLoadingState({ 
          isLoading: false, 
          error: errorMessage, 
          retryCount: this.loadingState.value.retryCount 
        });
        throw error;
      });
  }

  /**
   * Retry loading a component
   */
  retryLoading<T>(importFn: () => Promise<T>): Promise<T> {
    const currentState = this.loadingState.value;
    return this.loadComponent(importFn, 3, 1000);
  }

  /**
   * Navigate to home page
   */
  goToHome(): void {
    this.router.navigate(['/']);
  }

  /**
   * Clear loading state
   */
  clearState(): void {
    this.setLoadingState({ isLoading: false, error: undefined, retryCount: 0 });
  }

  private async executeWithRetry<T>(
    importFn: () => Promise<T>,
    maxRetries: number,
    retryDelay: number
  ): Promise<T> {
    let lastError: any;
    
    for (let attempt = 0; attempt <= maxRetries; attempt++) {
      try {
        if (attempt > 0) {
          this.setLoadingState({ 
            isLoading: true, 
            error: undefined, 
            retryCount: attempt 
          });
          await this.delay(retryDelay * attempt); // Exponential backoff
        }
        
        return await importFn();
      } catch (error) {
        lastError = error;
        console.warn(`Lazy loading attempt ${attempt + 1} failed:`, error);
        
        if (attempt === maxRetries) {
          break;
        }
      }
    }
    
    throw lastError;
  }

  private delay(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  private getErrorMessage(error: any): string {
    if (error?.name === 'ChunkLoadError') {
      return 'Erro ao carregar recursos da aplicação. Verifique sua conexão com a internet.';
    }
    
    if (error?.message?.includes('Loading chunk')) {
      return 'Erro ao carregar módulo. Tente atualizar a página.';
    }
    
    if (error?.message?.includes('Network')) {
      return 'Erro de conexão. Verifique sua internet e tente novamente.';
    }
    
    return 'Erro inesperado ao carregar módulo. Tente novamente.';
  }

  private setLoadingState(state: LazyLoadingState): void {
    this.loadingState.next(state);
  }
}