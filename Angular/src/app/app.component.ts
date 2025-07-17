import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterOutlet, Router, NavigationStart, NavigationEnd, NavigationError } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Menubar } from 'primeng/menubar';
import { Button } from 'primeng/button';
import { MenuItem } from 'primeng/api';
import { Message } from 'primeng/message';
import { Subject } from 'rxjs';
import { LoadingSpinnerComponent } from './shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule, Menubar, Button, Message, LoadingSpinnerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Sistema de Gestão Escolar';
  isNavigating = false;
  errorMessage: string | null = null;
  
  private destroy$ = new Subject<void>();
  
  constructor(
    private router: Router
  ) {}

  ngOnInit() {
    // Listen to router events for loading states
    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        this.isNavigating = true;
        this.errorMessage = null;
      } else if (event instanceof NavigationEnd) {
        this.isNavigating = false;
      } else if (event instanceof NavigationError) {
        this.isNavigating = false;
        this.handleNavigationError(event);
      }
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  hasActiveRoute(): boolean {
    // Sempre retorna true pois agora temos o router-outlet funcionando
    return true;
  }

  private handleNavigationError(event: NavigationError) {
    console.error('Navigation error:', event.error);
    this.errorMessage = 'Erro ao carregar página. Redirecionando para o dashboard...';
    setTimeout(() => {
      this.router.navigate(['/']);
    }, 2000);
  }

  dismissError() {
    this.errorMessage = null;
  }
  
  menuItems: MenuItem[] = [
    {
      label: 'Dashboard',
      icon: 'pi pi-home',
      routerLink: ['/']
    },
    {
      label: 'Escolas',
      icon: 'pi pi-building',
      items: [
        {
          label: 'Listar Escolas',
          icon: 'pi pi-list',
          routerLink: ['/escolas']
        },
        {
          label: 'Nova Escola',
          icon: 'pi pi-plus',
          routerLink: ['/escolas/nova']
        }
      ]
    },
    {
      label: 'Alunos',
      icon: 'pi pi-users',
      items: [
        {
          label: 'Listar Alunos',
          icon: 'pi pi-list',
          routerLink: ['/alunos']
        },
        {
          label: 'Novo Aluno',
          icon: 'pi pi-user-plus',
          routerLink: ['/alunos/novo']
        }
      ]
    },
    {
      label: 'Professores',
      icon: 'pi pi-user',
      items: [
        {
          label: 'Listar Professores',
          icon: 'pi pi-list',
          routerLink: ['/professores']
        },
        {
          label: 'Novo Professor',
          icon: 'pi pi-user-plus',
          routerLink: ['/professores/novo']
        }
      ]
    },
    {
      label: 'Acadêmico',
      icon: 'pi pi-book',
      items: [
        {
          label: 'Turmas',
          icon: 'pi pi-users',
          routerLink: ['/academico/turmas']
        },
        {
          label: 'Disciplinas',
          icon: 'pi pi-bookmark',
          routerLink: ['/academico/disciplinas']
        },
        {
          label: 'Horários',
          icon: 'pi pi-calendar',
          routerLink: ['/academico/horarios']
        }
      ]
    },
    {
      label: 'Avaliações',
      icon: 'pi pi-chart-bar',
      items: [
        {
          label: 'Notas',
          icon: 'pi pi-star',
          routerLink: ['/avaliacoes/notas']
        },
        {
          label: 'Frequência',
          icon: 'pi pi-check-circle',
          routerLink: ['/avaliacoes/frequencia']
        }
      ]
    },
    {
      label: 'Financeiro',
      icon: 'pi pi-dollar',
      items: [
        {
          label: 'Mensalidades',
          icon: 'pi pi-credit-card',
          routerLink: ['/financeiro/mensalidades']
        },
        {
          label: 'Pagamentos',
          icon: 'pi pi-money-bill',
          routerLink: ['/financeiro/pagamentos']
        }
      ]
    },
    {
      label: 'Relatórios',
      icon: 'pi pi-file',
      items: [
        {
          label: 'Acadêmicos',
          icon: 'pi pi-chart-line',
          routerLink: ['/relatorios/academicos']
        },
        {
          label: 'Financeiros',
          icon: 'pi pi-chart-pie',
          routerLink: ['/relatorios/financeiros']
        }
      ]
    }
  ];
}
