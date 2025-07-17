import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil, filter } from 'rxjs/operators';

// PrimeNG Components (Mixed standalone and module imports for v19)
import { Card } from 'primeng/card';
import { Button } from 'primeng/button';
import { Tag } from 'primeng/tag';
import { Skeleton } from 'primeng/skeleton';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MenuItem, MessageService, ConfirmationService } from 'primeng/api';

// Local Imports
import { PageHeaderComponent } from '../../../../shared/components/page-header/page-header.component';
import { Escola } from '../../models/escola.model';
import * as EscolasActions from '../../store/escolas.actions';
import * as EscolasSelectors from '../../store/escolas.selectors';

@Component({
  selector: 'app-escola-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    Card,
    Button,
    Tag,
    Skeleton,
    ToastModule,
    ConfirmDialogModule,
    PageHeaderComponent
  ],
  providers: [MessageService, ConfirmationService],
  template: `
    <app-page-header
      [title]="escola?.nome || 'Carregando...'"
      subtitle="Detalhes da escola"
      icon="pi pi-building"
      [breadcrumbItems]="breadcrumbItems">
      <div slot="actions" *ngIf="escola">
        <p-button 
          label="Editar" 
          icon="pi pi-pencil" 
          [routerLink]="['/escolas', escola.id, 'editar']"
          class="mr-2">
        </p-button>
        <p-button 
          [label]="escola.ativa ? 'Desativar' : 'Ativar'"
          [icon]="escola.ativa ? 'pi pi-times' : 'pi pi-check'"
          [severity]="escola.ativa ? 'warn' : 'success'"
          [outlined]="true"
          (onClick)="toggleEscolaStatus()"
          class="mr-2">
        </p-button>
        <p-button 
          label="Excluir" 
          icon="pi pi-trash" 
          severity="danger"
          [outlined]="true"
          (onClick)="confirmDelete()">
        </p-button>
      </div>
    </app-page-header>

    <div class="content-container">
      <!-- Loading State -->
      <div *ngIf="loading$ | async" class="loading-container">
        <p-card>
          <div class="skeleton-container">
            <p-skeleton height="2rem" class="mb-3"></p-skeleton>
            <p-skeleton height="1rem" class="mb-2"></p-skeleton>
            <p-skeleton height="1rem" class="mb-2"></p-skeleton>
            <p-skeleton height="1rem" class="mb-2"></p-skeleton>
          </div>
        </p-card>
      </div>

      <!-- Content -->
      <div *ngIf="escola && !(loading$ | async)" class="escola-details">
        <!-- Informações Básicas -->
        <p-card header="Informações Básicas" class="mb-3">
          <div class="info-grid">
            <div class="info-item">
              <label>Nome:</label>
              <span>{{ escola.nome }}</span>
            </div>
            <div class="info-item">
              <label>Tipo:</label>
              <span class="tipo-badge">{{ escola.tipo }}</span>
            </div>
            <div class="info-item">
              <label>CNPJ:</label>
              <span>{{ escola.cnpj || 'Não informado' }}</span>
            </div>
            <div class="info-item">
              <label>Status:</label>
              <p-tag 
                [value]="escola.ativa ? 'Ativa' : 'Inativa'" 
                [severity]="escola.ativa ? 'success' : 'warning'">
              </p-tag>
            </div>
            <div class="info-item" *ngIf="escola.nomeRedeEscolar">
              <label>Rede Escolar:</label>
              <span>{{ escola.nomeRedeEscolar }}</span>
            </div>
            <div class="info-item">
              <label>Data de Criação:</label>
              <span>{{ escola.dataCriacao | date:'dd/MM/yyyy' }}</span>
            </div>
          </div>
        </p-card>

        <!-- Endereço -->
        <p-card header="Endereço" class="mb-3" *ngIf="escola.endereco">
          <div class="info-grid">
            <div class="info-item full-width">
              <label>Logradouro:</label>
              <span>{{ escola.endereco.logradouro }}, {{ escola.endereco.numero }}</span>
              <span *ngIf="escola.endereco.complemento"> - {{ escola.endereco.complemento }}</span>
            </div>
            <div class="info-item">
              <label>Bairro:</label>
              <span>{{ escola.endereco.bairro }}</span>
            </div>
            <div class="info-item">
              <label>Cidade:</label>
              <span>{{ escola.endereco.cidade }}</span>
            </div>
            <div class="info-item">
              <label>Estado:</label>
              <span>{{ escola.endereco.estado }}</span>
            </div>
            <div class="info-item">
              <label>CEP:</label>
              <span>{{ escola.endereco.cep }}</span>
            </div>
          </div>
        </p-card>

        <!-- Unidades Escolares -->
        <p-card *ngIf="escola.unidades && escola.unidades.length > 0" class="mb-3">
          <ng-template pTemplate="header">
            <div class="card-header-content">
              <span>Unidades Escolares</span>
              <p-button 
                label="Nova Unidade" 
                icon="pi pi-plus" 
                size="small"
                [outlined]="true">
              </p-button>
            </div>
          </ng-template>
          
          <div class="unidades-grid">
            <div *ngFor="let unidade of escola.unidades" class="unidade-card">
              <div class="unidade-header">
                <h4>{{ unidade.nome }}</h4>
                <p-tag 
                  [value]="unidade.ativa ? 'Ativa' : 'Inativa'" 
                  [severity]="unidade.ativa ? 'success' : 'warning'">
                </p-tag>
              </div>
              <div class="unidade-info">
                <p><strong>Tipo:</strong> {{ unidade.tipo }}</p>
                <p><strong>Capacidade:</strong> {{ unidade.capacidadeMaximaAlunos }} alunos</p>
                <p><strong>Matriculados:</strong> {{ unidade.alunosMatriculados }} alunos</p>
                <p><strong>Ocupação:</strong> {{ unidade.percentualOcupacao }}%</p>
                <p *ngIf="unidade.seriesAtendidas?.length">
                  <strong>Séries:</strong> {{ unidade.seriesAtendidas.join(', ') }}
                </p>
              </div>
              <div class="unidade-actions">
                <p-button 
                  icon="pi pi-pencil" 
                  size="small"
                  [outlined]="true"
                  pTooltip="Editar unidade">
                </p-button>
                <p-button 
                  icon="pi pi-trash" 
                  size="small"
                  severity="danger"
                  [outlined]="true"
                  pTooltip="Remover unidade">
                </p-button>
              </div>
            </div>
          </div>
        </p-card>

        <!-- Estatísticas -->
        <p-card header="Estatísticas" *ngIf="escola.unidades && escola.unidades.length > 0">
          <div class="stats-grid">
            <div class="stat-item">
              <div class="stat-value">{{ escola.unidades.length }}</div>
              <div class="stat-label">Total de Unidades</div>
            </div>
            <div class="stat-item">
              <div class="stat-value">{{ getTotalCapacidade() }}</div>
              <div class="stat-label">Capacidade Total</div>
            </div>
            <div class="stat-item">
              <div class="stat-value">{{ getTotalMatriculados() }}</div>
              <div class="stat-label">Alunos Matriculados</div>
            </div>
            <div class="stat-item">
              <div class="stat-value">{{ getOcupacaoMedia() }}%</div>
              <div class="stat-label">Ocupação Média</div>
            </div>
          </div>
        </p-card>
      </div>

      <!-- Error State -->
      <div *ngIf="error$ | async as error" class="error-container">
        <p-card>
          <div class="error-content">
            <i class="pi pi-exclamation-triangle error-icon"></i>
            <h3>Erro ao carregar escola</h3>
            <p>{{ error }}</p>
            <p-button 
              label="Tentar Novamente" 
              icon="pi pi-refresh"
              (onClick)="loadEscola()">
            </p-button>
          </div>
        </p-card>
      </div>
    </div>

    <p-confirmDialog></p-confirmDialog>
    <p-toast></p-toast>
  `,
  styles: [`
    .content-container {
      max-width: 1000px;
      margin: 0 auto;
      padding: 0 1rem;
    }

    .loading-container,
    .error-container {
      .skeleton-container {
        padding: 1rem;
      }
      
      .error-content {
        text-align: center;
        padding: 2rem;
        
        .error-icon {
          font-size: 3rem;
          color: var(--red-500);
          margin-bottom: 1rem;
        }
        
        h3 {
          color: var(--text-color);
          margin-bottom: 0.5rem;
        }
        
        p {
          color: var(--text-color-secondary);
          margin-bottom: 1.5rem;
        }
      }
    }

    .card-header-content {
      display: flex;
      justify-content: space-between;
      align-items: center;
      width: 100%;
    }

    .info-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1.5rem;
    }

    .info-item {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;

      &.full-width {
        grid-column: 1 / -1;
      }

      label {
        font-weight: 600;
        color: var(--text-color-secondary);
        font-size: 0.875rem;
        text-transform: uppercase;
        letter-spacing: 0.5px;
      }

      span {
        color: var(--text-color);
        font-size: 1rem;
      }
    }

    .tipo-badge {
      background: var(--primary-color);
      color: white;
      padding: 0.25rem 0.75rem;
      border-radius: 1rem;
      font-size: 0.875rem;
      font-weight: 500;
      display: inline-block;
      width: fit-content;
    }

    .unidades-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 1.5rem;
    }

    .unidade-card {
      border: 1px solid var(--surface-border);
      border-radius: 0.5rem;
      padding: 1.5rem;
      background: var(--surface-ground);

      .unidade-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 1rem;

        h4 {
          margin: 0;
          color: var(--text-color);
        }
      }

      .unidade-info {
        margin-bottom: 1rem;

        p {
          margin: 0.5rem 0;
          color: var(--text-color-secondary);
        }
      }

      .unidade-actions {
        display: flex;
        gap: 0.5rem;
        justify-content: flex-end;
      }
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 1.5rem;
    }

    .stat-item {
      text-align: center;
      padding: 1rem;
      border: 1px solid var(--surface-border);
      border-radius: 0.5rem;
      background: var(--surface-ground);

      .stat-value {
        font-size: 2rem;
        font-weight: 700;
        color: var(--primary-color);
        margin-bottom: 0.5rem;
      }

      .stat-label {
        font-size: 0.875rem;
        color: var(--text-color-secondary);
        text-transform: uppercase;
        letter-spacing: 0.5px;
      }
    }

    :host ::ng-deep {
      .p-card .p-card-header {
        padding: 1rem 1.5rem;
        border-bottom: 1px solid var(--surface-border);
      }

      .p-card .p-card-body {
        padding: 1.5rem;
      }
    }

    @media screen and (max-width: 768px) {
      .info-grid {
        grid-template-columns: 1fr;
      }

      .unidades-grid {
        grid-template-columns: 1fr;
      }

      .stats-grid {
        grid-template-columns: repeat(2, 1fr);
      }
    }
  `]
})
export class EscolaDetailComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  escola: Escola | null = null;
  escolaId: string | null = null;
  breadcrumbItems: MenuItem[] = [];

  // Observables
  loading$: Observable<boolean>;
  error$: Observable<string | null>;

  constructor(
    private store: Store,
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {
    this.loading$ = this.store.select(EscolasSelectors.selectEscolasLoading);
    this.error$ = this.store.select(EscolasSelectors.selectEscolasError);
  }

  ngOnInit(): void {
    this.route.params
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        this.escolaId = params['id'];
        if (this.escolaId) {
          this.loadEscola();
          this.setupEscolaSubscription();
        }
      });

    this.setupErrorHandling();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadEscola(): void {
    if (this.escolaId) {
      this.store.dispatch(EscolasActions.loadEscolaById({ id: this.escolaId }));
    }
  }

  private setupEscolaSubscription(): void {
    this.store.select(EscolasSelectors.selectSelectedEscola)
      .pipe(
        takeUntil(this.destroy$),
        filter(escola => !!escola)
      )
      .subscribe(escola => {
        if (escola) {
          this.escola = escola;
          this.updateBreadcrumbs();
        }
      });
  }

  private setupErrorHandling(): void {
    this.error$
      .pipe(takeUntil(this.destroy$))
      .subscribe(error => {
        if (error) {
          this.messageService.add({
            severity: 'error',
            summary: 'Erro',
            detail: error
          });
        }
      });
  }

  private updateBreadcrumbs(): void {
    this.breadcrumbItems = [
      { label: 'Escolas', routerLink: '/escolas' },
      { label: this.escola?.nome || 'Detalhes' }
    ];
  }

  toggleEscolaStatus(): void {
    if (!this.escola) return;

    const action = this.escola.ativa ? 'desativar' : 'ativar';
    const message = this.escola.ativa ? 
      'Tem certeza que deseja desativar esta escola?' : 
      'Tem certeza que deseja ativar esta escola?';

    this.confirmationService.confirm({
      message,
      header: `${action.charAt(0).toUpperCase() + action.slice(1)} Escola`,
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        if (this.escola) {
          const updateData = {
            id: this.escola.id,
            ativa: !this.escola.ativa
          };
          
          this.store.dispatch(EscolasActions.updateEscola({ escola: updateData }));
          
          this.messageService.add({
            severity: 'success',
            summary: 'Sucesso',
            detail: `Escola ${action}da com sucesso`
          });
        }
      }
    });
  }

  confirmDelete(): void {
    if (!this.escola) return;

    this.confirmationService.confirm({
      message: `Tem certeza que deseja excluir a escola "${this.escola.nome}"? Esta ação não pode ser desfeita.`,
      header: 'Confirmar Exclusão',
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        if (this.escola) {
          this.store.dispatch(EscolasActions.deleteEscola({ id: this.escola.id }));
          
          this.messageService.add({
            severity: 'success',
            summary: 'Sucesso',
            detail: 'Escola excluída com sucesso'
          });

          // Navigate back to list after deletion
          setTimeout(() => {
            this.router.navigate(['/escolas']);
          }, 1500);
        }
      }
    });
  }

  getTotalCapacidade(): number {
    if (!this.escola?.unidades) return 0;
    return this.escola.unidades.reduce((total, unidade) => total + unidade.capacidadeMaximaAlunos, 0);
  }

  getTotalMatriculados(): number {
    if (!this.escola?.unidades) return 0;
    return this.escola.unidades.reduce((total, unidade) => total + unidade.alunosMatriculados, 0);
  }

  getOcupacaoMedia(): number {
    if (!this.escola?.unidades || this.escola.unidades.length === 0) return 0;
    const totalOcupacao = this.escola.unidades.reduce((total, unidade) => total + unidade.percentualOcupacao, 0);
    return Math.round(totalOcupacao / this.escola.unidades.length);
  }
}