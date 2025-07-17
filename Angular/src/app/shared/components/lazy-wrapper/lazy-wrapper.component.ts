import { Component, Input, OnInit, OnDestroy, ViewContainerRef, ComponentRef, Type } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { LoadingSpinnerComponent } from '../loading-spinner/loading-spinner.component';
import { LazyLoadingErrorComponent } from '../lazy-loading-error/lazy-loading-error.component';
import { LazyLoadingService, LazyLoadingState } from '../../services/lazy-loading.service';

@Component({
  selector: 'app-lazy-wrapper',
  standalone: true,
  imports: [CommonModule, LoadingSpinnerComponent, LazyLoadingErrorComponent],
  template: `
    <div class="lazy-wrapper">
      <!-- Loading State -->
      <app-loading-spinner 
        *ngIf="loadingState.isLoading"
        [message]="getLoadingMessage()"
        [overlay]="false">
      </app-loading-spinner>
      
      <!-- Error State -->
      <app-lazy-loading-error
        *ngIf="loadingState.error && !loadingState.isLoading"
        [errorMessage]="loadingState.error"
        (retry)="onRetry()"
        (goHome)="onGoHome()">
      </app-lazy-loading-error>
      
      <!-- Component Container -->
      <div 
        #componentContainer 
        *ngIf="!loadingState.isLoading && !loadingState.error"
        class="component-container">
      </div>
    </div>
  `,
  styles: [`
    .lazy-wrapper {
      width: 100%;
      min-height: 200px;
    }
    
    .component-container {
      width: 100%;
    }
  `]
})
export class LazyWrapperComponent implements OnInit, OnDestroy {
  @Input() componentImport!: () => Promise<{ [key: string]: Type<any> }>;
  @Input() componentKey!: string;
  @Input() loadingMessage?: string;

  loadingState: LazyLoadingState = {
    isLoading: false,
    error: undefined,
    retryCount: 0
  };

  private destroy$ = new Subject<void>();
  private componentRef?: ComponentRef<any>;

  constructor(
    private viewContainer: ViewContainerRef,
    private lazyLoadingService: LazyLoadingService
  ) {}

  ngOnInit() {
    this.lazyLoadingService.loadingState$
      .pipe(takeUntil(this.destroy$))
      .subscribe(state => {
        this.loadingState = state;
      });

    this.loadComponent();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    
    if (this.componentRef) {
      this.componentRef.destroy();
    }
    
    this.lazyLoadingService.clearState();
  }

  private async loadComponent() {
    try {
      const module = await this.lazyLoadingService.loadComponent(this.componentImport);
      const componentType = module[this.componentKey];
      
      if (componentType) {
        this.componentRef = this.viewContainer.createComponent(componentType);
      } else {
        throw new Error(`Component ${this.componentKey} not found in module`);
      }
    } catch (error) {
      console.error('Failed to load component:', error);
    }
  }

  onRetry() {
    this.lazyLoadingService.retryLoading(this.componentImport)
      .then(() => {
        this.loadComponent();
      })
      .catch(error => {
        console.error('Retry failed:', error);
      });
  }

  onGoHome() {
    this.lazyLoadingService.goToHome();
  }

  getLoadingMessage(): string {
    if (this.loadingMessage) {
      return this.loadingMessage;
    }
    
    if (this.loadingState.retryCount > 0) {
      return `Tentativa ${this.loadingState.retryCount + 1} de carregamento...`;
    }
    
    return 'Carregando módulo...';
  }
}