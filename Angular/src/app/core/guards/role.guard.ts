import { Injectable, inject } from '@angular/core';
import { CanActivate, CanActivateChild, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, map, take } from 'rxjs';
import { AuthState } from '../../store/auth/auth.reducer';
import * as AuthSelectors from '../../store/auth/auth.selectors';
import { NotificationService } from '../services/notification.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate, CanActivateChild {
  private store = inject(Store<AuthState>);
  private router = inject(Router);
  private notificationService = inject(NotificationService);

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.checkRole(route);
  }

  canActivateChild(
    childRoute: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.checkRole(childRoute);
  }

  private checkRole(route: ActivatedRouteSnapshot): Observable<boolean> {
    const requiredRoles = route.data['roles'] as string[];
    const requiredPermissions = route.data['permissions'] as string[];

    return this.store.select(AuthSelectors.selectCurrentUser).pipe(
      take(1),
      map(user => {
        if (!user) {
          this.router.navigate(['/auth/login']);
          return false;
        }

        // Check roles
        if (requiredRoles && requiredRoles.length > 0) {
          const hasRole = requiredRoles.includes(user.codigoPerfil);
          if (!hasRole) {
            this.notificationService.error('Você não tem permissão para acessar esta página.');
            this.router.navigate(['/dashboard']);
            return false;
          }
        }

        // Check permissions
        if (requiredPermissions && requiredPermissions.length > 0) {
          const userPermissions = user.permissoes || [];
          const hasPermission = requiredPermissions.some(permission => 
            userPermissions.includes(permission)
          );
          if (!hasPermission) {
            this.notificationService.error('Você não tem permissão para acessar esta página.');
            this.router.navigate(['/dashboard']);
            return false;
          }
        }

        return true;
      })
    );
  }
}