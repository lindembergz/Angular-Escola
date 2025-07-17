import { Routes } from '@angular/router';

export const PROFESSORES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/professores-list/professores-list.component').then(m => m.ProfessoresListComponent)
  },
  {
    path: 'novo',
    loadComponent: () => import('./pages/professor-form/professor-form.component').then(m => m.ProfessorFormComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./pages/professor-detail/professor-detail.component').then(m => m.ProfessorDetailComponent)
  },
  {
    path: ':id/editar',
    loadComponent: () => import('./pages/professor-form/professor-form.component').then(m => m.ProfessorFormComponent)
  }
];