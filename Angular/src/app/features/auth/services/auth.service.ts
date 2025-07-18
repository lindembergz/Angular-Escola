import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError, timer } from 'rxjs';
import { catchError, tap, map } from 'rxjs/operators';
import { Router } from '@angular/router';

import { StorageService } from '../../../core/services/storage.service';
import { NotificationService } from '../../../core/services/notification.service';
import { environment } from '../../../../environments/environment';

import {
  LoginRequest,
  LoginCredentials,
  AuthResponse,
  User,
  ExtendedRefreshTokenRequest,
  ChangePasswordRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  PasswordStrengthResponse,
  EmailAvailabilityResponse,
  ApiError,
  AuthEvent,
  AuthEventType,
  TokenInfo,
  AuthConfig
} from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_BASE = `${environment.apiUrl}/api/auth`;
  
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
  
  // Timer para renovação automática de token
  private tokenRefreshTimer?: any;
  
  // Configurações
  private readonly config: AuthConfig = {
    apiBaseUrl: this.API_BASE,
    tokenStorageKey: 'auth_token',
    refreshTokenStorageKey: 'refresh_token',
    userStorageKey: 'current_user',
    tokenExpirationBuffer: 5, // 5 minutos antes da expiração
    maxLoginAttempts: 5,
    lockoutDuration: 30, // 30 minutos
    passwordValidation: {
      minLength: 8,
      requireUppercase: true,
      requireLowercase: true,
      requireNumbers: true,
      requireSpecialChars: true,
      maxLength: 128
    }
  };

  constructor(
    private http: HttpClient,
    private storageService: StorageService,
    private notificationService: NotificationService,
    private router: Router
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
      
      if (token && user && !this.isTokenExpired(token)) {
        this.setAuthenticatedState(user, token);
        this.scheduleTokenRefresh(token);
      } else {
        this.clearAuthData();
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

    // Enriquecer credenciais com informações do cliente
    const enrichedCredentials: LoginRequest = {
      ...credentials,
      ipAddress: this.getClientIP(),
      userAgent: navigator.userAgent
    };

    return this.http.post<AuthResponse>(`${this.API_BASE}/login`, enrichedCredentials)
      .pipe(
        tap(response => {
          this.handleLoginSuccess(response);
          this.logAuthEvent(AuthEventType.Login, true, credentials.email);
        }),
        catchError(error => {
          this.handleAuthError(error);
          this.logAuthEvent(AuthEventType.Login, false, credentials.email, error);
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
    
    const currentUser = this.getCurrentUser();
    
    return this.http.post<void>(`${this.API_BASE}/logout`, {})
      .pipe(
        tap(() => {
          this.handleLogoutSuccess();
          this.logAuthEvent(AuthEventType.Logout, true, currentUser?.email);
        }),
        catchError(error => {
          // Mesmo com erro na API, fazer logout local
          this.handleLogoutSuccess();
          this.logAuthEvent(AuthEventType.Logout, false, currentUser?.email, error);
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
      this.handleLogoutSuccess();
      return throwError(() => new Error('Refresh token não encontrado'));
    }

    const request: ExtendedRefreshTokenRequest = {
      refreshToken,
      accessToken: this.getStoredToken(),
      ipAddress: this.getClientIP(),
      userAgent: navigator.userAgent
    };

    return this.http.post<AuthResponse>(`${this.API_BASE}/refresh`, request)
      .pipe(
        tap(response => {
          this.handleTokenRefreshSuccess(response);
          this.logAuthEvent(AuthEventType.TokenRefresh, true);
        }),
        catchError(error => {
          this.handleTokenRefreshError(error);
          this.logAuthEvent(AuthEventType.TokenRefresh, false, undefined, error);
          return throwError(() => error);
        })
      );
  }

  /**
   * Altera a senha do usuário
   */
  changePassword(request: ChangePasswordRequest): Observable<void> {
    this.setLoadingState(true);
    
    return this.http.post<void>(`${this.API_BASE}/change-password`, request)
      .pipe(
        tap(() => {
          this.notificationService.success('Senha alterada com sucesso');
          this.logAuthEvent(AuthEventType.PasswordChange, true);
          
          if (request.invalidateAllSessions) {
            // Se invalidou todas as sessões, fazer logout
            this.handleLogoutSuccess();
            this.router.navigate(['/auth/login']);
          }
        }),
        catchError(error => {
          this.handleAuthError(error);
          this.logAuthEvent(AuthEventType.PasswordChange, false, undefined, error);
          return throwError(() => error);
        }),
        tap(() => this.setLoadingState(false))
      );
  }

  /**
   * Solicita recuperação de senha
   */
  forgotPassword(request: ForgotPasswordRequest): Observable<void> {
    this.setLoadingState(true);
    
    return this.http.post<void>(`${this.API_BASE}/forgot-password`, request)
      .pipe(
        tap(() => {
          this.notificationService.success(
            'Se o email existir, você receberá instruções para recuperação da senha'
          );
        }),
        catchError(error => {
          this.handleAuthError(error);
          return throwError(() => error);
        }),
        tap(() => this.setLoadingState(false))
      );
  }

  /**
   * Redefine a senha usando token de recuperação
   */
  resetPassword(request: ResetPasswordRequest): Observable<void> {
    this.setLoadingState(true);
    
    return this.http.post<void>(`${this.API_BASE}/reset-password`, request)
      .pipe(
        tap(() => {
          this.notificationService.success('Senha redefinida com sucesso');
          this.logAuthEvent(AuthEventType.PasswordReset, true, request.email);
          this.router.navigate(['/auth/login']);
        }),
        catchError(error => {
          this.handleAuthError(error);
          this.logAuthEvent(AuthEventType.PasswordReset, false, request.email, error);
          return throwError(() => error);
        }),
        tap(() => this.setLoadingState(false))
      );
  }

  /**
   * Confirma o email do usuário
   */
  confirmEmail(email: string, token: string): Observable<void> {
    this.setLoadingState(true);
    
    return this.http.get<void>(`${this.API_BASE}/confirm-email`, {
      params: { email, token }
    }).pipe(
      tap(() => {
        this.notificationService.success('Email confirmado com sucesso');
        this.logAuthEvent(AuthEventType.EmailConfirmation, true, email);
        
        // Atualizar usuário atual se for o mesmo
        const currentUser = this.getCurrentUser();
        if (currentUser && currentUser.email === email) {
          const updatedUser = { ...currentUser, emailConfirmed: true };
          this.currentUserSubject.next(updatedUser);
          this.storageService.setItem(this.config.userStorageKey, JSON.stringify(updatedUser));
        }
      }),
      catchError(error => {
        this.handleAuthError(error);
        this.logAuthEvent(AuthEventType.EmailConfirmation, false, email, error);
        return throwError(() => error);
      }),
      tap(() => this.setLoadingState(false))
    );
  }

  /**
   * Reenvia email de confirmação
   */
  resendEmailConfirmation(email: string): Observable<void> {
    return this.http.post<void>(`${this.API_BASE}/resend-confirmation`, email)
      .pipe(
        tap(() => {
          this.notificationService.success(
            'Se o email existir e não estiver confirmado, você receberá um novo link de confirmação'
          );
        }),
        catchError(error => {
          this.handleAuthError(error);
          return throwError(() => error);
        })
      );
  }

  /**
   * Verifica se um email está disponível
   */
  checkEmailAvailability(email: string): Observable<boolean> {
    return this.http.get<EmailAvailabilityResponse>(`${this.API_BASE}/check-email`, {
      params: { email }
    }).pipe(
      map(response => response.available),
      catchError(() => {
        // Em caso de erro, assume que não está disponível
        return throwError(() => false);
      })
    );
  }

  /**
   * Valida a força de uma senha
   */
  validatePasswordStrength(password: string): Observable<PasswordStrengthResponse> {
    return this.http.post<PasswordStrengthResponse>(`${this.API_BASE}/validate-password`, password);
  }

  /**
   * Invalida todas as sessões do usuário
   */
  invalidateAllSessions(): Observable<void> {
    return this.http.post<void>(`${this.API_BASE}/invalidate-sessions`, {})
      .pipe(
        tap(() => {
          this.notificationService.success('Todas as sessões foram invalidadas');
          this.handleLogoutSuccess();
          this.router.navigate(['/auth/login']);
        }),
        catchError(error => {
          this.handleAuthError(error);
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

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  hasPermission(_permission: string): boolean {
    const user = this.getCurrentUser();
    // TODO: Implementar verificação de permissões quando integrar com backend
    return user !== null;
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.role === role;
  }

  hasAnyRole(roles: string[]): boolean {
    const user = this.getCurrentUser();
    return user ? roles.includes(user.role) : false;
  }

  getTokenInfo(): TokenInfo | null {
    const token = this.getStoredToken();
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiresAt = new Date(payload.exp * 1000);
      const now = new Date();
      
      return {
        token,
        expiresAt,
        isExpired: expiresAt <= now,
        expiresInMinutes: Math.max(0, Math.floor((expiresAt.getTime() - now.getTime()) / 60000))
      };
    } catch {
      return null;
    }
  }

  // Métodos privados

  private handleLoginSuccess(response: AuthResponse): void {
    this.setAuthData(response);
    this.scheduleTokenRefresh(response.accessToken);
    
    if (response.requiresPasswordChange) {
      this.notificationService.warn('Você precisa alterar sua senha');
      this.router.navigate(['/profile/change-password']);
    } else if (response.requiresEmailConfirmation) {
      this.notificationService.info('Verifique seu email para confirmar sua conta');
    } else {
      this.router.navigate(['/dashboard']);
    }
  }

  private handleLogoutSuccess(): void {
    this.clearAuthData();
    this.clearTokenRefreshTimer();
    this.router.navigate(['/auth/login']);
  }

  private handleTokenRefreshSuccess(response: AuthResponse): void {
    this.setAuthData(response);
    this.scheduleTokenRefresh(response.accessToken);
  }

  private handleTokenRefreshError(error: any): void {
    console.error('Erro ao renovar token:', error);
    this.handleLogoutSuccess();
  }

  private handleAuthError(error: HttpErrorResponse): void {
    let errorMessage = 'Erro interno do servidor';
    
    if (error.error && typeof error.error === 'object') {
      const apiError = error.error as ApiError;
      errorMessage = apiError.detail || apiError.title || errorMessage;
    } else if (error.message) {
      errorMessage = error.message;
    }
    
    this.authErrorSubject.next(errorMessage);
    this.notificationService.error(errorMessage);
  }

  private setAuthData(response: AuthResponse): void {
    this.storageService.setItem(this.config.tokenStorageKey, response.accessToken);
    this.storageService.setItem(this.config.refreshTokenStorageKey, response.refreshToken);
    this.storageService.setItem(this.config.userStorageKey, JSON.stringify(response.user));
    
    this.setAuthenticatedState(response.user, response.accessToken);
  }

  private setAuthenticatedState(user: User, _token: string): void {
    this.currentUserSubject.next(user);
    this.isAuthenticatedSubject.next(true);
  }

  private clearAuthData(): void {
    this.storageService.removeItem(this.config.tokenStorageKey);
    this.storageService.removeItem(this.config.refreshTokenStorageKey);
    this.storageService.removeItem(this.config.userStorageKey);
    
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
    this.clearError();
  }

  private getStoredToken(): string | null {
    return this.storageService.getItem(this.config.tokenStorageKey);
  }

  private getStoredRefreshToken(): string | null {
    return this.storageService.getItem(this.config.refreshTokenStorageKey);
  }

  private getStoredUser(): User | null {
    const userJson = this.storageService.getItem(this.config.userStorageKey);
    return userJson ? JSON.parse(userJson) : null;
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationTime = payload.exp * 1000;
      return Date.now() >= expirationTime;
    } catch {
      return true;
    }
  }

  private scheduleTokenRefresh(token: string): void {
    this.clearTokenRefreshTimer();
    
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationTime = payload.exp * 1000;
      const refreshTime = expirationTime - (this.config.tokenExpirationBuffer * 60 * 1000);
      const delay = refreshTime - Date.now();
      
      if (delay > 0) {
        this.tokenRefreshTimer = timer(delay).subscribe(() => {
          this.refreshToken().subscribe({
            error: () => this.handleLogoutSuccess()
          });
        });
      }
    } catch (error) {
      console.error('Erro ao agendar renovação de token:', error);
    }
  }

  private clearTokenRefreshTimer(): void {
    if (this.tokenRefreshTimer) {
      this.tokenRefreshTimer.unsubscribe();
      this.tokenRefreshTimer = undefined;
    }
  }

  private setLoadingState(loading: boolean): void {
    this.isLoadingSubject.next(loading);
  }

  private clearError(): void {
    this.authErrorSubject.next(null);
  }

  private getClientIP(): string {
    // Em produção, isso seria obtido do servidor
    return 'unknown';
  }

  private logAuthEvent(
    type: AuthEventType, 
    success: boolean, 
    email?: string, 
    error?: any
  ): void {
    const event: AuthEvent = {
      type,
      timestamp: new Date(),
      userId: this.getCurrentUser()?.id,
      email,
      ipAddress: this.getClientIP(),
      userAgent: navigator.userAgent,
      success,
      error: error?.message || error?.error?.detail,
      metadata: {
        url: window.location.href,
        referrer: document.referrer
      }
    };
    
    // TODO: Enviar eventos para serviço de analytics/logging
    console.log('Auth Event:', event);
  }
}