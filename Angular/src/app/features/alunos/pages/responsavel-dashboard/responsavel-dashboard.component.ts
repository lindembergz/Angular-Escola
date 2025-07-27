import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TabViewModule } from 'primeng/tabview';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ChartModule } from 'primeng/chart';
import { TimelineModule } from 'primeng/timeline';
import { BadgeModule } from 'primeng/badge';
import { AvatarModule } from 'primeng/avatar';
import { DividerModule } from 'primeng/divider';

import { AlunosFacade } from '../../../../store/alunos/alunos.facade';
import { AuthFacade } from '../../../../store/auth/auth.facade';
import { AlunoResumo } from '../../models/aluno.model';
import { AlunosService } from '../../services/alunos.service';

interface NotificacaoResponsavel {
  id: string;
  tipo: 'academica' | 'financeira' | 'disciplinar' | 'evento';
  titulo: string;
  descricao: string;
  data: Date;
  lida: boolean;
  prioridade: 'baixa' | 'media' | 'alta';
  alunoId: string;
  nomeAluno: string;
}

interface EventoEscolar {
  id: string;
  titulo: string;
  descricao: string;
  data: Date;
  tipo: 'reuniao' | 'evento' | 'feriado' | 'prova';
  turmaId?: string;
  nomeTurma?: string;
}

@Component({
  selector: 'app-responsavel-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    CardModule,
    ButtonModule,
    TableModule,
    TagModule,
    TabViewModule,
    ProgressSpinnerModule,
    ChartModule,
    TimelineModule,
    BadgeModule,
    AvatarModule,
    DividerModule
  ],
  template: `
    <div class="responsavel-dashboard-container">
      <!-- Header -->
      <div class="flex justify-content-between align-items-center mb-4">
        <div>
          <h1 class="text-3xl font-bold text-900 m-0">Portal do Responsável</h1>
          <p class="text-600 mt-1 mb-0">Acompanhe o desenvolvimento dos seus filhos</p>
        </div>
        <div class="flex align-items-center gap-2">
          <p-badge 
            [value]="notificacoesNaoLidas.length" 
            severity="danger" 
            *ngIf="notificacoesNaoLidas.length > 0">
          </p-badge>
          <p-button 
            icon="pi pi-bell" 
            styleClass="p-button-rounded p-button-text"
            [badge]="notificacoesNaoLidas.length.toString()"
            badgeClass="p-badge-danger"
            *ngIf="notificacoesNaoLidas.length > 0">
          </p-button>
        </div>
      </div>

      <!-- Loading -->
      <div *ngIf="loading$ | async" class="flex justify-content-center p-4">
        <p-progressSpinner></p-progressSpinner>
      </div>

      <!-- Content -->
      <div *ngIf="!(loading$ | async)">
        <!-- Resumo dos Filhos -->
        <div class="grid mb-4">
          <div class="col-12 md:col-4" *ngFor="let aluno of meusFilhos">
            <p-card styleClass="h-full">
              <ng-template pTemplate="header">
                <div class="p-3 bg-primary text-white">
                  <div class="flex align-items-center">
                    <p-avatar 
                      [label]="getInitials(aluno.nome)" 
                      styleClass="mr-2"
                      size="large">
                    </p-avatar>
                    <div>
                      <h3 class="m-0 text-white">{{ aluno.nome }}</h3>
                      <p class="m-0 text-100">{{ aluno.idade }} anos</p>
                    </div>
                  </div>
                </div>
              </ng-template>

              <div class="flex flex-column gap-3">
                <div class="flex justify-content-between align-items-center">
                  <span class="text-600">Status:</span>
                  <p-tag 
                    [value]="aluno.ativo ? 'Ativo' : 'Inativo'"
                    [severity]="aluno.ativo ? 'success' : 'danger'">
                  </p-tag>
                </div>

                <div class="flex justify-content-between align-items-center" *ngIf="aluno.nomeTurmaAtual">
                  <span class="text-600">Turma:</span>
                  <span class="font-medium">{{ aluno.nomeTurmaAtual }}</span>
                </div>

                <div class="flex justify-content-between align-items-center">
                  <span class="text-600">Matrícula:</span>
                  <p-tag 
                    [value]="aluno.possuiMatriculaAtiva ? 'Ativa' : 'Inativa'"
                    [severity]="aluno.possuiMatriculaAtiva ? 'info' : 'warning'">
                  </p-tag>
                </div>

                <p-divider></p-divider>

                <div class="flex gap-2">
                  <p-button 
                    label="Ver Detalhes" 
                    icon="pi pi-eye" 
                    styleClass="p-button-sm flex-1"
                    [routerLink]="['/alunos', aluno.id]">
                  </p-button>{{aluno.id}}
                  <p-button 
                    icon="pi pi-chart-line" 
                    styleClass="p-button-sm p-button-outlined"
                    pTooltip="Ver Notas"
                    (onClick)="verNotas(aluno.id)">
                  </p-button>
                </div>
              </div>
            </p-card>
          </div>
        </div>

        <!-- Dashboard Principal -->
        <p-tabView>
          <!-- Notificações -->
          <p-tabPanel header="Notificações" leftIcon="pi pi-bell">
            <div class="grid">
              <div class="col-12 lg:col-8">
                <p-card>
                  <ng-template pTemplate="header">
                    <div class="p-3">
                      <h3 class="text-lg font-medium text-900 m-0">Notificações Recentes</h3>
                    </div>
                  </ng-template>

                  <div *ngIf="notificacoes.length === 0" class="text-center p-4">
                    <i class="pi pi-bell-slash text-4xl text-400 mb-3"></i>
                    <p class="text-600 mb-0">Nenhuma notificação no momento</p>
                  </div>

                  <p-timeline 
                    [value]="notificacoes" 
                    *ngIf="notificacoes.length > 0"
                    styleClass="customized-timeline">
                    
                    <ng-template pTemplate="marker" let-notificacao>
                      <span 
                        class="custom-marker shadow-2"
                        [ngClass]="{
                          'bg-green-500': notificacao.tipo === 'academica',
                          'bg-blue-500': notificacao.tipo === 'financeira',
                          'bg-orange-500': notificacao.tipo === 'disciplinar',
                          'bg-purple-500': notificacao.tipo === 'evento'
                        }">
                        <i 
                          [ngClass]="{
                            'pi pi-graduation-cap': notificacao.tipo === 'academica',
                            'pi pi-dollar': notificacao.tipo === 'financeira',
                            'pi pi-exclamation-triangle': notificacao.tipo === 'disciplinar',
                            'pi pi-calendar': notificacao.tipo === 'evento'
                          }"
                          class="text-white">
                        </i>
                      </span>
                    </ng-template>

                    <ng-template pTemplate="content" let-notificacao>
                      <p-card 
                        [styleClass]="'shadow-1 ' + (notificacao.lida ? '' : 'border-left-3 border-primary')">
                        <div class="flex justify-content-between align-items-start">
                          <div class="flex-1">
                            <h4 class="mt-0 mb-2">{{ notificacao.titulo }}</h4>
                            <p class="text-600 mb-2">{{ notificacao.descricao }}</p>
                            <div class="flex align-items-center gap-2">
                              <small class="text-500">{{ notificacao.data | date:'dd/MM/yyyy HH:mm' }}</small>
                              <p-tag 
                                [value]="notificacao.nomeAluno"
                                severity="info"
                                styleClass="text-xs">
                              </p-tag>
                            </div>
                          </div>
                          <div class="flex flex-column align-items-end gap-2">
                            <p-tag 
                              [value]="getPrioridadeLabel(notificacao.prioridade)"
                              [severity]="getPrioridadeSeverity(notificacao.prioridade)"
                              styleClass="text-xs">
                            </p-tag>
                            <p-button 
                              *ngIf="!notificacao.lida"
                              icon="pi pi-check" 
                              styleClass="p-button-rounded p-button-text p-button-sm"
                              pTooltip="Marcar como lida"
                              (onClick)="marcarComoLida(notificacao.id)">
                            </p-button>
                          </div>
                        </div>
                      </p-card>
                    </ng-template>
                  </p-timeline>
                </p-card>
              </div>

              <div class="col-12 lg:col-4">
                <p-card>
                  <ng-template pTemplate="header">
                    <div class="p-3">
                      <h3 class="text-lg font-medium text-900 m-0">Resumo</h3>
                    </div>
                  </ng-template>

                  <div class="flex flex-column gap-3">
                    <div class="flex justify-content-between align-items-center p-3 border-round surface-100">
                      <div>
                        <div class="text-2xl font-bold text-900">{{ notificacoesNaoLidas.length }}</div>
                        <div class="text-600">Não Lidas</div>
                      </div>
                      <i class="pi pi-bell text-2xl text-orange-500"></i>
                    </div>

                    <div class="flex justify-content-between align-items-center p-3 border-round surface-100">
                      <div>
                        <div class="text-2xl font-bold text-900">{{ eventosProximos.length }}</div>
                        <div class="text-600">Eventos Próximos</div>
                      </div>
                      <i class="pi pi-calendar text-2xl text-blue-500"></i>
                    </div>

                    <div class="flex justify-content-between align-items-center p-3 border-round surface-100">
                      <div>
                        <div class="text-2xl font-bold text-900">{{ meusFilhos.length }}</div>
                        <div class="text-600">Filhos Cadastrados</div>
                      </div>
                      <i class="pi pi-users text-2xl text-green-500"></i>
                    </div>
                  </div>
                </p-card>
              </div>
            </div>
          </p-tabPanel>

          <!-- Calendário Escolar -->
          <p-tabPanel header="Calendário" leftIcon="pi pi-calendar">
            <p-card>
              <ng-template pTemplate="header">
                <div class="p-3">
                  <h3 class="text-lg font-medium text-900 m-0">Próximos Eventos</h3>
                </div>
              </ng-template>

              <div *ngIf="eventosProximos.length === 0" class="text-center p-4">
                <i class="pi pi-calendar text-4xl text-400 mb-3"></i>
                <p class="text-600 mb-0">Nenhum evento próximo</p>
              </div>

              <div class="grid" *ngIf="eventosProximos.length > 0">
                <div class="col-12 md:col-6 lg:col-4" *ngFor="let evento of eventosProximos">
                  <p-card styleClass="h-full">
                    <div class="flex align-items-start gap-3">
                      <div class="flex flex-column align-items-center">
                        <div class="bg-primary text-white p-2 border-round text-center" style="min-width: 60px;">
                          <div class="font-bold">{{ evento.data | date:'dd' }}</div>
                          <div class="text-xs">{{ evento.data | date:'MMM' | uppercase }}</div>
                        </div>
                      </div>
                      <div class="flex-1">
                        <h4 class="mt-0 mb-2">{{ evento.titulo }}</h4>
                        <p class="text-600 mb-2 text-sm">{{ evento.descricao }}</p>
                        <div class="flex align-items-center gap-2">
                          <p-tag 
                            [value]="getTipoEventoLabel(evento.tipo)"
                            [severity]="getTipoEventoSeverity(evento.tipo)"
                            styleClass="text-xs">
                          </p-tag>
                          <small class="text-500" *ngIf="evento.nomeTurma">{{ evento.nomeTurma }}</small>
                        </div>
                      </div>
                    </div>
                  </p-card>
                </div>
              </div>
            </p-card>
          </p-tabPanel>

          <!-- Comunicados -->
          <p-tabPanel header="Comunicados" leftIcon="pi pi-megaphone">
            <p-card>
              <ng-template pTemplate="header">
                <div class="p-3">
                  <h3 class="text-lg font-medium text-900 m-0">Comunicados da Escola</h3>
                </div>
              </ng-template>

              <div class="text-center p-4">
                <i class="pi pi-megaphone text-4xl text-400 mb-3"></i>
                <p class="text-600 mb-0">Funcionalidade em desenvolvimento</p>
                <p class="text-500 text-sm">Em breve você poderá visualizar todos os comunicados da escola</p>
              </div>
            </p-card>
          </p-tabPanel>
        </p-tabView>
      </div>
    </div>
  `,
  styles: [`
    .responsavel-dashboard-container {
      padding: 1rem;
    }

    :host ::ng-deep .customized-timeline .p-timeline-event-content {
      padding: 0 1rem;
    }

    :host ::ng-deep .custom-marker {
      display: flex;
      width: 2rem;
      height: 2rem;
      align-items: center;
      justify-content: center;
      border-radius: 50%;
    }

    :host ::ng-deep .p-card .p-card-body {
      padding: 1rem;
    }

    :host ::ng-deep .p-tabview .p-tabview-panels {
      padding: 0;
    }

    :host ::ng-deep .p-avatar {
      background-color: var(--primary-color);
      color: white;
    }
  `]
})
export class ResponsavelDashboardComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  // Observables
  loading$;
  currentUser$;

  // Data
  meusFilhos: AlunoResumo[] = [];
  notificacoes: NotificacaoResponsavel[] = [];
  eventosProximos: EventoEscolar[] = [];

  constructor(
    private alunosFacade: AlunosFacade,
    private authFacade: AuthFacade,
    private alunosService: AlunosService
  ) {
    this.loading$ = this.alunosFacade.loading$;
    this.currentUser$ = this.authFacade.currentUser$;
  }

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadData(): void {
    // Load current user's children
    this.currentUser$.pipe(
      takeUntil(this.destroy$)
    ).subscribe(user => {
      if (user?.id) {
        // This would call a service to get children by responsible person ID
        // For now, we'll use mock data
        this.loadMeusFilhos(user.id);
      }
    });
  }

  private loadMeusFilhos(responsavelId: string): void {
    // Call the service to get children by responsible person ID
    this.alunosService.obterAlunosPorResponsavel(responsavelId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (alunos: AlunoResumo[]) => {
          this.meusFilhos = alunos || [];
        },
        error: (error: any) => {
          console.error('Erro ao carregar filhos:', error);
          this.meusFilhos = [];
        }
      });
  }



  get notificacoesNaoLidas(): NotificacaoResponsavel[] {
    return this.notificacoes.filter(n => !n.lida);
  }

  getInitials(nome: string): string {
    return nome.split(' ')
      .map(n => n[0])
      .join('')
      .substring(0, 2)
      .toUpperCase();
  }

  getPrioridadeLabel(prioridade: string): string {
    const labels = {
      'baixa': 'Baixa',
      'media': 'Média',
      'alta': 'Alta'
    };
    return labels[prioridade as keyof typeof labels] || prioridade;
  }

  getPrioridadeSeverity(prioridade: string): string {
    const severities = {
      'baixa': 'info',
      'media': 'warning',
      'alta': 'danger'
    };
    return severities[prioridade as keyof typeof severities] || 'info';
  }

  getTipoEventoLabel(tipo: string): string {
    const labels = {
      'reuniao': 'Reunião',
      'evento': 'Evento',
      'feriado': 'Feriado',
      'prova': 'Prova'
    };
    return labels[tipo as keyof typeof labels] || tipo;
  }

  getTipoEventoSeverity(tipo: string): string {
    const severities = {
      'reuniao': 'info',
      'evento': 'success',
      'feriado': 'secondary',
      'prova': 'warning'
    };
    return severities[tipo as keyof typeof severities] || 'info';
  }

  marcarComoLida(notificacaoId: string): void {
    const notificacao = this.notificacoes.find(n => n.id === notificacaoId);
    if (notificacao) {
      notificacao.lida = true;
      // Here you would call the API to mark as read
    }
  }

  verNotas(alunoId: string): void {
    // Navigate to grades/notes view for the student
    console.log('Ver notas do aluno:', alunoId);
    // This would navigate to a grades component
  }
}