import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';

// PrimeNG Imports
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { TagModule } from 'primeng/tag';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { ToolbarModule } from 'primeng/toolbar';
import { PaginatorModule } from 'primeng/paginator';
import { ConfirmationService } from 'primeng/api';

// Store
import { AppState } from '../../../../store/app.state';
import * as AcademicoActions from '../../../../store/academico/academico.actions';
import * as AcademicoSelectors from '../../../../store/academico/academico.selectors';

// Models
import { Turma, OpcoesTurma } from '../../models/turma.model';

@Component({
  selector: 'app-turmas-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    DropdownModule,
    TagModule,
    ConfirmDialogModule,
    TooltipModule,
    CardModule,
    ToolbarModule,
    PaginatorModule
  ],
  providers: [ConfirmationService],
  template: `
    <div class="turmas-list-container">
      <p-card>
        <ng-template pTemplate="header">
          <div class="flex justify-content-between align-items-center">
            <h2 class="m-0">
              <i class="pi pi-users mr-2"></i>
              Gestão de Turmas
            </h2>
          </div>
        </ng-template>

        <p-toolbar class="mb-4">
          <ng-template pTemplate="left">
            <button 
              pButton 
              type="button" 
              label="Nova Turma" 
              icon="pi pi-plus" 
              class="p-button-success mr-2"
              routerLink="/academico/turmas/nova">
            </button>
            <button 
              pButton 
              type="button" 
              label="Atualizar" 
              icon="pi pi-refresh" 
              class="p-button-outlined"
              (click)="atualizarLista()">
            </button>
          </ng-template>
        </p-toolbar>

        <!-- Filtros -->
        <form [formGroup]="filtrosForm" class="grid mb-4">
          <div class="col-12 md:col-3">
            <label for="serie" class="block text-900 font-medium mb-2">Série</label>
            <p-dropdown
              id="serie"
              formControlName="serie"
              [options]="serieOptions"
              placeholder="Todas as séries"
              [showClear]="true"
              styleClass="w-full">
            </p-dropdown>
          </div>
          
          <div class="col-12 md:col-3">
            <label for="turno" class="block text-900 font-medium mb-2">Turno</label>
            <p-dropdown
              id="turno"
              formControlName="turno"
              [options]="turnoOptions"
              placeholder="Todos os turnos"
              [showClear]="true"
              styleClass="w-full">
            </p-dropdown>
          </div>
          
          <div class="col-12 md:col-3">
            <label for="anoLetivo" class="block text-900 font-medium mb-2">Ano Letivo</label>
            <p-dropdown
              id="anoLetivo"
              formControlName="anoLetivo"
              [options]="anoLetivoOptions"
              placeholder="Todos os anos"
              [showClear]="true"
              styleClass="w-full">
            </p-dropdown>
          </div>
          
          <div class="col-12 md:col-3">
            <label for="ativa" class="block text-900 font-medium mb-2">Status</label>
            <p-dropdown
              id="ativa"
              formControlName="ativa"
              [options]="statusOptions"
              placeholder="Todos os status"
              [showClear]="true"
              styleClass="w-full">
            </p-dropdown>
          </div>
        </form>

        <!-- Tabela -->
        <p-table 
          [value]="(turmas$ | async) || []" 
          [loading]="loading$ | async"
          [paginator]="false"
          [rows]="pageSize"
          [showCurrentPageReport]="true"
          [rowsPerPageOptions]="[10, 25, 50]"
          currentPageReportTemplate="Mostrando {first} a {last} de {totalRecords} turmas"
          [globalFilterFields]="['nome', 'serie', 'turno']"
          styleClass="p-datatable-gridlines">
          
          <ng-template pTemplate="header">
            <tr>
              <th pSortableColumn="nome">
                Nome <p-sortIcon field="nome"></p-sortIcon>
              </th>
              <th pSortableColumn="serie">
                Série <p-sortIcon field="serie"></p-sortIcon>
              </th>
              <th pSortableColumn="turno">
                Turno <p-sortIcon field="turno"></p-sortIcon>
              </th>
              <th pSortableColumn="anoLetivo">
                Ano Letivo <p-sortIcon field="anoLetivo"></p-sortIcon>
              </th>
              <th>Capacidade</th>
              <th>Alunos</th>
              <th>Status</th>
              <th>Ações</th>
            </tr>
          </ng-template>
          
          <ng-template pTemplate="body" let-turma>
            <tr>
              <td>
                <strong>{{ turma.nome }}</strong>
              </td>
              <td>{{ turma.serie }}</td>
              <td>{{ turma.turno }}</td>
              <td>{{ turma.anoLetivo }}</td>
              <td>
                <span class="text-600">{{ turma.alunosMatriculados }}/{{ turma.capacidadeMaxima }}</span>
                <div class="w-full bg-gray-200 border-round mt-1" style="height: 6px;">
                  <div 
                    class="bg-primary border-round h-full" 
                    [style.width.%]="(turma.alunosMatriculados / turma.capacidadeMaxima) * 100">
                  </div>
                </div>
              </td>
              <td>
                <span class="font-semibold">{{ turma.alunosMatriculados }}</span>
                <small class="text-500 block">{{ turma.vagasDisponiveis }} vagas</small>
              </td>
              <td>
                <p-tag 
                  [value]="turma.ativa ? 'Ativa' : 'Inativa'" 
                  [severity]="turma.ativa ? 'success' : 'danger'">
                </p-tag>
              </td>
              <td>
                <div class="flex gap-2">
                  <button 
                    pButton 
                    type="button" 
                    icon="pi pi-eye" 
                    class="p-button-rounded p-button-text p-button-info"
                    pTooltip="Visualizar"
                    [routerLink]="['/academico/turmas', turma.id]">
                  </button>
                  <button 
                    pButton 
                    type="button" 
                    icon="pi pi-pencil" 
                    class="p-button-rounded p-button-text p-button-warning"
                    pTooltip="Editar"
                    [routerLink]="['/academico/turmas', turma.id, 'editar']">
                  </button>
                  <button 
                    pButton 
                    type="button" 
                    [icon]="turma.ativa ? 'pi pi-times' : 'pi pi-check'"
                    [class]="turma.ativa ? 'p-button-rounded p-button-text p-button-danger' : 'p-button-rounded p-button-text p-button-success'"
                    [pTooltip]="turma.ativa ? 'Inativar' : 'Reativar'"
                    (click)="toggleStatus(turma)">
                  </button>
                </div>
              </td>
            </tr>
          </ng-template>
          
          <ng-template pTemplate="emptymessage">
            <tr>
              <td colspan="8" class="text-center py-4">
                <i class="pi pi-info-circle text-3xl text-400 mb-3"></i>
                <p class="text-600 m-0">Nenhuma turma encontrada</p>
              </td>
            </tr>
          </ng-template>
        </p-table>

        <!-- Paginação -->
        <p-paginator 
          [rows]="pageSize"
          [totalRecords]="(pagination$ | async)?.totalCount || 0"
          [first]="((pagination$ | async)?.page || 1 - 1) * pageSize"
          (onPageChange)="onPageChange($event)"
          [showCurrentPageReport]="true"
          currentPageReportTemplate="Mostrando {first} a {last} de {totalRecords} turmas">
        </p-paginator>
      </p-card>
    </div>

    <p-confirmDialog></p-confirmDialog>
  `,
  styles: [`
    .turmas-list-container {
      padding: 1rem;
    }
    
    .p-datatable .p-datatable-tbody > tr > td {
      padding: 1rem;
    }
    
    .p-toolbar {
      border: 1px solid var(--surface-border);
      border-radius: var(--border-radius);
    }
  `]
})
export class TurmasListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  turmas$: Observable<Turma[]>;
  loading$: Observable<boolean>;
  pagination$: Observable<any>;
  opcoesTurma$: Observable<OpcoesTurma | null>;
  
  filtrosForm: FormGroup;
  pageSize = 10;
  
  serieOptions: any[] = [];
  turnoOptions: any[] = [];
  anoLetivoOptions: any[] = [];
  statusOptions = [
    { label: 'Ativa', value: true },
    { label: 'Inativa', value: false }
  ];

  constructor(
    private store: Store<AppState>,
    private fb: FormBuilder,
    private confirmationService: ConfirmationService
  ) {
    this.turmas$ = this.store.select(AcademicoSelectors.selectTurmas);
    this.loading$ = this.store.select(AcademicoSelectors.selectLoading);
    this.pagination$ = this.store.select(AcademicoSelectors.selectPagination);
    this.opcoesTurma$ = this.store.select(AcademicoSelectors.selectOpcoesTurma);
    
    this.filtrosForm = this.fb.group({
      serie: [null],
      turno: [null],
      anoLetivo: [null],
      ativa: [null]
    });
  }

  ngOnInit(): void {
    this.carregarOpcoes();
    this.carregarTurmas();
    this.configurarFiltros();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private carregarOpcoes(): void {
    this.store.dispatch(AcademicoActions.loadOpcoesTurma());
    
    this.opcoesTurma$
      .pipe(takeUntil(this.destroy$))
      .subscribe(opcoes => {
        if (opcoes) {
          this.serieOptions = opcoes.series.map(serie => ({ label: serie, value: serie }));
          this.turnoOptions = opcoes.turnos.map(turno => ({ label: turno, value: turno }));
          this.anoLetivoOptions = opcoes.anosLetivos.map(ano => ({ label: ano.toString(), value: ano }));
        }
      });
  }

  private carregarTurmas(): void {
    const filtros = this.filtrosForm.value;
    this.store.dispatch(AcademicoActions.loadTurmas({
      page: 1,
      pageSize: this.pageSize,
      ...filtros
    }));
  }

  private configurarFiltros(): void {
    this.filtrosForm.valueChanges
      .pipe(
        takeUntil(this.destroy$),
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(() => {
        this.carregarTurmas();
      });
  }

  atualizarLista(): void {
    this.carregarTurmas();
  }

  onPageChange(event: any): void {
    const page = Math.floor(event.first / event.rows) + 1;
    this.pageSize = event.rows;
    
    const filtros = this.filtrosForm.value;
    this.store.dispatch(AcademicoActions.loadTurmas({
      page,
      pageSize: this.pageSize,
      ...filtros
    }));
  }

  toggleStatus(turma: Turma): void {
    const action = turma.ativa ? 'inativar' : 'reativar';
    const message = turma.ativa 
      ? `Tem certeza que deseja inativar a turma "${turma.nome}"?`
      : `Tem certeza que deseja reativar a turma "${turma.nome}"?`;

    this.confirmationService.confirm({
      message,
      header: `${action.charAt(0).toUpperCase() + action.slice(1)} Turma`,
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        if (turma.ativa) {
          this.store.dispatch(AcademicoActions.inativarTurma({ id: turma.id }));
        } else {
          this.store.dispatch(AcademicoActions.reativarTurma({ id: turma.id }));
        }
      }
    });
  }
}