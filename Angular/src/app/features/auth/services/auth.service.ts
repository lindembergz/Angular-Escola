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
  User,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  ChangePasswordRequest,
  EmailAvailabilityResponse,
  PasswordStrengthResponse,
  LoginRequest,
  ExtendedRefreshTokenRequest
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

    const request: LoginRequest = {
      ...credentials,
      agenteUsuario: navigator.userAgent,
      enderecoIp: '0.0.0.0' // Placeholder, as direct client IP is not accessible from frontend
    };

    return this.http.post<AuthResponse>(`${this.API_BASE}/login`, request)
      .pipe(
        tap(response => {
          this.setAuthData(response);
        }),
        catchError(error => {
          const errorMessage = error?.error?.detail || error?.error?.message || error?.message || 'Erro no login';
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

    const request: ExtendedRefreshTokenRequest = {
      refreshToken,
      agenteUsuario: navigator.userAgent,
      enderecoIp: '0.0.0.0' // Placeholder
    };

    return this.http.post<AuthResponse>(`${this.API_BASE}/refresh`, request)
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

  /**
   * Solicita recuperação de senha
   */
  forgotPassword(request: ForgotPasswordRequest): Observable<void> {
    this.setLoadingState(true);
    this.clearError();

    return this.http.post<void>(`${this.API_BASE}/forgot-password`, request)
      .pipe(
        tap(() => {
          this.notificationService.success('Se o email existir, você receberá instruções para recuperação');
        }),
        catchError(error => {
          const errorMessage = error?.error?.detail || error?.message || 'Erro ao solicitar recuperação';
          this.authErrorSubject.next(errorMessage);
          this.notificationService.error(errorMessage);
          return throwError(() => error);
        }),
        tap(() => this.setLoadingState(false))
      );
  }

  /**
   * Redefine senha usando token de recuperação
   */
  resetPassword(request: ResetPasswordRequest): Observable<void> {
    this.setLoadingState(true);
    this.clearError();

    return this.http.post<void>(`${this.API_BASE}/reset-password`, request)
      .pipe(
        tap(() => {
          this.notificationService.success('Senha redefinida com sucesso');
        }),
        catchError(error => {
          const errorMessage = error?.error?.detail || error?.message || 'Erro ao redefinir senha';
          this.authErrorSubject.next(errorMessage);
          this.notificationService.error(errorMessage);
          return throwError(() => error);
        }),
        tap(() => this.setLoadingState(false))
      );
  }

  /**
   * Confirma email do usuário
   */
  confirmEmail(email: string, token: string): Observable<void> {
    this.setLoadingState(true);
    this.clearError();

    return this.http.get<void>(`${this.API_BASE}/confirm-email`, {
      params: { email, token }
    })
      .pipe(
        tap(() => {
          this.notificationService.success('Email confirmado com sucesso');
        }),
        catchError(error => {
          const errorMessage = error?.error?.detail || error?.message || 'Erro ao confirmar email';
          this.authErrorSubject.next(errorMessage);
          this.notificationService.error(errorMessage);
          return throwError(() => error);
        }),
        tap(() => this.setLoadingState(false))
      );
  }

  /**
   * Reenvia confirmação de email
   */
  resendEmailConfirmation(email: string): Observable<void> {
    this.setLoadingState(true);
    this.clearError();

    return this.http.post<void>(`${this.API_BASE}/resend-confirmation`, email)
      .pipe(
        tap(() => {
          this.notificationService.success('Nova confirmação enviada para seu email');
        }),
        catchError(error => {
          const errorMessage = error?.error?.detail || error?.message || 'Erro ao reenviar confirmação';
          this.authErrorSubject.next(errorMessage);
          this.notificationService.error(errorMessage);
          return throwError(() => error);
        }),
        tap(() => this.setLoadingState(false))
      );
  }

  /**
   * Altera senha do usuário
   */
  changePassword(request: ChangePasswordRequest): Observable<void> {
    this.setLoadingState(true);
    this.clearError();

    return this.http.post<void>(`${this.API_BASE}/change-password`, request)
      .pipe(
        tap(() => {
          this.notificationService.success('Senha alterada com sucesso');
        }),
        catchError(error => {
          const errorMessage = error?.error?.detail || error?.message || 'Erro ao alterar senha';
          this.authErrorSubject.next(errorMessage);
          this.notificationService.error(errorMessage);
          return throwError(() => error);
        }),
        tap(() => this.setLoadingState(false))
      );
  }

  /**
   * Verifica disponibilidade de email
   */
  checkEmailAvailability(email: string): Observable<EmailAvailabilityResponse> {
    return this.http.get<EmailAvailabilityResponse>(`${this.API_BASE}/check-email`, {
      params: { email }
    });
  }

  /**
   * Valida força da senha
   */
  validatePasswordStrength(password: string): Observable<PasswordStrengthResponse> {
    return this.http.post<PasswordStrengthResponse>(`${this.API_BASE}/validate-password`, password);
  }

  /**
   * Invalida todas as sessões do usuário
   */
  invalidateAllSessions(): Observable<void> {
    this.setLoadingState(true);

    return this.http.post<void>(`${this.API_BASE}/invalidate-sessions`, {})
      .pipe(
        tap(() => {
          this.notificationService.success('Todas as sessões foram invalidadas');
        }),
        catchError(error => {
          const errorMessage = error?.error?.detail || error?.message || 'Erro ao invalidar sessões';
          this.notificationService.error(errorMessage);
          return throwError(() => error);
        }),
        tap(() => this.setLoadingState(false))
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
    this.storageService.setItem('auth_token', response.token);
    this.storageService.setItem('refresh_token', response.refreshToken);
    this.storageService.setItem('current_user', JSON.stringify(response.usuario));
    this.storageService.setItem('user_permissions', JSON.stringify(response.permissoes));
    this.storageService.setItem('token_expires_at', response.expiraEm_DateTime);
    
    this.currentUserSubject.next(response.usuario);
    this.isAuthenticatedSubject.next(true);
  }

  private clearAuthData(): void {
    this.storageService.removeItem('auth_token');
    this.storageService.removeItem('refresh_token');
    this.storageService.removeItem('current_user');
    this.storageService.removeItem('user_permissions');
    this.storageService.removeItem('token_expires_at');
    
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