import { Routes } from '@angular/router';
import { LazyLoadingGuard } from '../../shared/guards/lazy-loading.guard';

export const ALUNOS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/alunos-list/alunos-list.component')
      .then(m => m.AlunosListComponent)
      .catch(error => {
        console.error('Error loading alunos-list component:', error);
        return import('../../shared/components/lazy-loading-error/lazy-loading-error.component')
          .then(m => m.LazyLoadingErrorComponent);
      }),
    canActivate: [LazyLoadingGuard]
  },
  {
    path: 'novo',
    loadComponent: () => import('./pages/aluno-form/aluno-form.component')
      .then(m => m.AlunoFormComponent)
      .catch(error => {
        console.error('Error loading aluno-form component:', error);
        return import('../../shared/components/lazy-loading-error/lazy-loading-error.component')
          .then(m => m.LazyLoadingErrorComponent);
      }),
    canActivate: [LazyLoadingGuard]
  },
  {
    path: ':id',
    loadComponent: () => import('./pages/aluno-detail/aluno-detail.component')
      .then(m => m.AlunoDetailComponent)
      .catch(error => {
        console.error('Error loading aluno-detail component:', error);
        return import('../../shared/components/lazy-loading-error/lazy-loading-error.component')
          .then(m => m.LazyLoadingErrorComponent);
      }),
    canActivate: [LazyLoadingGuard]
  },
  {
    path: ':id/editar',
    loadComponent: () => import('./pages/aluno-form/aluno-form.component')
      .then(m => m.AlunoFormComponent)
      .catch(error => {
        console.error('Error loading aluno-form component:', error);
        return import('../../shared/components/lazy-loading-error/lazy-loading-error.component')
          .then(m => m.LazyLoadingErrorComponent);
      }),
    canActivate: [LazyLoadingGuard]
  }
];