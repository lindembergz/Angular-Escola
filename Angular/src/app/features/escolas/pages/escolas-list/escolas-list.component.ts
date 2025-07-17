import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';

// PrimeNG Components (Mixed standalone and module imports for v19)
import { Card } from 'primeng/card';
import { Button } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { Tag } from 'primeng/tag';
import { InputText } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { Toolbar } from 'primeng/toolbar';
import { Tooltip } from 'primeng/tooltip';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { ProgressSpinner } from 'primeng/progressspinner';
import { TabViewModule } from 'primeng/tabview';
import { MenuItem, ConfirmationService, MessageService } from 'primeng/api';

// Local Imports
import { PageHeaderComponent } from '../../../../shared/components/page-header/page-header.component';
import { RedeEscolarFormComponent } from '../../components/rede-escolar-form/rede-escolar-form.component';
import { Escola, RedeEscolar, EscolasFilter, TipoEscola } from '../../models/escola.model';
import * as EscolasActions from '../../store/escolas.actions';
import * as EscolasSelectors from '../../store/escolas.selectors';

@Component({
  selector: 'app-escolas-list',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    ReactiveFormsModule,
    Card, 
    Button, 
    TableModule, 
    Tag,
    InputText,
    DropdownModule,
    Toolbar,
    Tooltip,
    ConfirmDialogModule,
    ToastModule,
    ProgressSpinner,
    TabViewModule,
    PageHeaderComponent,
    RedeEscolarFormComponent
  ],
  providers: [ConfirmationService, MessageService],
  template: `
    <app-page-header
      title="Escolas"
      subtitle="Gerencie as unidades escolares do sistema"
      icon="pi pi-building"
      [breadcrumbItems]="breadcrumbItems">
      <div slot="actions">
        <p-button 
          label="Nova Escola" 
          icon="pi pi-plus" 
          routerLink="/escolas/nova">
        </p-button>
      </div>
    </app-page-header>

    <div class="content-container">
      <!-- Tabs -->
      <p-tabView [(activeIndex)]="activeTabIndex" class="mb-3">
        <p-tabPanel header="Escolas" leftIcon="pi pi-building">
          <!-- Filtros -->
          <p-card styleClass="mb-3">
            <p-toolbar>
              <div class="p-toolbar-group-start">
                <form [formGroup]="filterForm" class="filter-form">
                  <div class="filter-field">
                    <input 
                      pInputText 
                      placeholder="Buscar por nome..." 
                      formControlName="nome"
                      class="filter-input">
                  </div>
                  <div class="filter-field">
                    <p-dropdown 
                      [options]="tiposEscola" 
                      placeholder="Tipo de escola"
                      formControlName="tipo"
                      optionLabel="label"
                      optionValue="value"
                      [showClear]="true">
                    </p-dropdown>
                  </div>
                  <div class="filter-field">
                    <p-dropdown 
                      [options]="statusOptions" 
                      placeholder="Status"
                      formControlName="ativa"
                      optionLabel="label"
                      optionValue="value"
                      [showClear]="true">
                    </p-dropdown>
                  </div>
                </form>
              </div>
              <div class="p-toolbar-group-end">
                <p-button 
                  icon="pi pi-refresh" 
                  class="p-button-outlined"
                  (onClick)="refreshData()"
                  pTooltip="Atualizar dados">
                </p-button>
              </div>
            </p-toolbar>
          </p-card>

      <!-- Tabela de Escolas -->
      <p-card>
        <p-table 
          [value]="(escolas$ | async) || []" 
          [loading]="loading$ | async"
          [paginator]="true" 
          [rows]="pageSize"
          [totalRecords]="(totalCount$ | async) || 0"
          [lazy]="true"
          (onLazyLoad)="onLazyLoad($event)"
          [showCurrentPageReport]="true"
          currentPageReportTemplate="Mostrando {first} a {last} de {totalRecords} escolas"
          [rowsPerPageOptions]="[10, 25, 50]"
          responsiveLayout="scroll">
          
          <ng-template pTemplate="header">
            <tr>
              <th pSortableColumn="nome">
                Nome 
                <p-sortIcon field="nome"></p-sortIcon>
              </th>
              <th pSortableColumn="tipo">
                Tipo 
                <p-sortIcon field="tipo"></p-sortIcon>
              </th>
              <th>CNPJ</th>
              <th>Cidade</th>
              <th pSortableColumn="ativa">
                Status 
                <p-sortIcon field="ativa"></p-sortIcon>
              </th>
              <th>Ações</th>
            </tr>
          </ng-template>
          
          <ng-template pTemplate="body" let-escola>
            <tr>
              <td>
                <div class="escola-name">
                  <strong>{{ escola.nome }}</strong>
                  <small *ngIf="escola.nomeRedeEscolar" class="text-muted d-block">
                    Rede: {{ escola.nomeRedeEscolar }}
                  </small>
                </div>
              </td>
              <td>
                <span class="tipo-badge">{{ escola.tipo }}</span>
              </td>
              <td>{{ escola.cnpj || '-' }}</td>
              <td>{{ escola.endereco?.cidade || '-' }}</td>
              <td>
                <p-tag 
                  [value]="escola.ativa ? 'Ativa' : 'Inativa'" 
                  [severity]="escola.ativa ? 'success' : 'warning'">
                </p-tag>
              </td>
              <td>
                <div class="action-buttons">
                  <p-button 
                    icon="pi pi-eye" 
                    class="p-button-rounded p-button-text p-button-sm"
                    [routerLink]="['/escolas', escola.id]"
                    pTooltip="Visualizar">
                  </p-button>
                  <p-button 
                    icon="pi pi-pencil" 
                    class="p-button-rounded p-button-text p-button-sm"
                    [routerLink]="['/escolas', escola.id, 'editar']"
                    pTooltip="Editar">
                  </p-button>
                  <p-button 
                    [icon]="escola.ativa ? 'pi pi-times' : 'pi pi-check'"
                    [class]="escola.ativa ? 'p-button-rounded p-button-text p-button-sm p-button-warning' : 'p-button-rounded p-button-text p-button-sm p-button-success'"
                    (onClick)="toggleEscolaStatus(escola)"
                    [pTooltip]="escola.ativa ? 'Desativar' : 'Ativar'">
                  </p-button>
                  <p-button 
                    icon="pi pi-trash" 
                    class="p-button-rounded p-button-text p-button-sm p-button-danger"
                    (onClick)="confirmDelete(escola)"
                    pTooltip="Excluir">
                  </p-button>
                </div>
              </td>
            </tr>
          </ng-template>
          
          <ng-template pTemplate="emptymessage">
            <tr>
              <td colspan="6" class="text-center">
                <div class="empty-state">
                  <i class="pi pi-building empty-icon"></i>
                  <h3>Nenhuma escola encontrada</h3>
                  <p>{{ hasFilters() ? 'Tente ajustar os filtros de busca.' : 'Comece cadastrando a primeira escola do sistema.' }}</p>
                  <p-button 
                    *ngIf="!hasFilters()"
                    label="Cadastrar Escola" 
                    icon="pi pi-plus" 
                    routerLink="/escolas/nova">
                  </p-button>
                </div>
              </td>
            </tr>
          </ng-template>
          
          <ng-template pTemplate="loadingbody">
            <tr>
              <td colspan="6" class="text-center">
                <p-progressSpinner [style]="{'width': '50px', 'height': '50px'}"></p-progressSpinner>
              </td>
            </tr>
          </ng-template>
        </p-table>
      </p-card>
        </p-tabPanel>

        <p-tabPanel header="Redes Escolares" leftIcon="pi pi-sitemap">
          <!-- Toolbar para Redes Escolares -->
          <p-card styleClass="mb-3">
            <p-toolbar>
              <div class="p-toolbar-group-start">
                <h5 class="m-0">Redes Escolares</h5>
              </div>
              <div class="p-toolbar-group-end">
                <p-button 
                  label="Nova Rede" 
                  icon="pi pi-plus" 
                  (onClick)="showRedeEscolarDialog()">
                </p-button>
              </div>
            </p-toolbar>
          </p-card>

          <!-- Tabela de Redes Escolares -->
          <p-card>
            <p-table 
              [value]="(redesEscolares$ | async) || []" 
              [loading]="redesLoading$ | async"
              [paginator]="true" 
              [rows]="10"
              [showCurrentPageReport]="true"
              currentPageReportTemplate="Mostrando {first} a {last} de {totalRecords} redes"
              [rowsPerPageOptions]="[10, 25, 50]"
              responsiveLayout="scroll">
              
              <ng-template pTemplate="header">
                <tr>
                  <th>Nome da Rede</th>
                  <th>CNPJ Mantenedora</th>
                  <th>Cidade</th>
                  <th>Total de Escolas</th>
                  <th>Status</th>
                  <th>Ações</th>
                </tr>
              </ng-template>
              
              <ng-template pTemplate="body" let-rede>
                <tr>
                  <td>
                    <strong>{{ rede.nome }}</strong>
                  </td>
                  <td>{{ rede.cnpjMantenedora }}</td>
                  <td>{{ rede.enderecoSede?.cidade || '-' }}</td>
                  <td>
                    <span class="escolas-count">{{ rede.totalEscolas || 0 }}</span>
                  </td>
                  <td>
                    <p-tag 
                      [value]="rede.ativa ? 'Ativa' : 'Inativa'" 
                      [severity]="rede.ativa ? 'success' : 'warning'">
                    </p-tag>
                  </td>
                  <td>
                    <div class="action-buttons">
                      <p-button 
                        icon="pi pi-eye" 
                        class="p-button-rounded p-button-text p-button-sm"
                        pTooltip="Visualizar escolas da rede">
                      </p-button>
                      <p-button 
                        icon="pi pi-pencil" 
                        class="p-button-rounded p-button-text p-button-sm"
                        (onClick)="editRedeEscolar(rede)"
                        pTooltip="Editar">
                      </p-button>
                      <p-button 
                        [icon]="rede.ativa ? 'pi pi-times' : 'pi pi-check'"
                        [class]="rede.ativa ? 'p-button-rounded p-button-text p-button-sm p-button-warning' : 'p-button-rounded p-button-text p-button-sm p-button-success'"
                        (onClick)="toggleRedeStatus(rede)"
                        [pTooltip]="rede.ativa ? 'Desativar' : 'Ativar'">
                      </p-button>
                      <p-button 
                        icon="pi pi-trash" 
                        class="p-button-rounded p-button-text p-button-sm p-button-danger"
                        (onClick)="confirmDeleteRede(rede)"
                        pTooltip="Excluir">
                      </p-button>
                    </div>
                  </td>
                </tr>
              </ng-template>
              
              <ng-template pTemplate="emptymessage">
                <tr>
                  <td colspan="6" class="text-center">
                    <div class="empty-state">
                      <i class="pi pi-sitemap empty-icon"></i>
                      <h3>Nenhuma rede escolar encontrada</h3>
                      <p>Comece cadastrando a primeira rede escolar do sistema.</p>
                      <p-button 
                        label="Cadastrar Rede" 
                        icon="pi pi-plus" 
                        (onClick)="showRedeEscolarDialog()">
                      </p-button>
                    </div>
                  </td>
                </tr>
              </ng-template>
              
              <ng-template pTemplate="loadingbody">
                <tr>
                  <td colspan="6" class="text-center">
                    <p-progressSpinner [style]="{'width': '50px', 'height': '50px'}"></p-progressSpinner>
                  </td>
                </tr>
              </ng-template>
            </p-table>
          </p-card>
        </p-tabPanel>
      </p-tabView>

      <!-- Rede Escolar Form Dialog -->
      <app-rede-escolar-form
        [visible]="showRedeDialog"
        [redeEscolar]="selectedRede"
        (visibleChange)="showRedeDialog = $event"
        (saved)="onRedeEscolarSaved($event)"
        (cancelled)="onRedeEscolarCancelled()">
      </app-rede-escolar-form>
    </div>

    <p-confirmDialog></p-confirmDialog>
    <p-toast></p-toast>
  `,
  styles: [`
    .content-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 1rem;
    }

    .filter-form {
      display: flex;
      gap: 1rem;
      align-items: center;
      flex-wrap: wrap;
    }

    .filter-field {
      min-width: 200px;
    }

    .filter-input {
      width: 100%;
    }

    .escola-name {
      .text-muted {
        color: var(--text-color-secondary);
        font-size: 0.875rem;
      }
    }

    .tipo-badge {
      background: var(--primary-color);
      color: white;
      padding: 0.25rem 0.5rem;
      border-radius: 0.25rem;
      font-size: 0.875rem;
    }

    .action-buttons {
      display: flex;
      gap: 0.25rem;
    }

    .empty-state {
      text-align: center;
      padding: 3rem 1rem;
      
      .empty-icon {
        font-size: 4rem;
        color: var(--text-color-secondary);
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

    :host ::ng-deep {
      .p-table .p-table-tbody > tr > td {
        vertical-align: middle;
      }

      .p-toolbar {
        padding: 1rem;
        border: none;
        background: transparent;
      }

      .p-card .p-card-body {
        padding: 0;
      }

      .p-card.mb-3 .p-card-body {
        padding: 0;
      }
    }

    @media screen and (max-width: 768px) {
      .filter-form {
        flex-direction: column;
        align-items: stretch;
      }

      .filter-field {
        min-width: auto;
        width: 100%;
      }

      .action-buttons {
        flex-direction: column;
      }
    }
  `]
})
export class EscolasListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  breadcrumbItems: MenuItem[] = [
    { label: 'Escolas' }
  ];

  filterForm: FormGroup;
  pageSize = 10;
  currentPage = 1;
  activeTabIndex = 0;

  // Observables
  escolas$: Observable<Escola[]>;
  loading$: Observable<boolean>;
  error$: Observable<string | null>;
  totalCount$: Observable<number>;
  
  // Redes Escolares
  redesEscolares$: Observable<RedeEscolar[]>;
  redesLoading$: Observable<boolean>;
  showRedeDialog = false;
  selectedRede: RedeEscolar | null = null;

  // Options
  tiposEscola = [
    { label: 'Infantil', value: TipoEscola.INFANTIL },
    { label: 'Fundamental', value: TipoEscola.FUNDAMENTAL },
    { label: 'Médio', value: TipoEscola.MEDIO },
    { label: 'Fundamental e Médio', value: TipoEscola.FUNDAMENTAL_E_MEDIO },
    { label: 'Técnico', value: TipoEscola.TECNICO },
    { label: 'EJA', value: TipoEscola.EJA }
  ];

  statusOptions = [
    { label: 'Ativa', value: true },
    { label: 'Inativa', value: false }
  ];

  constructor(
    private store: Store,
    private fb: FormBuilder,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {
    this.filterForm = this.fb.group({
      nome: [''],
      tipo: [null],
      ativa: [null]
    });

    // Initialize observables
    this.escolas$ = this.store.select(EscolasSelectors.selectAllEscolas);
    this.loading$ = this.store.select(EscolasSelectors.selectEscolasLoading);
    this.error$ = this.store.select(EscolasSelectors.selectEscolasError);
    this.totalCount$ = this.store.select(EscolasSelectors.selectEscolasTotalCount);
    
    // Initialize Redes Escolares observables
    this.redesEscolares$ = this.store.select(EscolasSelectors.selectAllRedesEscolares);
    this.redesLoading$ = this.store.select(EscolasSelectors.selectRedesEscolaresLoading);
  }

  ngOnInit(): void {
    this.loadEscolas();
    this.loadRedesEscolares();
    this.setupFilterSubscription();
    this.setupErrorHandling();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupFilterSubscription(): void {
    this.filterForm.valueChanges
      .pipe(
        takeUntil(this.destroy$),
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(() => {
        this.currentPage = 1;
        this.loadEscolas();
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

  private loadEscolas(): void {
    const filter: EscolasFilter = {
      ...this.filterForm.value,
      page: this.currentPage,
      pageSize: this.pageSize
    };

    // Remove empty values
    Object.keys(filter).forEach(key => {
      if (filter[key as keyof EscolasFilter] === null || filter[key as keyof EscolasFilter] === '') {
        delete filter[key as keyof EscolasFilter];
      }
    });

    this.store.dispatch(EscolasActions.loadEscolas({ filter }));
  }

  onLazyLoad(event: any): void {
    this.currentPage = Math.floor(event.first / event.rows) + 1;
    this.pageSize = event.rows;
    this.loadEscolas();
  }

  refreshData(): void {
    this.loadEscolas();
    this.messageService.add({
      severity: 'success',
      summary: 'Atualizado',
      detail: 'Dados atualizados com sucesso'
    });
  }

  hasFilters(): boolean {
    const values = this.filterForm.value;
    return values.nome || values.tipo || values.ativa !== null;
  }

  toggleEscolaStatus(escola: Escola): void {
    const action = escola.ativa ? 'desativar' : 'ativar';
    const message = escola.ativa ? 
      'Tem certeza que deseja desativar esta escola?' : 
      'Tem certeza que deseja ativar esta escola?';

    this.confirmationService.confirm({
      message,
      header: `${action.charAt(0).toUpperCase() + action.slice(1)} Escola`,
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        const updateData = {
          id: escola.id,
          ativa: !escola.ativa
        };
        
        this.store.dispatch(EscolasActions.updateEscola({ escola: updateData }));
        
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: `Escola ${action}da com sucesso`
        });
      }
    });
  }

  confirmDelete(escola: Escola): void {
    this.confirmationService.confirm({
      message: `Tem certeza que deseja excluir a escola "${escola.nome}"? Esta ação não pode ser desfeita.`,
      header: 'Confirmar Exclusão',
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.store.dispatch(EscolasActions.deleteEscola({ id: escola.id }));
        
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Escola excluída com sucesso'
        });
      }
    });
  }

  // Redes Escolares Methods
  private loadRedesEscolares(): void {
    this.store.dispatch(EscolasActions.loadRedesEscolares());
  }

  showRedeEscolarDialog(): void {
    this.selectedRede = null;
    this.showRedeDialog = true;
  }

  editRedeEscolar(rede: RedeEscolar): void {
    this.selectedRede = rede;
    this.showRedeDialog = true;
  }

  onRedeEscolarSaved(_rede: RedeEscolar): void {
    this.loadRedesEscolares();
  }

  onRedeEscolarCancelled(): void {
    this.selectedRede = null;
  }

  toggleRedeStatus(rede: RedeEscolar): void {
    const action = rede.ativa ? 'desativar' : 'ativar';
    const message = rede.ativa ? 
      'Tem certeza que deseja desativar esta rede escolar?' : 
      'Tem certeza que deseja ativar esta rede escolar?';

    this.confirmationService.confirm({
      message,
      header: `${action.charAt(0).toUpperCase() + action.slice(1)} Rede Escolar`,
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        // TODO: Implement update rede escolar action
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: `Rede escolar ${action}da com sucesso`
        });
      }
    });
  }

  confirmDeleteRede(rede: RedeEscolar): void {
    this.confirmationService.confirm({
      message: `Tem certeza que deseja excluir a rede escolar "${rede.nome}"? Esta ação não pode ser desfeita.`,
      header: 'Confirmar Exclusão',
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        // TODO: Implement delete rede escolar action
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Rede escolar excluída com sucesso'
        });
      }
    });
  }
}