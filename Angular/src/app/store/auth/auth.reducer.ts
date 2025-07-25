import { createReducer, on } from '@ngrx/store';
import { User } from '../../features/auth/models/auth.models';
import * as AuthActions from './auth.actions';

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

export const initialState: AuthState = {
  user: null,
  token: null,
  isAuthenticated: false,
  isLoading: false,
  error: null
};

export const authReducer = createReducer(
  initialState,

  // Login
  on(AuthActions.login, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(AuthActions.loginSuccess, (state, { authResponse }) => ({
    ...state,
    user: authResponse.usuario,
    token: authResponse.token,
    isAuthenticated: true,
    isLoading: false,
    error: null
  })),

  on(AuthActions.loginFailure, (state, { error }) => ({
    ...state,
    user: null,
    token: null,
    isAuthenticated: false,
    isLoading: false,
    error
  })),

  // Logout
  on(AuthActions.logout, (state) => ({
    ...state,
    isLoading: true
  })),

  on(AuthActions.logoutSuccess, () => ({
    ...initialState
  })),

  // Auto Login
  on(AuthActions.autoLogin, (state) => ({
    ...state,
    isLoading: true
  })),

  on(AuthActions.autoLoginSuccess, (state, { user, token }) => ({
    ...state,
    user,
    token,
    isAuthenticated: true,
    isLoading: false,
    error: null
  })),

  on(AuthActions.autoLoginFailure, (state) => ({
    ...state,
    isLoading: false,
    error: null
  })),

  // Refresh Token
  on(AuthActions.refreshToken, (state) => ({
    ...state,
    isLoading: true
  })),

  on(AuthActions.refreshTokenSuccess, (state, { authResponse }) => ({
    ...state,
    user: authResponse.usuario,
    token: authResponse.token,
    isAuthenticated: true,
    isLoading: false,
    error: null
  })),

  on(AuthActions.refreshTokenFailure, (state, { error }) => ({
    ...state,
    token: null,
    isAuthenticated: false,
    isLoading: false,
    error
  })),

  // Clear Error
  on(AuthActions.clearAuthError, (state) => ({
    ...state,
    error: null
  }))
);