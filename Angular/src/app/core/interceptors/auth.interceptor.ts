import { Injectable, inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap, take } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { AuthState } from '../../store/auth/auth.reducer';
import * as AuthSelectors from '../../store/auth/auth.selectors';
import * as AuthActions from '../../store/auth/auth.actions';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private store = inject(Store<AuthState>);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return this.store.select(AuthSelectors.selectAuthToken).pipe(
      take(1),
      switchMap(token => {
        // Clone request with auth header if token exists
        const authReq = token ? req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`
          }
        }) : req;

        return next.handle(authReq).pipe(
          catchError((error: HttpErrorResponse) => {
            // Handle 401 Unauthorized errors
            if (error.status === 401 && token) {
              // Try to refresh token
              this.store.dispatch(AuthActions.refreshToken());
            }
            
            return throwError(() => error);
          })
        );
      })
    );
  }
}