import { Routes } from '@angular/router';

export const ACADEMICO_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'turmas',
    pathMatch: 'full'
  },
  {
    path: 'turmas',
    loadComponent: () => import('./pages/turmas-list/turmas-list.component').then(m => m.TurmasListComponent)
  },
  {
    path: 'turmas/nova',
    loadComponent: () => import('./pages/turma-form/turma-form.component').then(m => m.TurmaFormComponent)
  },
  {
    path: 'turmas/:id',
    loadComponent: () => import('./pages/turma-detail/turma-detail.component').then(m => m.TurmaDetailComponent)
  },
  {
    path: 'turmas/:id/editar',
    loadComponent: () => import('./pages/turma-form/turma-form.component').then(m => m.TurmaFormComponent)
  },
  {
    path: 'disciplinas',
    loadComponent: () => import('./pages/disciplinas-list/disciplinas-list.component').then(m => m.DisciplinasListComponent)
  },
  {
    path: 'disciplinas/nova',
    loadComponent: () => import('./pages/disciplina-form/disciplina-form.component').then(m => m.DisciplinaFormComponent)
  },
  {
    path: 'horarios',
    loadComponent: () => import('./pages/horarios-list/horarios-list.component').then(m => m.HorariosListComponent)
  }
];