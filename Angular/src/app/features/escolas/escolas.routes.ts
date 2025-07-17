import { Routes } from '@angular/router';

export const ESCOLAS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/escolas-list/escolas-list.component').then(m => m.EscolasListComponent)
  },
  {
    path: 'nova',
    loadComponent: () => import('./pages/escola-form/escola-form.component').then(m => m.EscolaFormComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./pages/escola-detail/escola-detail.component').then(m => m.EscolaDetailComponent)
  },
  {
    path: ':id/editar',
    loadComponent: () => import('./pages/escola-form/escola-form.component').then(m => m.EscolaFormComponent)
  }
];