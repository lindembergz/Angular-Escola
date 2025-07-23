import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AppState } from '../app.state';
import { User, LoginCredentials, LoginRequest } from '../../features/auth/models/auth.models';
import * as AuthActions from './auth.actions';
import * as AuthSelectors from './auth.selectors';

@Injectable({
  providedIn: 'root'
})
export class AuthFacade {
  // Selectors
  readonly currentUser$;
  readonly isAuthenticated$;
  readonly authToken$;
  readonly isLoading$;
  readonly error$;
  readonly userRole$;
  readonly userPermissions$;
  readonly userSchoolId$;

  constructor(private store: Store<AppState>) {
    this.currentUser$ = this.store.select(AuthSelectors.selectCurrentUser);
    this.isAuthenticated$ = this.store.select(AuthSelectors.selectIsAuthenticated);
    this.authToken$ = this.store.select(AuthSelectors.selectAuthToken);
    this.isLoading$ = this.store.select(AuthSelectors.selectAuthLoading);
    this.error$ = this.store.select(AuthSelectors.selectAuthError);
    this.userRole$ = this.store.select(AuthSelectors.selectUserRole);
    this.userPermissions$ = this.store.select(AuthSelectors.selectUserPermissions);
    this.userSchoolId$ = this.store.select(AuthSelectors.selectUserSchoolId);
  }

  // Actions
  login(credentials: LoginCredentials): void {
    const request: LoginRequest = {
      ...credentials,
      agenteUsuario: navigator.userAgent,
      enderecoIp: '0.0.0.0'
    };
    this.store.dispatch(AuthActions.login({ request }));
  }

  logout(): void {
    this.store.dispatch(AuthActions.logout());
  }

  autoLogin(): void {
    this.store.dispatch(AuthActions.autoLogin());
  }

  refreshToken(): void {
    this.store.dispatch(AuthActions.refreshToken());
  }

  clearError(): void {
    this.store.dispatch(AuthActions.clearAuthError());
  }

  // Helper methods
  getCurrentUser(): Observable<User | null> {
    return this.currentUser$;
  }

  isAuthenticated(): Observable<boolean> {
    return this.isAuthenticated$;
  }

  hasRole(role: string): Observable<boolean> {
    return this.store.select(state => {
      const user = AuthSelectors.selectCurrentUser(state);
      return user?.codigoPerfil === role || false;
    });
  }

  hasPermission(permission: string): Observable<boolean> {
    return this.store.select(state => {
      const permissions = AuthSelectors.selectUserPermissions(state);
      return permissions.includes(permission);
    });
  }
}