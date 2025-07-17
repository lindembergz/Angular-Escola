import { Routes } from '@angular/router';

export const AVALIACOES_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'notas',
    pathMatch: 'full'
  },
  {
    path: 'notas',
    loadComponent: () => import('./pages/notas-list/notas-list.component').then(m => m.NotasListComponent)
  },
  {
    path: 'notas/nova',
    loadComponent: () => import('./pages/nota-form/nota-form.component').then(m => m.NotaFormComponent)
  },
  {
    path: 'frequencia',
    loadComponent: () => import('./pages/frequencia-list/frequencia-list.component').then(m => m.FrequenciaListComponent)
  },
  {
    path: 'frequencia/nova',
    loadComponent: () => import('./pages/frequencia-form/frequencia-form.component').then(m => m.FrequenciaFormComponent)
  }
];