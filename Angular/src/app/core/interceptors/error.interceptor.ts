import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, filter, take, switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(
    private authService: AuthService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && !req.url.includes('/auth/login') && !req.url.includes('/auth/refresh')) {
          return this.handle401Error(req, next);
        }

        return this.handleOtherErrors(error);
      })
    );
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      const refreshToken = this.authService.getRefreshToken();
      
      if (refreshToken) {
        return this.authService.refreshToken().pipe(
          switchMap((authResponse: any) => {
            this.isRefreshing = false;
            this.refreshTokenSubject.next(authResponse.accessToken);
            
            // Retry the original request with new token
            const authReq = request.clone({
              setHeaders: {
                Authorization: `Bearer ${authResponse.accessToken}`
              }
            });
            
            return next.handle(authReq);
          }),
          catchError((refreshError) => {
            this.isRefreshing = false;
            this.authService.logout().subscribe();
            this.router.navigate(['/login']);
            return throwError(() => refreshError);
          })
        );
      } else {
        this.isRefreshing = false;
        this.authService.logout().subscribe();
        this.router.navigate(['/login']);
        return throwError(() => new Error('No refresh token available'));
      }
    } else {
      // If refresh is in progress, wait for it to complete
      return this.refreshTokenSubject.pipe(
        filter(token => token != null),
        take(1),
        switchMap(token => {
          const authReq = request.clone({
            setHeaders: {
              Authorization: `Bearer ${token}`
            }
          });
          return next.handle(authReq);
        })
      );
    }
  }

  private handleOtherErrors(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'Ocorreu um erro inesperado';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Erro: ${error.error.message}`;
    } else {
      // Server-side error
      switch (error.status) {
        case 400:
          errorMessage = this.extractErrorMessage(error) || 'Dados inválidos';
          break;
        case 401:
          errorMessage = 'Não autorizado. Faça login novamente.';
          break;
        case 403:
          errorMessage = 'Acesso negado. Você não tem permissão para esta ação.';
          break;
        case 404:
          errorMessage = 'Recurso não encontrado';
          break;
        case 422:
          errorMessage = this.extractErrorMessage(error) || 'Dados de entrada inválidos';
          break;
        case 429:
          errorMessage = 'Muitas tentativas. Tente novamente em alguns minutos.';
          break;
        case 500:
          errorMessage = 'Erro interno do servidor. Tente novamente mais tarde.';
          break;
        case 503:
          errorMessage = 'Serviço temporariamente indisponível';
          break;
        default:
          errorMessage = this.extractErrorMessage(error) || `Erro ${error.status}: ${error.statusText}`;
      }
    }

    // Show error notification (except for 401 to avoid spam during logout)
    if (error.status !== 401) {
      this.notificationService.error(errorMessage);
    }

    return throwError(() => error);
  }

  private extractErrorMessage(error: HttpErrorResponse): string | null {
    if (error.error?.detail) {
      return error.error.detail;
    }
    
    if (error.error?.message) {
      return error.error.message;
    }
    
    if (error.error?.title) {
      return error.error.title;
    }
    
    if (typeof error.error === 'string') {
      return error.error;
    }
    
    return null;
  }
}