import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { StorageService } from './storage.service';

export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
  schoolId?: string;
  permissions: string[];
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface AuthResponse {
  user: User;
  token: string;
  refreshToken: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);

  public currentUser$ = this.currentUserSubject.asObservable();
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private storageService: StorageService) {
    this.initializeAuth();
  }

  private initializeAuth(): void {
    const token = this.storageService.getItem('auth_token');
    const user = this.storageService.getItem('current_user');
    
    if (token && user) {
      this.currentUserSubject.next(JSON.parse(user));
      this.isAuthenticatedSubject.next(true);
    }
  }

  login(credentials: LoginCredentials): Observable<AuthResponse> {
    // TODO: Implementar chamada real para API
    // Por enquanto, simulação para desenvolvimento
    const mockUser: User = {
      id: '1',
      name: 'Usuário Teste',
      email: credentials.email,
      role: 'admin',
      permissions: ['read', 'write', 'delete']
    };

    const mockResponse: AuthResponse = {
      user: mockUser,
      token: 'mock-jwt-token',
      refreshToken: 'mock-refresh-token'
    };

    // Simular delay da API
    return new Observable(observer => {
      setTimeout(() => {
        this.setAuthData(mockResponse);
        observer.next(mockResponse);
        observer.complete();
      }, 1000);
    });
  }

  logout(): void {
    this.storageService.removeItem('auth_token');
    this.storageService.removeItem('refresh_token');
    this.storageService.removeItem('current_user');
    
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  refreshToken(): Observable<AuthResponse> {
    // TODO: Implementar refresh token real
    const refreshToken = this.storageService.getItem('refresh_token');
    
    if (!refreshToken) {
      this.logout();
      return new Observable(observer => {
        observer.error('No refresh token');
      });
    }

    // Simulação por enquanto
    return new Observable(observer => {
      observer.error('Not implemented');
    });
  }

  private setAuthData(authResponse: AuthResponse): void {
    this.storageService.setItem('auth_token', authResponse.token);
    this.storageService.setItem('refresh_token', authResponse.refreshToken);
    this.storageService.setItem('current_user', JSON.stringify(authResponse.user));
    
    this.currentUserSubject.next(authResponse.user);
    this.isAuthenticatedSubject.next(true);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return this.storageService.getItem('auth_token');
  }

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  hasPermission(permission: string): boolean {
    const user = this.getCurrentUser();
    return user?.permissions.includes(permission) || false;
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.role === role;
  }
}