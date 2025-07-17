import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private authService: AuthService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Ocorreu um erro inesperado';

        if (error.error instanceof ErrorEvent) {
          // Client-side error
          errorMessage = `Erro: ${error.error.message}`;
        } else {
          // Server-side error
          switch (error.status) {
            case 400:
              errorMessage = error.error?.message || 'Dados inválidos';
              break;
            case 401:
              errorMessage = 'Não autorizado. Faça login novamente.';
              this.authService.logout();
              this.router.navigate(['/login']);
              break;
            case 403:
              errorMessage = 'Acesso negado. Você não tem permissão para esta ação.';
              break;
            case 404:
              errorMessage = 'Recurso não encontrado';
              break;
            case 422:
              errorMessage = error.error?.message || 'Dados de entrada inválidos';
              break;
            case 500:
              errorMessage = 'Erro interno do servidor. Tente novamente mais tarde.';
              break;
            case 503:
              errorMessage = 'Serviço temporariamente indisponível';
              break;
            default:
              errorMessage = error.error?.message || `Erro ${error.status}: ${error.statusText}`;
          }
        }

        // Show error notification (except for 401 to avoid spam during logout)
        if (error.status !== 401) {
          this.notificationService.error(errorMessage);
        }

        return throwError(() => error);
      })
    );
  }
}