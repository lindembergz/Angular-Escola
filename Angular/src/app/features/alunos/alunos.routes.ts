import { Routes } from '@angular/router';
import { LazyLoadingGuard } from '../../shared/guards/lazy-loading.guard';
import { Type } from '@angular/core';
import { LazyLoadingErrorComponent } from '../../shared/components/lazy-loading-error/lazy-loading-error.component';

/**
 * Envolve uma promise de carregamento de componente com um tratador de erros.
 * Se a promise for rejeitada, registra o erro e carrega um componente de fallback.
 * @param componentPromise A promise retornada pelo import dinâmico.
 * @returns Uma nova promise que resolve para o componente ou para o componente de erro.
 */
function withLazyLoadErrorHandler<T>(componentPromise: Promise<T>): Promise<T | Type<LazyLoadingErrorComponent>> {
  return componentPromise.catch(error => {
    console.error('Error loading component:', error);
    return import('../../shared/components/lazy-loading-error/lazy-loading-error.component')
      .then(m => m.LazyLoadingErrorComponent);
  });
}

export const ALUNOS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => withLazyLoadErrorHandler(
      import('./pages/alunos-list/alunos-list.component').then(m => m.AlunosListComponent)
    ),
    canActivate: [LazyLoadingGuard]
  },
  {
    path: 'novo',
    loadComponent: () => withLazyLoadErrorHandler(
      import('./pages/aluno-form/aluno-form.component').then(m => m.AlunoFormComponent)
    ),
    canActivate: [LazyLoadingGuard]
  },
  {
    path: ':id',
    loadComponent: () => withLazyLoadErrorHandler(
      import('./pages/aluno-detail/aluno-detail.component').then(m => m.AlunoDetailComponent)
    ),
    canActivate: [LazyLoadingGuard]
  },
  {
    path: ':id/editar',
    loadComponent: () => withLazyLoadErrorHandler(
      import('./pages/aluno-form/aluno-form.component').then(m => m.AlunoFormComponent)
    ),
    canActivate: [LazyLoadingGuard]
  }
];