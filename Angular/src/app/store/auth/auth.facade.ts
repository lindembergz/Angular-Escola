import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AppState } from '../app.state';
import { User, LoginCredentials } from '../../core/services/auth.service';
import * as AuthActions from './auth.actions';
import * as AuthSelectors from './auth.selectors';

@Injectable({
  providedIn: 'root'
})
export class AuthFacade {
  // Selectors
  currentUser$ = this.store.select(AuthSelectors.selectCurrentUser);
  isAuthenticated$ = this.store.select(AuthSelectors.selectIsAuthenticated);
  authToken$ = this.store.select(AuthSelectors.selectAuthToken);
  isLoading$ = this.store.select(AuthSelectors.selectAuthLoading);
  error$ = this.store.select(AuthSelectors.selectAuthError);
  userRole$ = this.store.select(AuthSelectors.selectUserRole);
  userPermissions$ = this.store.select(AuthSelectors.selectUserPermissions);
  userSchoolId$ = this.store.select(AuthSelectors.selectUserSchoolId);

  constructor(private store: Store<AppState>) {}

  // Actions
  login(credentials: LoginCredentials): void {
    this.store.dispatch(AuthActions.login({ credentials }));
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
      return user?.role === role || false;
    });
  }

  hasPermission(permission: string): Observable<boolean> {
    return this.store.select(state => {
      const permissions = AuthSelectors.selectUserPermissions(state);
      return permissions.includes(permission);
    });
  }
}