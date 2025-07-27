import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterOutlet, Router, NavigationStart, NavigationEnd, NavigationError } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Message } from 'primeng/message';
import { Toast } from 'primeng/toast';
import { Subject, takeUntil } from 'rxjs';
import { Store } from '@ngrx/store';
import { LoadingSpinnerComponent } from './shared/components/loading-spinner/loading-spinner.component';
import { HeaderComponent } from './layout/components/header/header.component';
import { AuthState } from './store/auth/auth.reducer';
import * as AuthActions from './store/auth/auth.actions';
import * as AuthSelectors from './store/auth/auth.selectors';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule, Message, Toast, LoadingSpinnerComponent, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Sistema de Gestão Escolar';
  isNavigating = false;
  errorMessage: string | null = null;
  isAuthenticated = false;
  
  private destroy$ = new Subject<void>();
  
  constructor(
    private router: Router,
    private store: Store<AuthState>
  ) {}

  ngOnInit() {
    // Initialize auto-login on app start
    this.store.dispatch(AuthActions.autoLogin());

    // Listen to authentication state from store
    this.store.select(AuthSelectors.selectIsAuthenticated)
      .pipe(takeUntil(this.destroy$))
      .subscribe(isAuthenticated => {
        this.isAuthenticated = isAuthenticated;
        // Force change detection when authentication state changes
        if (!isAuthenticated) {
          // Ensure we're on login page when not authenticated
          const currentUrl = this.router.url;
          if (!currentUrl.startsWith('/auth')) {
            this.router.navigate(['/auth/login']);
          }
        }
      });

    // Listen to router events for loading states
    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        this.isNavigating = true;
        this.errorMessage = null;
      } else if (event instanceof NavigationEnd) {
        this.isNavigating = false;
      } else if (event instanceof NavigationError) {
        this.isNavigating = false;
        this.handleNavigationError(event);
      }
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  hasActiveRoute(): boolean {
    // Sempre retorna true pois agora temos o router-outlet funcionando
    return true;
  }

  private handleNavigationError(event: NavigationError) {
    console.error('Navigation error:', event.error);
    this.errorMessage = 'Erro ao carregar página. Redirecionando para o dashboard...';
    setTimeout(() => {
      this.router.navigate(['/']);
    }, 2000);
  }

  dismissError() {
    this.errorMessage = null;
  }
}
