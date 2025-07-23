import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';

// PrimeNG Imports
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { CardModule } from 'primeng/card';
import { ToolbarModule } from 'primeng/toolbar';
import { PaginatorModule } from 'primeng/paginator';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ConfirmationService } from 'primeng/api';
import { DataViewModule } from 'primeng/dataview';
import { ToggleButtonModule } from 'primeng/togglebutton';

import { AlunosFacade } from '../../../../store/alunos/alunos.facade';
import { AlunoResumo } from '../../models/aluno.model';
import { AlunoCardComponent } from '../../components/aluno-card/aluno-card.component';
import { AlunosFilterComponent, AlunosFilterCriteria } from '../../components/alunos-filter/alunos-filter.component';

interface StatusOption {
  label: string;
  value: boolean | undefined;
}

@Component({
  selector: 'app-alunos-list',
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
    DialogModule,
    CardModule,
    ToolbarModule,
    PaginatorModule,
    ProgressSpinnerModule,
    DataViewModule,
    ToggleButtonModule,
    AlunoCardComponent,
    AlunosFilterComponent
  ],
  providers: [ConfirmationService],
  template: `
    <div class="alunos-list-container">
      <!-- Header -->
      <div class="flex justify-content-between align-items-center mb-4">
        <div>
          <h1 class="text-3xl font-bold text-900 m-0">Gestão de Alunos</h1>
          <p class="text-600 mt-1 mb-0">Gerencie os alunos da instituição</p>
        </div>
        <div class="flex gap-2">
          <p-button 
            label="Portal Responsável" 
            icon="pi pi-home" 
            styleClass="p-button-outlined"
            routerLink="/alunos/responsavel-dashboard">
          </p-button>
          <p-button 
            label="Novo Aluno" 
            icon="pi pi-plus" 
            routerLink="/alunos/novo"
            styleClass="p-button-primary">
          </p-button>
        </div>
      </div>

      <!-- Stats Cards -->
      <div class="grid mb-4" *ngIf="stats$ | async as stats">
        <div class="col-12 md:col-3">
          <p-card styleClass="h-full stats-card">
            <div class="flex align-items-center">
              <div class="flex-1">
                <div class="text-2xl font-bold text-900">{{ stats.total }}</div>
                <div class="text-600">Total de Alunos</div>
              </div>
              <i class="pi pi-users text-3xl text-blue-500"></i>
            </div>
          </p-card>
        </div>
        <div class="col-12 md:col-3">
          <p-card styleClass="h-full stats-card">
            <div class="flex align-items-center">
              <div class="flex-1">
                <div class="text-2xl font-bold text-green-600">{{ stats.ativos }}</div>
                <div class="text-600">Alunos Ativos</div>
              </div>
              <i class="pi pi-check-circle text-3xl text-green-500"></i>
            </div>
          </p-card>
        </div>
        <div class="col-12 md:col-3">
          <p-card styleClass="h-full stats-card">
            <div class="flex align-items-center">
              <div class="flex-1">
                <div class="text-2xl font-bold text-orange-600">{{ stats.comMatriculaAtiva }}</div>
                <div class="text-600">Com Matrícula Ativa</div>
              </div>
              <i class="pi pi-id-card text-3xl text-orange-500"></i>
            </div>
          </p-card>
        </div>
        <div class="col-12 md:col-3">
          <p-card styleClass="h-full stats-card">
            <div class="flex align-items-center">
              <div class="flex-1">
                <div class="text-2xl font-bold text-purple-600">{{ stats.comDeficiencia }}</div>
                <div class="text-600">Com Deficiência</div>
              </div>
              <i class="pi pi-heart text-3xl text-purple-500"></i>
            </div>
          </p-card>
        </div>
      </div>

      <!-- Advanced Filters -->
      <app-alunos-filter
        [showAdvancedFilters]="true"
        [initialFilters]="currentFilters"
        (filtersChange)="onFiltersChange($event)"
        (filtersApply)="onFiltersApply($event)"
        (filtersClear)="onFiltersClear()"
        styleClass="mb-4">
      </app-alunos-filter>

      <!-- View Controls -->
      <p-card styleClass="mb-4">
        <div class="flex justify-content-between align-items-center">
          <div class="flex align-items-center gap-3">
            <span class="text-600">Visualização:</span>
            <p-toggleButton 
              [(ngModel)]="isCardView"
              onLabel="Cards" 
              offLabel="Tabela"
              onIcon="pi pi-th-large" 
              offIcon="pi pi-list"
              styleClass="p-button-sm">
            </p-toggleButton>
          </div>
          <div class="flex align-items-center gap-2">
            <span class="text-600 text-sm">{{ (alunos$ | async)?.length || 0 }} aluno(s) encontrado(s)</span>
            <p-button 
              icon="pi pi-refresh" 
              styleClass="p-button-rounded p-button-text p-button-sm"
              pTooltip="Atualizar"
              (onClick)="refresh()">
            </p-button>
          </div>
        </div>
      </p-card>

      <!-- Loading -->
      <div *ngIf="loading$ | async" class="flex justify-content-center p-4">
        <p-progressSpinner></p-progressSpinner>
      </div>

      <!-- Debug Info -->
      <div class="mb-3 p-2 bg-yellow-50 border-round" *ngIf="!(loading$ | async)">
        <small class="text-600">
          Debug: {{ (alunos$ | async)?.length || 0 }} alunos no array | 
          Loading: {{ (loading$ | async) ? 'true' : 'false' }} |
          Error: {{ (error$ | async) || 'none' }}
        </small>
      </div>

      <!-- Content -->
      <div *ngIf="!(loading$ | async)">
        <!-- Card View -->
        <div *ngIf="isCardView" class="grid">
          <div class="col-12 md:col-6 lg:col-4" *ngFor="let aluno of alunos$ | async">
            <app-aluno-card 
              [aluno]="aluno"
              (deleteClick)="confirmDelete($event)">
            </app-aluno-card>
          </div>
          
          <!-- Empty State for Cards -->
          <div class="col-12" *ngIf="(alunos$ | async)?.length === 0">
            <p-card>
              <div class="text-center p-4">
                <i class="pi pi-users text-6xl text-400 mb-4"></i>
                <h3 class="text-900 mb-3">Nenhum aluno encontrado</h3>
                <p class="text-600 mb-4">Tente ajustar os filtros ou cadastre um novo aluno</p>
                <p-button 
                  label="Cadastrar Primeiro Aluno" 
                  icon="pi pi-plus" 
                  routerLink="/alunos/novo">
                </p-button>
              </div>
            </p-card>
          </div>
        </div>

        <!-- Table View -->
        <p-card *ngIf="!isCardView">
          <p-table 
            [value]="(alunos$ | async) || []" 
            [paginator]="false"
            [rows]="20"
            [showCurrentPageReport]="true"
            [rowsPerPageOptions]="[10, 20, 50]"
            [globalFilterFields]="['nome', 'cpf', 'email']"
            styleClass="p-datatable-gridlines"
            [tableStyle]="{ 'min-width': '50rem' }"
            responsiveLayout="scroll">
            
            <ng-template pTemplate="header">
              <tr>
                <th pSortableColumn="nome">
                  Nome <p-sortIcon field="nome"></p-sortIcon>
                </th>
                <th pSortableColumn="cpf">
                  CPF <p-sortIcon field="cpf"></p-sortIcon>
                </th>
                <th pSortableColumn="idade">
                  Idade <p-sortIcon field="idade"></p-sortIcon>
                </th>
                <th>Gênero</th>
                <th>Turma Atual</th>
                <th>Status</th>
                <th>Responsáveis</th>
                <th>Ações</th>
              </tr>
            </ng-template>
            
            <ng-template pTemplate="body" let-aluno>
              <tr>
                <td>
                  <div class="flex align-items-center">
                    <div>
                      <div class="font-medium">{{ aluno.nome }}</div>
                      <div class="text-sm text-600" *ngIf="aluno.email">{{ aluno.email }}</div>
                    </div>
                  </div>
                </td>
                <td>{{ aluno.cpf }}</td>
                <td>{{ aluno.idade }} anos</td>
                <td>{{ aluno.generoDescricao }}</td>
                <td>
                  <span *ngIf="aluno.nomeTurmaAtual; else semTurma">
                    {{ aluno.nomeTurmaAtual }}
                  </span>
                  <ng-template #semTurma>
                    <span class="text-500">Sem turma</span>
                  </ng-template>
                </td>
                <td>
                  <p-tag 
                    [value]="aluno.ativo ? 'Ativo' : 'Inativo'"
                    [severity]="aluno.ativo ? 'success' : 'danger'">
                  </p-tag>
                  <p-tag 
                    *ngIf="aluno.possuiMatriculaAtiva"
                    value="Matriculado"
                    severity="info"
                    styleClass="ml-1">
                  </p-tag>
                  <p-tag 
                    *ngIf="aluno.possuiDeficiencia"
                    value="PcD"
                    severity="warning"
                    styleClass="ml-1"
                    [pTooltip]="aluno.deficienciaDescricao">
                  </p-tag>
                </td>
                <td>
                  <span class="font-medium">{{ aluno.quantidadeResponsaveis }}</span>
                  <span class="text-600"> responsável(is)</span>
                </td>
                <td>
                  <div class="flex gap-2">
                    <p-button 
                      icon="pi pi-eye" 
                      styleClass="p-button-rounded p-button-text p-button-sm"
                      pTooltip="Visualizar"
                      [routerLink]="['/alunos', aluno.id]">
                    </p-button>
                    <p-button 
                      icon="pi pi-pencil" 
                      styleClass="p-button-rounded p-button-text p-button-sm"
                      pTooltip="Editar"
                      [routerLink]="['/alunos', aluno.id, 'editar']">
                    </p-button>
                    <p-button 
                      icon="pi pi-trash" 
                      styleClass="p-button-rounded p-button-text p-button-sm p-button-danger"
                      pTooltip="Excluir"
                      (onClick)="confirmDelete(aluno)">
                    </p-button>
                  </div>
                </td>
              </tr>
            </ng-template>
            
            <ng-template pTemplate="emptymessage">
              <tr>
                <td colspan="8" class="text-center py-4">
                  <div class="flex flex-column align-items-center">
                    <i class="pi pi-users text-4xl text-400 mb-3"></i>
                    <p class="text-600 mb-0">Nenhum aluno encontrado</p>
                    <p class="text-500 text-sm">Tente ajustar os filtros ou cadastre um novo aluno</p>
                  </div>
                </td>
              </tr>
            </ng-template>
          </p-table>
        </p-card>

        <!-- Pagination -->
        <div class="mt-3" *ngIf="paginationInfo$ | async as pagination">
          <p-paginator 
            [rows]="pagination.tamanhoPagina"
            [totalRecords]="pagination.totalRegistros"
            [first]="(pagination.paginaAtual - 1) * pagination.tamanhoPagina"
            [rowsPerPageOptions]="[10, 20, 50]"
            (onPageChange)="onPageChange($event)">
          </p-paginator>
        </div>
      </div>
    </div>

    <!-- Confirm Dialog -->
    <p-confirmDialog></p-confirmDialog>
  `,
  styles: [`
    .alunos-list-container {
      padding: 1rem;
    }

    .loading-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: rgba(255, 255, 255, 0.8);
      display: flex;
      justify-content: center;
      align-items: center;
      z-index: 1000;
    }

    :host ::ng-deep .p-card .p-card-body {
      padding: 1rem;
    }

    :host ::ng-deep .stats-card .p-card-body {
      padding: 1.5rem;
    }

    :host ::ng-deep .p-datatable .p-datatable-tbody > tr > td {
      padding: 0.75rem;
    }

    :host ::ng-deep .p-tag {
      font-size: 0.75rem;
    }

    :host ::ng-deep .p-togglebutton {
      height: 2.5rem;
    }

    .stats-card {
      transition: all 0.3s ease;
    }

    .stats-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    }
  `]
})
export class AlunosListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  // Observables
  alunos$;
  loading$;
  error$;
  paginationInfo$;
  stats$;

  // View properties
  isCardView = false;
  currentFilters: AlunosFilterCriteria = {};

  // Legacy filter properties (keeping for backward compatibility)
  searchTerm = '';
  selectedStatus: boolean | undefined = undefined;

  statusOptions: StatusOption[] = [
    { label: 'Todos', value: undefined },
    { label: 'Ativos', value: true },
    { label: 'Inativos', value: false }
  ];

  private searchSubject = new Subject<string>();

  constructor(
    private alunosFacade: AlunosFacade,
    private confirmationService: ConfirmationService
  ) {
    // Initialize observables
    this.alunos$ = this.alunosFacade.alunos$;
    this.loading$ = this.alunosFacade.loading$;
    this.error$ = this.alunosFacade.error$;
    this.paginationInfo$ = this.alunosFacade.paginationInfo$;
    this.stats$ = this.alunosFacade.alunosStats$;

    // Add debugging subscription
    this.alunos$.subscribe(alunos => {
      console.log('🎯 Component received alunos from store:', alunos);
    });

    // Setup search debouncing
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(searchTerm => {
      this.alunosFacade.searchAlunos(searchTerm);
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadData(): void {
    this.alunosFacade.loadAlunos();
  }

  // New filter methods
  onFiltersChange(filters: AlunosFilterCriteria): void {
    this.currentFilters = filters;
    // Apply filters immediately for better UX
    this.applyFilters(filters);
  }

  onFiltersApply(filters: AlunosFilterCriteria): void {
    this.applyFilters(filters);
  }

  onFiltersClear(): void {
    this.currentFilters = {};
    this.alunosFacade.clearFilters();
    this.alunosFacade.loadAlunos();
  }

  private applyFilters(filters: AlunosFilterCriteria): void {
    // Convert the filter criteria to the format expected by the facade
    const params: any = {};
    
    if (filters.searchTerm) {
      params.searchTerm = filters.searchTerm;
    }
    
    if (filters.escolaId) {
      params.escolaId = filters.escolaId;
    }
    
    if (filters.turmaId) {
      params.turmaId = filters.turmaId;
    }
    
    if (filters.ativo !== undefined) {
      params.ativo = filters.ativo;
    }

    // Apply filters through facade
    this.alunosFacade.setFilters(filters);
    this.alunosFacade.loadAlunos(params);
  }

  // Legacy methods (keeping for backward compatibility)
  onSearchChange(event: any): void {
    const value = event.target.value;
    this.searchSubject.next(value);
  }

  onStatusChange(): void {
    this.alunosFacade.filterByStatus(this.selectedStatus!);
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedStatus = undefined;
    this.currentFilters = {};
    this.alunosFacade.clearFilters();
    this.alunosFacade.loadAlunos();
  }

  onPageChange(event: any): void {
    const page = Math.floor(event.first / event.rows) + 1;
    this.alunosFacade.changePage(page);
  }

  confirmDelete(aluno: AlunoResumo): void {
    this.confirmationService.confirm({
      message: `Tem certeza que deseja excluir o aluno ${aluno.nome}?`,
      header: 'Confirmar Exclusão',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim, excluir',
      rejectLabel: 'Cancelar',
      accept: () => {
        this.alunosFacade.deleteAluno(aluno.id);
      }
    });
  }

  refresh(): void {
    this.alunosFacade.refreshAlunos();
  }
}