/**
 * Functional Validation Tests
 * Comprehensive tests to ensure all optimizations maintain application functionality
 */

import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { Component } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from '../app/app.routes';

describe('Functional Validation After Optimizations', () => {
  let router: Router;
  let location: Location;
  let fixture: any;

  beforeEach(async () => {
    @Component({
      template: '<router-outlet></router-outlet>'
    })
    class TestComponent { }

    await TestBed.configureTestingModule({
      imports: [TestComponent],
      providers: [
        provideRouter(routes)
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    location = TestBed.inject(Location);
    fixture = TestBed.createComponent(TestComponent);
    fixture.detectChanges();
  });

  describe('Route Navigation and Lazy Loading', () => {
    it('should navigate to dashboard route', async () => {
      await router.navigate(['']);
      expect(location.path()).toBe('');
    });

    it('should navigate to dashboard explicitly', async () => {
      await router.navigate(['/dashboard']);
      expect(location.path()).toBe('/dashboard');
    });

    it('should handle lazy loaded escolas route', async () => {
      await router.navigate(['/escolas']);
      expect(location.path()).toBe('/escolas');
    });

    it('should handle lazy loaded alunos route', async () => {
      await router.navigate(['/alunos']);
      expect(location.path()).toBe('/alunos');
    });

    it('should handle lazy loaded professores route', async () => {
      await router.navigate(['/professores']);
      expect(location.path()).toBe('/professores');
    });

    it('should handle lazy loaded academico route', async () => {
      await router.navigate(['/academico']);
      expect(location.path()).toBe('/academico');
    });

    it('should handle lazy loaded avaliacoes route', async () => {
      await router.navigate(['/avaliacoes']);
      expect(location.path()).toBe('/avaliacoes');
    });

    it('should handle lazy loaded financeiro route', async () => {
      await router.navigate(['/financeiro']);
      expect(location.path()).toBe('/financeiro');
    });

    it('should handle lazy loaded relatorios route', async () => {
      await router.navigate(['/relatorios']);
      expect(location.path()).toBe('/relatorios');
    });

    it('should redirect unknown routes to dashboard', async () => {
      await router.navigate(['/unknown-route']);
      expect(location.path()).toBe('');
    });
  });

  describe('Lazy Loading Error Handling', () => {
    it('should handle lazy loading failures gracefully', () => {
      // Test that routes have error handling configured
      const routesWithErrorHandling = routes.filter(route => 
        route.loadComponent && route.loadComponent.toString().includes('catch')
      );
      
      expect(routesWithErrorHandling.length).toBeGreaterThan(0);
    });

    it('should have lazy loading guard configured', () => {
      const routesWithGuard = routes.filter(route => 
        route.canActivate && route.canActivate.length > 0
      );
      
      expect(routesWithGuard.length).toBeGreaterThan(0);
    });
  });

  describe('Route Configuration Validation', () => {
    it('should have all expected routes configured', () => {
      const expectedPaths = [
        '', 'dashboard', 'escolas', 'alunos', 'professores', 
        'academico', 'avaliacoes', 'financeiro', 'relatorios'
      ];
      
      const configuredPaths = routes
        .filter(route => route.path !== '**')
        .map(route => route.path);
      
      expectedPaths.forEach(path => {
        expect(configuredPaths).toContain(path);
      });
    });

    it('should have wildcard route for 404 handling', () => {
      const wildcardRoute = routes.find(route => route.path === '**');
      expect(wildcardRoute).toBeDefined();
      expect(wildcardRoute?.redirectTo).toBe('');
    });
  });
});