import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { provideMockActions } from '@ngrx/effects/testing';
import { Observable, of, throwError } from 'rxjs';
import { AuthEffects } from './auth.effects';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import * as AuthActions from './auth.actions';

describe('AuthEffects - Logout Flow', () => {
  let effects: AuthEffects;
  let actions$: Observable<any>;
  let authService: jasmine.SpyObj<AuthService>;
  let notificationService: jasmine.SpyObj<NotificationService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['logout', 'clearAuthData']);
    const notificationServiceSpy = jasmine.createSpyObj('NotificationService', ['success']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        AuthEffects,
        provideMockActions(() => actions$),
        { provide: AuthService, useValue: authServiceSpy },
        { provide: NotificationService, useValue: notificationServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    });

    effects = TestBed.inject(AuthEffects);
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    notificationService = TestBed.inject(NotificationService) as jasmine.SpyObj<NotificationService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  describe('logoutSuccess$', () => {
    it('should clear auth data, show success message and navigate to login', (done) => {
      // Arrange
      actions$ = of(AuthActions.logoutSuccess());

      // Act
      effects.logoutSuccess$.subscribe(() => {
        // Assert
        expect(authService.clearAuthData).toHaveBeenCalled();
        expect(notificationService.success).toHaveBeenCalledWith('Logout realizado com sucesso');
        expect(router.navigate).toHaveBeenCalledWith(['/auth/login']);
        done();
      });
    });
  });

  describe('logout$', () => {
    it('should call logout service and dispatch logoutSuccess', (done) => {
      // Arrange
      authService.logout.and.returnValue(of({}));
      actions$ = of(AuthActions.logout());

      // Act
      effects.logout$.subscribe(action => {
        // Assert
        expect(authService.logout).toHaveBeenCalled();
        expect(action).toEqual(AuthActions.logoutSuccess());
        done();
      });
    });

    it('should clear auth data and dispatch logoutSuccess even if API fails', (done) => {
      // Arrange
      authService.logout.and.returnValue(throwError(() => new Error('API Error')));
      actions$ = of(AuthActions.logout());

      // Act
      effects.logout$.subscribe(action => {
        // Assert
        expect(authService.clearAuthData).toHaveBeenCalled();
        expect(action).toEqual(AuthActions.logoutSuccess());
        done();
      });
    });

    it('should handle network errors gracefully', (done) => {
      // Arrange
      authService.logout.and.returnValue(throwError(() => new Error('Network Error')));
      actions$ = of(AuthActions.logout());

      // Act
      effects.logout$.subscribe(action => {
        // Assert
        expect(authService.clearAuthData).toHaveBeenCalled();
        expect(action).toEqual(AuthActions.logoutSuccess());
        done();
      });
    });
  });

  describe('complete logout flow integration', () => {
    it('should complete full logout flow from action to navigation', (done) => {
      // Arrange
      authService.logout.and.returnValue(of({}));
      
      // Test logout action
      actions$ = of(AuthActions.logout());
      
      effects.logout$.subscribe(logoutAction => {
        expect(logoutAction).toEqual(AuthActions.logoutSuccess());
        
        // Test logout success effect
        actions$ = of(AuthActions.logoutSuccess());
        
        effects.logoutSuccess$.subscribe(() => {
          // Assert complete flow
          expect(authService.logout).toHaveBeenCalled();
          expect(authService.clearAuthData).toHaveBeenCalled();
          expect(notificationService.success).toHaveBeenCalledWith('Logout realizado com sucesso');
          expect(router.navigate).toHaveBeenCalledWith(['/auth/login']);
          done();
        });
      });
    });

    it('should ensure header is hidden after logout navigation', (done) => {
      // Arrange
      actions$ = of(AuthActions.logoutSuccess());

      // Act
      effects.logoutSuccess$.subscribe(() => {
        // Assert navigation to login page (where header should be hidden)
        expect(router.navigate).toHaveBeenCalledWith(['/auth/login']);
        
        // Verify auth data is cleared (which should trigger header hiding)
        expect(authService.clearAuthData).toHaveBeenCalled();
        done();
      });
    });
  });
});