import { createAction, props } from '@ngrx/store';
import { User, AuthResponse, LoginRequest } from '../../features/auth/models/auth.models';

// Login Actions
export const login = createAction(
  '[Auth] Login',
  props<{ request: LoginRequest }>()
);

export const loginSuccess = createAction(
  '[Auth] Login Success',
  props<{ authResponse: AuthResponse }>()
);

export const loginFailure = createAction(
  '[Auth] Login Failure',
  props<{ error: string }>()
);

// Logout Actions
export const logout = createAction('[Auth] Logout');

export const logoutSuccess = createAction('[Auth] Logout Success');

// Auto Login (from stored token)
export const autoLogin = createAction('[Auth] Auto Login');

export const autoLoginSuccess = createAction(
  '[Auth] Auto Login Success',
  props<{ user: User; token: string }>()
);

export const autoLoginFailure = createAction('[Auth] Auto Login Failure');

// Refresh Token Actions
export const refreshToken = createAction('[Auth] Refresh Token');

export const refreshTokenSuccess = createAction(
  '[Auth] Refresh Token Success',
  props<{ authResponse: AuthResponse }>()
);

export const refreshTokenFailure = createAction(
  '[Auth] Refresh Token Failure',
  props<{ error: string }>()
);

// Clear Auth Error
export const clearAuthError = createAction('[Auth] Clear Error');