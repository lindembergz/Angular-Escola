import { Routes } from '@angular/router';

export const RELATORIOS_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'academicos',
    pathMatch: 'full'
  },
  {
    path: 'academicos',
    loadComponent: () => import('./pages/relatorios-academicos/relatorios-academicos.component').then(m => m.RelatoriosAcademicosComponent)
  },
  {
    path: 'financeiros',
    loadComponent: () => import('./pages/relatorios-financeiros/relatorios-financeiros.component').then(m => m.RelatoriosFinanceirosComponent)
  }
];