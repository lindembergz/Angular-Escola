import { Routes } from '@angular/router';

export const authRoutes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then(c => c.LoginComponent),
    title: 'Login - Sistema de Gestão Escolar'
  },
  {
    path: 'forgot-password',
    loadComponent: () => import('./pages/forgot-password/forgot-password.component').then(c => c.ForgotPasswordComponent),
    title: 'Recuperar Senha - Sistema de Gestão Escolar'
  },
  {
    path: 'reset-password',
    loadComponent: () => import('./pages/reset-password/reset-password.component').then(c => c.ResetPasswordComponent),
    title: 'Redefinir Senha - Sistema de Gestão Escolar'
  },
  {
    path: 'confirm-email',
    loadComponent: () => import('./pages/confirm-email/confirm-email.component').then(c => c.ConfirmEmailComponent),
    title: 'Confirmar Email - Sistema de Gestão Escolar'
  }
];