import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { LazyLoadingGuard } from './shared/guards/lazy-loading.guard';

export const routes: Routes = [
  // Rota padrão - Redireciona para dashboard se autenticado, senão para login
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },

  // Módulo de Autenticação - Lazy Loading (sem guard)
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes')
      .then(m => m.authRoutes)
  },
  
  // Dashboard - Protegido por AuthGuard
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component')
      .then(m => m.DashboardComponent)
      .catch(error => {
        console.error('Error loading dashboard:', error);
        return import('./shared/components/lazy-loading-error/lazy-loading-error.component')
          .then(m => m.LazyLoadingErrorComponent);
      }),
    canActivate: [AuthGuard, LazyLoadingGuard]
  },
  
  // Módulo de Escolas - Lazy Loading
  {
    path: 'escolas',
    loadChildren: () => import('./features/escolas/escolas.routes')
      .then(m => m.ESCOLAS_ROUTES),
    canActivate: [AuthGuard, LazyLoadingGuard]
  },
  
  // Módulo de Alunos - Lazy Loading
  {
    path: 'alunos',
    loadChildren: () => import('./features/alunos/alunos.routes')
      .then(m => m.ALUNOS_ROUTES),
    canActivate: [AuthGuard, LazyLoadingGuard]
  },
  
  // Módulo de Professores - Lazy Loading
  {
    path: 'professores',
    loadChildren: () => import('./features/professores/professores.routes')
      .then(m => m.PROFESSORES_ROUTES),
    canActivate: [AuthGuard, LazyLoadingGuard]
  },
  
  // Módulo Acadêmico - Lazy Loading
  {
    path: 'academico',
    loadChildren: () => import('./features/academico/academico.routes')
      .then(m => m.ACADEMICO_ROUTES),
    canActivate: [AuthGuard, LazyLoadingGuard]
  },
  
  // Módulo de Avaliações - Lazy Loading
  {
    path: 'avaliacoes',
    loadChildren: () => import('./features/avaliacoes/avaliacoes.routes')
      .then(m => m.AVALIACOES_ROUTES),
    canActivate: [AuthGuard, LazyLoadingGuard]
  },
  
  // Módulo Financeiro - Lazy Loading
  {
    path: 'financeiro',
    loadChildren: () => import('./features/financeiro/financeiro.routes')
      .then(m => m.FINANCEIRO_ROUTES),
    canActivate: [AuthGuard, LazyLoadingGuard]
  },
  
  // Módulo de Relatórios - Lazy Loading
  {
    path: 'relatorios',
    loadChildren: () => import('./features/relatorios/relatorios.routes')
      .then(m => m.RELATORIOS_ROUTES),
    canActivate: [AuthGuard, LazyLoadingGuard]
  },
  
  // Rota 404 - Página não encontrada
  {
    path: '**',
    redirectTo: ''
  }
];
