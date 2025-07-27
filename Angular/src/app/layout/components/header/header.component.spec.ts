import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';
import { HeaderComponent } from './header.component';
import { AuthService } from '../../../features/auth/services/auth.service';
import { User } from '../../../features/auth/models/auth.models';
import * as AuthActions from '../../../store/auth/auth.actions';

describe('HeaderComponent - Navigation Tests', () => {
  let component: HeaderComponent;
  let fixture: ComponentFixture<HeaderComponent>;
  let router: jasmine.SpyObj<Router>;
  let authService: jasmine.SpyObj<AuthService>;
  let store: jasmine.SpyObj<Store>;

  const mockUser: User = {
    id: '1',
    primeiroNome: 'Test',
    ultimoNome: 'User',
    nomeCompleto: 'Test User',
    email: 'test@example.com',
    codigoPerfil: 'admin',
    nomePerfil: 'Administrador',
    nivelPerfil: 1,
    iniciais: 'TU',
    ativo: true,
    emailConfirmado: true
  };

  beforeEach(async () => {
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    const authServiceSpy = jasmine.createSpyObj('AuthService', [], {
      currentUser$: of(mockUser)
    });
    const storeSpy = jasmine.createSpyObj('Store', ['dispatch']);

    await TestBed.configureTestingModule({
      imports: [HeaderComponent],
      providers: [
        { provide: Router, useValue: routerSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Store, useValue: storeSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HeaderComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    store = TestBed.inject(Store) as jasmine.SpyObj<Store>;
    
    fixture.detectChanges();
  });

  describe('navigateToHome', () => {
    it('should navigate to home page when called', () => {
      // Act
      component.navigateToHome();

      // Assert
      expect(router.navigate).toHaveBeenCalledWith(['/']);
    });

    it('should navigate to home when title is clicked', () => {
      // Arrange
      const titleElement = fixture.debugElement.nativeElement.querySelector('.logo-text');
      
      // Act
      titleElement?.click();

      // Assert
      expect(router.navigate).toHaveBeenCalledWith(['/']);
    });

    it('should have cursor pointer style on title element', () => {
      // Arrange
      const titleElement = fixture.debugElement.nativeElement.querySelector('.logo-text');
      
      // Assert
      expect(titleElement).toBeTruthy();
      expect(titleElement.classList).toContain('cursor-pointer');
    });
  });

  describe('logout functionality', () => {
    it('should dispatch logout action when logout is called', () => {
      // Act
      component['logout']();

      // Assert
      expect(store.dispatch).toHaveBeenCalledWith(AuthActions.logout());
    });

    it('should call logout when logout menu item is clicked', () => {
      // Arrange
      spyOn(component as any, 'logout');
      
      // Act
      const logoutMenuItem = component.userMenuItems.find(item => item.label === 'Sair');
      logoutMenuItem?.command?.({} as any);

      // Assert
      expect(component['logout']).toHaveBeenCalled();
    });
  });

  describe('sidebar toggle', () => {
    it('should emit toggleSidebar event when onToggleSidebar is called', () => {
      // Arrange
      spyOn(component.toggleSidebar, 'emit');

      // Act
      component.onToggleSidebar();

      // Assert
      expect(component.toggleSidebar.emit).toHaveBeenCalled();
    });
  });

  describe('user menu setup', () => {
    it('should setup user menu items correctly', () => {
      // Assert
      expect(component.userMenuItems.length).toBe(4);
      expect(component.userMenuItems[0].label).toBe('Perfil');
      expect(component.userMenuItems[1].label).toBe('Configurações');
      expect(component.userMenuItems[2].separator).toBe(true);
      expect(component.userMenuItems[3].label).toBe('Sair');
    });

    it('should navigate to profile when profile menu item is clicked', () => {
      // Act
      const profileMenuItem = component.userMenuItems.find(item => item.label === 'Perfil');
      profileMenuItem?.command?.({} as any);

      // Assert
      expect(router.navigate).toHaveBeenCalledWith(['/profile']);
    });

    it('should navigate to settings when settings menu item is clicked', () => {
      // Act
      const settingsMenuItem = component.userMenuItems.find(item => item.label === 'Configurações');
      settingsMenuItem?.command?.({} as any);

      // Assert
      expect(router.navigate).toHaveBeenCalledWith(['/settings']);
    });
  });

  describe('current user handling', () => {
    it('should set current user from auth service', () => {
      // Assert
      expect(component.currentUser).toEqual(mockUser);
    });

    it('should update user menu when current user changes', () => {
      // Arrange
      const newUser: User = {
        id: '2',
        primeiroNome: 'New',
        ultimoNome: 'User',
        nomeCompleto: 'New User',
        email: 'new@example.com',
        codigoPerfil: 'user',
        nomePerfil: 'Usuário',
        nivelPerfil: 2,
        iniciais: 'NU',
        ativo: true,
        emailConfirmado: true
      };
      
      // Act
      (authService as any).currentUser$ = of(newUser);
      // Simulate constructor call again
      component['setupUserMenu']();

      // Assert
      expect(component.currentUser).toEqual(newUser);
    });
  });
});