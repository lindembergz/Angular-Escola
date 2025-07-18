import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

import { StorageService } from '../../../core/services/storage.service';
import { NotificationService } from '../../../core/services/notification.service';
import { environment } from '../../../../environments/environment';

import {
  LoginCredentials,
  AuthResponse,
  User
} from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_BASE = `${environment.apiUrl}/auth`;
  
  // Subjects para estado reativo
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  private isLoadingSubject = new BehaviorSubject<boolean>(false);
  private authErrorSubject = new BehaviorSubject<string | null>(null);
  
  // Observables públicos
  public readonly currentUser$ = this.currentUserSubject.asObservable();
  public readonly isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
  public readonly isLoading$ = this.isLoadingSubject.asObservable();
  public readonly authError$ = this.authErrorSubject.asObservable();

  constructor(
    private http: HttpClient,
    private storageService: StorageService,
    private notificationService: NotificationService
  ) {
    this.initializeAuth();
  }

  /**
   * Inicializa o estado de autenticação a partir do storage
   */
  private initializeAuth(): void {
    try {
      const token = this.getStoredToken();
      const user = this.getStoredUser();
      
      if (token && user) {
        this.currentUserSubject.next(user);
        this.isAuthenticatedSubject.next(true);
      }
    } catch (error) {
      console.error('Erro ao inicializar autenticação:', error);
      this.clearAuthData();
    }
  }

  /**
   * Realiza login do usuário
   */
  login(credentials: LoginCredentials): Observable<AuthResponse> {
    this.setLoadingState(true);
    this.clearError();

    return this.http.post<AuthResponse>(`${this.API_BASE}/login`, credentials)
      .pipe(
        tap(response => {
          this.setAuthData(response);
          this.notificationService.success(`Bem-vindo, ${response.user.fullName}!`);
        }),
        catchError(error => {
          const errorMessage = error?.error?.message || error?.message || 'Erro no login';
          this.authErrorSubject.next(errorMessage);
          this.notificationService.error(errorMessage);
          return throwError(() => error);
        }),
        tap(() => this.setLoadingState(false))
      );
  }

  /**
   * Realiza logout do usuário
   */
  logout(): Observable<void> {
    this.setLoadingState(true);
    
    return this.http.post<void>(`${this.API_BASE}/logout`, {})
      .pipe(
        tap(() => {
          this.clearAuthData();
          this.notificationService.success('Logout realizado com sucesso');
        }),
        catchError(error => {
          // Mesmo com erro na API, fazer logout local
          this.clearAuthData();
          return throwError(() => error);
        }),
        tap(() => this.setLoadingState(false))
      );
  }

  /**
   * Renova o token de acesso
   */
  refreshToken(): Observable<AuthResponse> {
    const refreshToken = this.getStoredRefreshToken();
    
    if (!refreshToken) {
      this.clearAuthData();
      return throwError(() => new Error('Refresh token não encontrado'));
    }

    return this.http.post<AuthResponse>(`${this.API_BASE}/refresh`, { refreshToken })
      .pipe(
        tap(response => {
          this.setAuthData(response);
        }),
        catchError(error => {
          this.clearAuthData();
          return throwError(() => error);
        })
      );
  }

  // Métodos de utilidade pública
  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return this.getStoredToken();
  }

  getRefreshToken(): string | null {
    return this.getStoredRefreshToken();
  }

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  // Métodos privados
  private setAuthData(response: AuthResponse): void {
    this.storageService.setItem('auth_token', response.accessToken);
    this.storageService.setItem('refresh_token', response.refreshToken);
    this.storageService.setItem('current_user', JSON.stringify(response.user));
    
    this.currentUserSubject.next(response.user);
    this.isAuthenticatedSubject.next(true);
  }

  private clearAuthData(): void {
    this.storageService.removeItem('auth_token');
    this.storageService.removeItem('refresh_token');
    this.storageService.removeItem('current_user');
    
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
    this.clearError();
  }

  private getStoredToken(): string | null {
    return this.storageService.getItem('auth_token');
  }

  private getStoredRefreshToken(): string | null {
    return this.storageService.getItem('refresh_token');
  }

  private getStoredUser(): User | null {
    const userJson = this.storageService.getItem('current_user');
    return userJson ? JSON.parse(userJson) : null;
  }

  private setLoadingState(loading: boolean): void {
    this.isLoadingSubject.next(loading);
  }

  private clearError(): void {
    this.authErrorSubject.next(null);
  }
}