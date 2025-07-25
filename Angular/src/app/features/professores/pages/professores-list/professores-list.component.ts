import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';

// PrimeNG Imports
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { CardModule } from 'primeng/card';
import { ToolbarModule } from 'primeng/toolbar';
import { PaginatorModule } from 'primeng/paginator';
import { SkeletonModule } from 'primeng/skeleton';

import { ConfirmationService, MessageService } from 'primeng/api';

// Store
import { AppState } from '../../../../store/app.state';
import * as ProfessoresActions from '../../../../store/professores/professores.actions';
import * as ProfessoresSelectors from '../../../../store/professores/professores.selectors';

// Models
import { ProfessorResumo, FiltrosProfessor } from '../../models/professor.model';

@Component({
  selector: 'app-professores-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    DropdownModule,
    TagModule,
    TooltipModule,
    ConfirmDialogModule,
    ToastModule,
    CardModule,
    ToolbarModule,
    PaginatorModule,
    SkeletonModule
  ],
  providers: [ConfirmationService, MessageService],
  template: `
    <div class="professores-list-container">
      <p-toast></p-toast>
      <p-confirmDialog></p-confirmDialog>

      <!-- Header -->
      <div class="page-header mb-4">
        <h1 class="text-3xl font-bold text-900">Gestão de Professores</h1>
        <p class="text-600 mt-2">Gerencie os professores da instituição</p>
      </div>

      <!-- Toolbar -->
      <p-toolbar class="mb-4">
        <div class="p-toolbar-group-start">
          <p-button 
            label="Novo Professor" 
            icon="pi pi-plus" 
            routerLink="/professores/novo"
            class="p-button-success">
          </p-button>
        </div>

        <div class="p-toolbar-group-end">
          <div class="flex gap-2 align-items-center">
            <!-- Search -->
            <span class="p-input-icon-left">
              <i class="pi pi-search"></i>
              <input 
                pInputText 
                type="text" 
                [(ngModel)]="searchTerm"
                (input)="onSearchChange($event)"
                placeholder="Buscar professores..."
                class="w-20rem">
            </span>

            <!-- Status Filter -->
            <p-dropdown 
              [(ngModel)]="statusFilter"
              [options]="statusOptions"
              (onChange)="onFilterChange()"
              placeholder="Status"
              [showClear]="true"
              class="w-10rem">
            </p-dropdown>

            <!-- Refresh Button -->
            <p-button 
              icon="pi pi-refresh" 
              (onClick)="refreshData()"
              [loading]="loading$ | async"
              pTooltip="Atualizar dados"
              class="p-button-outlined">
            </p-button>
          </div>
        </div>
      </p-toolbar>

      <!-- Data Table -->
      <p-card>
        <p-table 
          [value]="(professores$ | async) || []" 
          [loading]="loading$ | async"
          [paginator]="false"
          [rows]="pageSize"
          [showCurrentPageReport]="true"
          [rowsPerPageOptions]="[10, 25, 50]"
          currentPageReportTemplate="Mostrando {first} a {last} de {totalRecords} professores"
          [globalFilterFields]="['nome', 'cpf', 'registro', 'email']"
          styleClass="p-datatable-striped"
          [tableStyle]="{ 'min-width': '60rem' }">

          <!-- Loading Template -->
          <ng-template pTemplate="loadingbody">
            <tr>
              <td colspan="8">
                <div class="flex align-items-center justify-content-center p-4">
                  <p-skeleton height="2rem" class="mb-2 w-full"></p-skeleton>
                </div>
              </td>
            </tr>
          </ng-template>

          <!-- Empty Template -->
          <ng-template pTemplate="emptymessage">
            <tr>
              <td colspan="8">
                <div class="flex flex-column align-items-center justify-content-center p-4">
                  <i class="pi pi-users text-4xl text-400 mb-3"></i>
                  <p class="text-600 text-lg">Nenhum professor encontrado</p>
                  <p class="text-500">
                    {{ searchTerm ? 'Tente ajustar os filtros de busca' : 'Clique em "Novo Professor" para começar' }}
                  </p>
                </div>
              </td>
            </tr>
          </ng-template>

          <!-- Header -->
          <ng-template pTemplate="header">
            <tr>
              <th pSortableColumn="nome">
                Nome <p-sortIcon field="nome"></p-sortIcon>
              </th>
              <th pSortableColumn="cpf">
                CPF <p-sortIcon field="cpf"></p-sortIcon>
              </th>
              <th pSortableColumn="registro">
                Registro <p-sortIcon field="registro"></p-sortIcon>
              </th>
              <th>Email</th>
              <th>Carga Horária</th>
              <th>Disciplinas</th>
              <th>Status</th>
              <th>Ações</th>
            </tr>
          </ng-template>

          <!-- Body -->
          <ng-template pTemplate="body" let-professor>
            <tr>
              <td>
                <div class="flex align-items-center">
                  <div>
                    <div class="font-medium text-900">{{ professor.nome }}</div>
                    <div class="text-500 text-sm">{{ professor.maiorTitulo }}</div>
                  </div>
                </div>
              </td>
              <td>{{ professor.cpf }}</td>
              <td>{{ professor.registro }}</td>
              <td>
                <span *ngIf="professor.email; else noEmail">{{ professor.email }}</span>
                <ng-template #noEmail>
                  <span class="text-400">Não informado</span>
                </ng-template>
              </td>
              <td>
                <span class="font-medium">{{ professor.cargaHorariaTotal }}h</span>
                <span class="text-500">/semana</span>
              </td>
              <td>
                <p-tag 
                  [value]="professor.quantidadeDisciplinas + ' disciplina' + (professor.quantidadeDisciplinas !== 1 ? 's' : '')"
                  [severity]="professor.quantidadeDisciplinas > 0 ? 'info' : 'warning'">
                </p-tag>
              </td>
              <td>
                <p-tag 
                  [value]="professor.ativo ? 'Ativo' : 'Inativo'"
                  [severity]="professor.ativo ? 'success' : 'danger'">
                </p-tag>
              </td>
              <td>
                <div class="flex gap-2">
                  <p-button 
                    icon="pi pi-eye" 
                    [routerLink]="['/professores', professor.id]"
                    pTooltip="Ver detalhes"
                    class="p-button-rounded p-button-text p-button-info">
                  </p-button>
                  
                  <p-button 
                    icon="pi pi-pencil" 
                    [routerLink]="['/professores', professor.id, 'editar']"
                    pTooltip="Editar"
                    class="p-button-rounded p-button-text p-button-warning">
                  </p-button>
                  
                  <p-button 
                    *ngIf="professor.ativo"
                    icon="pi pi-times" 
                    (onClick)="confirmDeactivate(professor)"
                    pTooltip="Desativar"
                    class="p-button-rounded p-button-text p-button-danger">
                  </p-button>
                </div>
              </td>
            </tr>
          </ng-template>
        </p-table>

        <!-- Custom Paginator -->
        <div class="flex justify-content-between align-items-center mt-3" *ngIf="pagination$ | async as pagination">
          <div class="text-500">
            Mostrando {{ getDisplayRange(pagination) }} de {{ pagination.totalItems }} professores
          </div>
          
          <p-paginator
            [rows]="pagination.pageSize"
            [totalRecords]="pagination.totalItems"
            [first]="(pagination.currentPage - 1) * pagination.pageSize"
            (onPageChange)="onPageChange($event)"
            [rowsPerPageOptions]="[10, 25, 50]"
            [showCurrentPageReport]="false">
          </p-paginator>
        </div>
      </p-card>
    </div>
  `,
  styles: [`
    .professores-list-container {
      padding: 1rem;
    }

    .page-header {
      margin-bottom: 2rem;
    }

    :host ::ng-deep {
      .p-datatable .p-datatable-tbody > tr > td {
        padding: 1rem 0.75rem;
      }

      .p-toolbar {
        border-radius: 6px;
        padding: 1rem;
      }

      .p-card .p-card-body {
        padding: 1.5rem;
      }

      .p-paginator {
        border: none;
        padding: 1rem 0;
      }
    }
  `]
})
export class ProfessoresListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  // Observables
  professores$: Observable<ProfessorResumo[]>;
  loading$: Observable<boolean>;
  error$: Observable<string | null>;
  pagination$: Observable<any>;

  // Form data
  searchTerm = '';
  statusFilter: boolean | null = null;
  pageSize = 10;

  // Options
  statusOptions = [
    { label: 'Ativo', value: true },
    { label: 'Inativo', value: false }
  ];

  // Current filters
  private currentFilters: FiltrosProfessor = {
    pagina: 1,
    tamanhoPagina: 10
  };

  constructor(
    private store: Store<AppState>,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {
    // Initialize observables
    this.professores$ = this.store.select(ProfessoresSelectors.selectProfessores);
    this.loading$ = this.store.select(ProfessoresSelectors.selectLoading);
    this.error$ = this.store.select(ProfessoresSelectors.selectError);
    this.pagination$ = this.store.select(ProfessoresSelectors.selectPagination);
  }

  ngOnInit(): void {
    this.loadData();
    this.setupSearch();
    this.setupErrorHandling();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupSearch(): void {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(searchTerm => {
      this.currentFilters = {
        ...this.currentFilters,
        nome: searchTerm || undefined,
        pagina: 1
      };
      this.loadData();
    });
  }

  private setupErrorHandling(): void {
    this.error$.pipe(
      takeUntil(this.destroy$)
    ).subscribe(error => {
      if (error) {
        this.messageService.add({
          severity: 'error',
          summary: 'Erro',
          detail: error,
          life: 5000
        });
      }
    });
  }

  private loadData(): void {
    this.store.dispatch(ProfessoresActions.loadProfessores({ 
      filtros: this.currentFilters 
    }));
  }

  onSearchChange(event: any): void {
    const value = event.target.value;
    this.searchSubject.next(value);
  }

  onFilterChange(): void {
    this.currentFilters = {
      ...this.currentFilters,
      ativo: this.statusFilter ?? undefined,
      pagina: 1
    };
    this.loadData();
  }

  onPageChange(event: any): void {
    this.currentFilters = {
      ...this.currentFilters,
      pagina: event.page + 1,
      tamanhoPagina: event.rows
    };
    this.pageSize = event.rows;
    this.loadData();
  }

  refreshData(): void {
    this.loadData();
    this.messageService.add({
      severity: 'success',
      summary: 'Atualizado',
      detail: 'Dados atualizados com sucesso',
      life: 3000
    });
  }

  confirmDeactivate(professor: ProfessorResumo): void {
    this.confirmationService.confirm({
      message: `Tem certeza que deseja desativar o professor ${professor.nome}?`,
      header: 'Confirmar Desativação',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim, desativar',
      rejectLabel: 'Cancelar',
      accept: () => {
        this.store.dispatch(ProfessoresActions.deactivateProfessor({ 
          id: professor.id 
        }));
        
        this.messageService.add({
          severity: 'success',
          summary: 'Professor Desativado',
          detail: `${professor.nome} foi desativado com sucesso`,
          life: 3000
        });
      }
    });
  }

  getDisplayRange(pagination: any): string {
    const start = (pagination.currentPage - 1) * pagination.pageSize + 1;
    const end = Math.min(pagination.currentPage * pagination.pageSize, pagination.totalItems);
    return `${start}-${end}`;
  }
}