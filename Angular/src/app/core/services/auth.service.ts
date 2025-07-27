import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { StorageService } from './storage.service';
import { environment } from '../../../environments/environment';
import {
  User,
  LoginCredentials,
  AuthResponse,
  ChangePasswordRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  PasswordStrength,
  LoginRequest,
  ExtendedRefreshTokenRequest
} from '../../features/auth/models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/api/auth`;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  private permissionsSubject = new BehaviorSubject<string[]>([]);

  public currentUser$ = this.currentUserSubject.asObservable();
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
  public permissions$ = this.permissionsSubject.asObservable();

  constructor(
    private http: HttpClient,
    private storageService: StorageService
  ) {
    this.initializeAuth();
  }

  private initializeAuth(): void {
    const token = this.storageService.getItem('auth_token');
    const user = this.storageService.getItem('current_user');
    const permissions = this.storageService.getItem('user_permissions');
    
    if (token && user) {
      try {
        const parsedUser = JSON.parse(user);
        const parsedPermissions = permissions ? JSON.parse(permissions) : [];
        
        this.currentUserSubject.next(parsedUser);
        this.permissionsSubject.next(parsedPermissions);
        this.isAuthenticatedSubject.next(true);
      } catch (error) {
        console.error('Error parsing stored auth data:', error);
        this.clearAuthData();
      }
    }
  }

  /**
   * Realiza login do usuário
   */
  login(credentials: LoginCredentials): Observable<AuthResponse> {
    const request: LoginRequest = {
      ...credentials,
      agenteUsuario: navigator.userAgent,
      enderecoIp: '0.0.0.0' // Placeholder, conforme discutido
    };
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => this.setAuthData(response)),
      catchError(this.handleError)
    );
  }

  /**
   * Realiza logout do usuário
   */
  logout(): Observable<any> {
    return this.http.post(`${this.apiUrl}/logout`, {}).pipe(
      tap(() => this.clearAuthData()),
      catchError(() => {
        // Clear local data even if API fails
        this.clearAuthData();
        return throwError('Logout failed');
      })
    );
  }

  /**
   * Renova o token de acesso
   */
  refreshToken(): Observable<AuthResponse> {
    const refreshToken = this.storageService.getItem('refresh_token');
    
    if (!refreshToken) {
      this.clearAuthData();
      return throwError('No refresh token available');
    }

    const request: ExtendedRefreshTokenRequest = {
      refreshToken,
      agenteUsuario: navigator.userAgent,
      enderecoIp: '0.0.0.0' // Placeholder
    };

    return this.http.post<AuthResponse>(`${this.apiUrl}/refresh`, request).pipe(
      tap(response => this.setAuthData(response)),
      catchError(error => {
        this.clearAuthData();
        return this.handleError(error);
      })
    );
  }

  /**
   * Altera a senha do usuário
   */
  changePassword(request: ChangePasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/change-password`, request).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Solicita recuperação de senha
   */
  forgotPassword(request: ForgotPasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/forgot-password`, request).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Redefine a senha usando token de recuperação
   */
  resetPassword(request: ResetPasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset-password`, request).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Confirma o email do usuário
   */
  confirmEmail(email: string, token: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/confirm-email`, {
      params: { email, token }
    }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Reenvia email de confirmação
   */
  resendEmailConfirmation(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/resend-confirmation`, email).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Obtém informações do usuário atual
   */
  getCurrentUserInfo(): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/me`).pipe(
      tap(user => this.currentUserSubject.next(user)),
      catchError(this.handleError)
    );
  }

  /**
   * Verifica se um email está disponível
   */
  checkEmailAvailability(email: string): Observable<{ available: boolean }> {
    return this.http.get<{ available: boolean }>(`${this.apiUrl}/check-email`, {
      params: { email }
    }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Valida a força de uma senha
   */
  validatePasswordStrength(password: string): Observable<PasswordStrength> {
    return this.http.post<PasswordStrength>(`${this.apiUrl}/validate-password`, password).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Invalida todas as sessões do usuário
   */
  invalidateAllSessions(): Observable<any> {
    return this.http.post(`${this.apiUrl}/invalidate-sessions`, {}).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Define os dados de autenticação após login/refresh
   */
  setAuthData(authResponse: AuthResponse): void {
    this.storageService.setItem('auth_token', authResponse.token);
    this.storageService.setItem('refresh_token', authResponse.refreshToken);
    this.storageService.setItem('current_user', JSON.stringify(authResponse.usuario));
    this.storageService.setItem('user_permissions', JSON.stringify(authResponse.permissoes));
    
    this.currentUserSubject.next(authResponse.usuario);
    this.permissionsSubject.next(authResponse.permissoes);
    this.isAuthenticatedSubject.next(true);
  }

  /**
   * Limpa todos os dados de autenticação
   */
  clearAuthData(): void {
    this.storageService.removeItem('auth_token');
    this.storageService.removeItem('refresh_token');
    this.storageService.removeItem('current_user');
    this.storageService.removeItem('user_permissions');
    this.storageService.removeItem('token_expires_at');
    
    this.currentUserSubject.next(null);
    this.permissionsSubject.next([]);
    this.isAuthenticatedSubject.next(false);
  }

  /**
   * Trata erros das requisições HTTP
   */
  private handleError = (error: any): Observable<never> => {
    let errorMessage = 'Erro desconhecido';
    
    if (error.error?.detail) {
      errorMessage = error.error.detail;
    } else if (error.error?.message) {
      errorMessage = error.error.message;
    } else if (error.message) {
      errorMessage = error.message;
    }
    
    return throwError(errorMessage);
  };

  // Métodos de conveniência
  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return this.storageService.getItem('auth_token');
  }

  getRefreshToken(): string | null {
    return this.storageService.getItem('refresh_token');
  }

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  isTokenExpired(): boolean {
    const expiresAt = this.storageService.getItem('token_expires_at');
    if (!expiresAt) return true;
    
    return new Date(expiresAt) <= new Date();
  }

  hasPermission(permission: string): boolean {
    const permissions = this.permissionsSubject.value;
    return permissions.includes(permission);
  }

  hasAnyPermission(permissions: string[]): boolean {
    const userPermissions = this.permissionsSubject.value;
    return permissions.some(permission => userPermissions.includes(permission));
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.codigoPerfil === role;
  }

  hasAnyRole(roles: string[]): boolean {
    const user = this.getCurrentUser();
    return user ? roles.includes(user.codigoPerfil) : false;
  }

  canAccessSchool(schoolId: string): boolean {
    const user = this.getCurrentUser();
    if (!user) return false;
    
    // SuperAdmin pode acessar qualquer escola
    if (user.codigoPerfil === 'SuperAdmin') return true;
    
    // Verificar se o usuário tem acesso à escola específica
    return user.escolaId === schoolId;
  }

  getUserInitials(): string {
    const user = this.getCurrentUser();
    return user?.iniciais || '';
  }

  getUserFullName(): string {
    const user = this.getCurrentUser();
    return user?.nomeCompleto || '';
  }

  isEmailConfirmed(): boolean {
    const user = this.getCurrentUser();
    return user?.emailConfirmado || false;
  }
}