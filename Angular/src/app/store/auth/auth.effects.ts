import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { map, exhaustMap, catchError, tap } from 'rxjs/operators';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import * as AuthActions from './auth.actions';

@Injectable()
export class AuthEffects {
  private actions$ = inject(Actions);
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);

  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.login),
      exhaustMap(({ request }) =>
        this.authService.login(request).pipe(
          map((authResponse) => AuthActions.loginSuccess({ authResponse })),
          catchError((error) =>
            of(AuthActions.loginFailure({ error: error?.error?.detail || error?.error?.message || error?.message || 'Erro no login' }))
          )
        )
      )
    )
  );

  loginSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.loginSuccess),
        tap(({ authResponse }) => {
          // Ensure AuthService is updated with auth data
          this.authService.setAuthData(authResponse);
          this.notificationService.success(`Bem-vindo, ${authResponse.usuario.nomeCompleto}!`);
          this.router.navigate(['/dashboard']);
        })
      ),
    { dispatch: false }
  );

  loginFailure$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.loginFailure),
        tap(({ error }) => {
          this.notificationService.error(error);
        })
      ),
    { dispatch: false }
  );

  logout$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.logout),
      exhaustMap(() =>
        this.authService.logout().pipe(
          map(() => AuthActions.logoutSuccess()),
          catchError(() => {
            // Even if API fails, logout locally
            this.authService.clearAuthData();
            return of(AuthActions.logoutSuccess());
          })
        )
      )
    )
  );

  logoutSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.logoutSuccess),
        tap(() => {
          // Clear auth data first to ensure immediate state update
          this.authService.clearAuthData();
          this.notificationService.success('Logout realizado com sucesso');
          // Navigate to login after clearing data
          this.router.navigate(['/auth/login']);
        })
      ),
    { dispatch: false }
  );

  autoLogin$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.autoLogin),
      exhaustMap(() => {
        const user = this.authService.getCurrentUser();
        const token = this.authService.getToken();
        
        if (user && token) {
          return of(AuthActions.autoLoginSuccess({ user, token }));
        } else {
          return of(AuthActions.autoLoginFailure());
        }
      })
    )
  );

  refreshToken$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.refreshToken),
      exhaustMap(() =>
        this.authService.refreshToken().pipe(
          map((authResponse) => AuthActions.refreshTokenSuccess({ authResponse })),
          catchError((error) =>
            of(AuthActions.refreshTokenFailure({ error: error.message || 'Erro ao renovar token' }))
          )
        )
      )
    )
  );

  refreshTokenFailure$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.refreshTokenFailure),
        tap(() => {
          this.router.navigate(['/auth/login']);
        })
      ),
    { dispatch: false }
  );
}