import { Routes } from '@angular/router';

export const FINANCEIRO_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'mensalidades',
    pathMatch: 'full'
  },
  {
    path: 'mensalidades',
    loadComponent: () => import('./pages/mensalidades-list/mensalidades-list.component').then(m => m.MensalidadesListComponent)
  },
  {
    path: 'mensalidades/nova',
    loadComponent: () => import('./pages/mensalidade-form/mensalidade-form.component').then(m => m.MensalidadeFormComponent)
  },
  {
    path: 'pagamentos',
    loadComponent: () => import('./pages/pagamentos-list/pagamentos-list.component').then(m => m.PagamentosListComponent)
  },
  {
    path: 'pagamentos/novo',
    loadComponent: () => import('./pages/pagamento-form/pagamento-form.component').then(m => m.PagamentoFormComponent)
  }
];