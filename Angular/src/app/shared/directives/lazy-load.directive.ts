import { 
  Directive, 
  Input, 
  ViewContainerRef, 
  OnInit, 
  OnDestroy, 
  ComponentRef, 
  Type,
  TemplateRef,
  EmbeddedViewRef
} from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { LazyLoadingService, LazyLoadingState } from '../services/lazy-loading.service';
import { LoadingSpinnerComponent } from '../components/loading-spinner/loading-spinner.component';
import { LazyLoadingErrorComponent } from '../components/lazy-loading-error/lazy-loading-error.component';

@Directive({
  selector: '[appLazyLoad]',
  standalone: true
})
export class LazyLoadDirective implements OnInit, OnDestroy {
  @Input('appLazyLoad') componentImport!: () => Promise<{ [key: string]: Type<any> }>;
  @Input('lazyLoadKey') componentKey!: string;
  @Input('lazyLoadingTemplate') loadingTemplate?: TemplateRef<any>;
  @Input('lazyErrorTemplate') errorTemplate?: TemplateRef<any>;

  private destroy$ = new Subject<void>();
  private componentRef?: ComponentRef<any>;
  private loadingViewRef?: EmbeddedViewRef<any>;
  private errorViewRef?: EmbeddedViewRef<any>;

  constructor(
    private viewContainer: ViewContainerRef,
    private lazyLoadingService: LazyLoadingService
  ) {}

  ngOnInit() {
    this.lazyLoadingService.loadingState$
      .pipe(takeUntil(this.destroy$))
      .subscribe(state => {
        this.handleStateChange(state);
      });

    this.loadComponent();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    this.cleanup();
  }

  private async loadComponent() {
    try {
      const module = await this.lazyLoadingService.loadComponent(this.componentImport);
      const componentType = module[this.componentKey];
      
      if (componentType) {
        this.cleanup();
        this.componentRef = this.viewContainer.createComponent(componentType);
      } else {
        throw new Error(`Component ${this.componentKey} not found in module`);
      }
    } catch (error) {
      console.error('Failed to load component with directive:', error);
    }
  }

  private handleStateChange(state: LazyLoadingState) {
    if (state.isLoading) {
      this.showLoading();
    } else if (state.error) {
      this.showError(state.error);
    } else {
      this.hideLoadingAndError();
    }
  }

  private showLoading() {
    this.hideLoadingAndError();
    
    if (this.loadingTemplate) {
      this.loadingViewRef = this.viewContainer.createEmbeddedView(this.loadingTemplate);
    } else {
      // Create default loading component
      const loadingRef = this.viewContainer.createComponent(LoadingSpinnerComponent);
      loadingRef.instance.message = 'Carregando componente...';
      this.componentRef = loadingRef;
    }
  }

  private showError(errorMessage: string) {
    this.hideLoadingAndError();
    
    if (this.errorTemplate) {
      this.errorViewRef = this.viewContainer.createEmbeddedView(this.errorTemplate, {
        $implicit: errorMessage
      });
    } else {
      // Create default error component
      const errorRef = this.viewContainer.createComponent(LazyLoadingErrorComponent);
      errorRef.instance.errorMessage = errorMessage;
      errorRef.instance.retry.subscribe(() => this.onRetry());
      errorRef.instance.goHome.subscribe(() => this.onGoHome());
      this.componentRef = errorRef;
    }
  }

  private hideLoadingAndError() {
    if (this.loadingViewRef) {
      this.loadingViewRef.destroy();
      this.loadingViewRef = undefined;
    }
    
    if (this.errorViewRef) {
      this.errorViewRef.destroy();
      this.errorViewRef = undefined;
    }
  }

  private cleanup() {
    this.hideLoadingAndError();
    
    if (this.componentRef) {
      this.componentRef.destroy();
      this.componentRef = undefined;
    }
    
    this.viewContainer.clear();
  }

  private onRetry() {
    this.lazyLoadingService.retryLoading(this.componentImport)
      .then(() => {
        this.loadComponent();
      })
      .catch(error => {
        console.error('Retry failed:', error);
      });
  }

  private onGoHome() {
    this.lazyLoadingService.goToHome();
  }
}