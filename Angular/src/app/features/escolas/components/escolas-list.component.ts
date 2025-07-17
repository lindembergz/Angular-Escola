import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

// PrimeNG Components (Mixed standalone and module imports for v19)
import { TableModule } from 'primeng/table';
import { Button } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { Tag } from 'primeng/tag';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { Toolbar } from 'primeng/toolbar';
import { Card } from 'primeng/card';

import { AppState } from '../../../store/app.state';
import { Escola, EscolasFilter, TipoEscola } from '../models/escola.model';
import * as EscolasActions from '../store/escolas.actions';
import * as EscolasSelectors from '../store/escolas.selectors';

@Component({
  selector: 'app-escolas-list',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    Button,
    InputText,
    DropdownModule,
    Tag,
    ConfirmDialogModule,
    ToastModule,
    Toolbar,
    Card
  ],
  template: `
    <div class="card">
      <p-toolbar styleClass="mb-4 gap-2">
        <ng-template pTemplate="left">
          <h2 class="m-0">Gestão de Escolas</h2>
        </ng-template>
        
        <ng-template pTemplate="right">
          <p-button 
            label="Nova Escola" 
            icon="pi pi-plus" 
            class="p-button-success mr-2"
            (onClick)="novaEscola()">
          </p-button>
          <p-button 
            label="Nova Rede Escolar" 
            icon="pi pi-sitemap" 
            class="p-button-info"
            (onClick)="novaRedeEscolar()">
          </p-button>
        </ng-template>
      </p-toolbar>

      <!-- Filtros -->
      <div class="grid mb-4">
        <div class="col-12 md:col-4">
          <span class="p-float-label">
            <input 
              pInputText 
              id="filtroNome" 
              type="text" 
              [(ngModel)]="filtros.nome"
              (input)="aplicarFiltros()"
              class="w-full">
            <label for="filtroNome">Buscar por nome</label>
          </span>
        </div>
        
        <div class="col-12 md:col-3">
          <span class="p-float-label">
            <p-dropdown 
              [options]="tiposEscola" 
              [(ngModel)]="filtros.tipoEscola"
              (onChange)="aplicarFiltros()"
              optionLabel="label" 
              optionValue="value"
              [showClear]="true"
              class="w-full">
            </p-dropdown>
            <label>Tipo de Escola</label>
          </span>
        </div>
        
        <div class="col-12 md:col-3">
          <span class="p-float-label">
            <p-dropdown 
              [options]="statusOptions" 
              [(ngModel)]="filtros.ativa"
              (onChange)="aplicarFiltros()"
              optionLabel="label" 
              optionValue="value"
              [showClear]="true"
              class="w-full">
            </p-dropdown>
            <label>Status</label>
          </span>
        </div>
        
        <div class="col-12 md:col-2">
          <p-button 
            label="Limpar" 
            icon="pi pi-filter-slash" 
            class="p-button-outlined w-full"
            (onClick)="limparFiltros()">
          </p-button>
        </div>
      </div>

      <!-- Tabela -->
      <p-table 
        [value]="escolas$ | async" 
        [loading]="loading$ | async"
        [paginator]="true" 
        [rows]="10"
        [totalRecords]="totalCount$ | async"
        [lazy]="true"
        (onLazyLoad)="carregarEscolas($event)"
        responsiveLayout="scroll"
        styleClass="p-datatable-gridlines">
        
        <ng-template pTemplate="header">
          <tr>
            <th pSortableColumn="nome">Nome <p-sortIcon field="nome"></p-sortIcon></th>
            <th>CNPJ</th>
            <th>Tipo</th>
            <th>Rede Escolar</th>
            <th>Status</th>
            <th>Ações</th>
          </tr>
        </ng-template>
        
        <ng-template pTemplate="body" let-escola>
          <tr>
            <td>
              <strong>{{ escola.nome }}</strong>
              <br>
              <small class="text-color-secondary">{{ escola.endereco.cidade }}/{{ escola.endereco.estado }}</small>
            </td>
            <td>{{ escola.cnpj }}</td>
            <td>
              <p-tag 
                [value]="escola.tipo" 
                [severity]="getTipoSeverity(escola.tipo)">
              </p-tag>
            </td>
            <td>{{ escola.nomeRedeEscolar || 'Independente' }}</td>
            <td>
              <p-tag 
                [value]="escola.ativa ? 'Ativa' : 'Inativa'" 
                [severity]="escola.ativa ? 'success' : 'danger'">
              </p-tag>
            </td>
            <td>
              <div class="flex gap-2">
                <p-button 
                  icon="pi pi-eye" 
                  class="p-button-rounded p-button-text p-button-info"
                  pTooltip="Visualizar"
                  (onClick)="visualizarEscola(escola)">
                </p-button>
                <p-button 
                  icon="pi pi-pencil" 
                  class="p-button-rounded p-button-text p-button-warning"
                  pTooltip="Editar"
                  (onClick)="editarEscola(escola)">
                </p-button>
                <p-button 
                  [icon]="escola.ativa ? 'pi pi-times' : 'pi pi-check'" 
                  [class]="escola.ativa ? 'p-button-rounded p-button-text p-button-danger' : 'p-button-rounded p-button-text p-button-success'"
                  [pTooltip]="escola.ativa ? 'Desativar' : 'Ativar'"
                  (onClick)="toggleStatusEscola(escola)">
                </p-button>
                <p-button 
                  icon="pi pi-trash" 
                  class="p-button-rounded p-button-text p-button-danger"
                  pTooltip="Excluir"
                  (onClick)="excluirEscola(escola)">
                </p-button>
              </div>
            </td>
          </tr>
        </ng-template>
        
        <ng-template pTemplate="emptymessage">
          <tr>
            <td colspan="6" class="text-center">
              <div class="flex flex-column align-items-center justify-content-center py-5">
                <i class="pi pi-building text-4xl text-color-secondary mb-3"></i>
                <p class="text-color-secondary">Nenhuma escola encontrada</p>
              </div>
            </td>
          </tr>
        </ng-template>
      </p-table>
    </div>

    <p-confirmDialog></p-confirmDialog>
    <p-toast></p-toast>
  `
})
export class EscolasListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  // Observables do Store
  escolas$: Observable<Escola[]>;
  loading$: Observable<boolean>;
  error$: Observable<string | null>;
  totalCount$: Observable<number>;
  pagination$: Observable<any>;

  // Filtros
  filtros: EscolasFilter = {
    page: 1,
    pageSize: 10
  };

  // Opções para dropdowns
  tiposEscola = [
    { label: 'Educação Infantil', value: TipoEscola.INFANTIL },
    { label: 'Ensino Fundamental', value: TipoEscola.FUNDAMENTAL },
    { label: 'Ensino Médio', value: TipoEscola.MEDIO },
    { label: 'Fundamental e Médio', value: TipoEscola.FUNDAMENTAL_E_MEDIO },
    { label: 'Ensino Técnico', value: TipoEscola.TECNICO },
    { label: 'EJA', value: TipoEscola.EJA },
    { label: 'Ensino Superior', value: TipoEscola.SUPERIOR },
    { label: 'Pós-Graduação', value: TipoEscola.POS_GRADUACAO }
  ];

  statusOptions = [
    { label: 'Ativa', value: true },
    { label: 'Inativa', value: false }
  ];

  constructor(private store: Store<AppState>) {
    // Inicializar observables do store
    this.escolas$ = this.store.select(EscolasSelectors.selectAllEscolas);
    this.loading$ = this.store.select(EscolasSelectors.selectEscolasLoading);
    this.error$ = this.store.select(EscolasSelectors.selectEscolasError);
    this.totalCount$ = this.store.select(EscolasSelectors.selectEscolasTotalCount);
    this.pagination$ = this.store.select(EscolasSelectors.selectEscolasPagination);
  }

  ngOnInit() {
    // Carregar escolas inicialmente
    this.carregarEscolas();

    // Observar erros e mostrar notificações
    this.error$.pipe(takeUntil(this.destroy$)).subscribe(error => {
      if (error) {
        console.error('Erro:', error);
        // Aqui você pode adicionar notificações toast
      }
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  carregarEscolas(event?: any) {
    if (event) {
      this.filtros.page = Math.floor(event.first / event.rows) + 1;
      this.filtros.pageSize = event.rows;
    }

    this.store.dispatch(EscolasActions.loadEscolas({ filter: this.filtros }));
  }

  aplicarFiltros() {
    this.filtros.page = 1; // Reset para primeira página
    this.carregarEscolas();
  }

  limparFiltros() {
    this.filtros = {
      page: 1,
      pageSize: 10
    };
    this.carregarEscolas();
  }

  novaEscola() {
    // Navegar para formulário de nova escola
    console.log('Navegar para nova escola');
  }

  novaRedeEscolar() {
    // Navegar para formulário de nova rede escolar
    console.log('Navegar para nova rede escolar');
  }

  visualizarEscola(escola: Escola) {
    this.store.dispatch(EscolasActions.setSelectedEscola({ escola }));
    // Navegar para detalhes da escola
    console.log('Visualizar escola:', escola.nome);
  }

  editarEscola(escola: Escola) {
    this.store.dispatch(EscolasActions.setSelectedEscola({ escola }));
    // Navegar para formulário de edição
    console.log('Editar escola:', escola.nome);
  }

  toggleStatusEscola(escola: Escola) {
    const acao = escola.ativa ? 'desativar' : 'ativar';
    console.log(`${acao} escola:`, escola.nome);
    
    // Aqui você pode adicionar confirmação
    // this.store.dispatch(EscolasActions.updateEscola({ 
    //   escola: { id: escola.id, ativa: !escola.ativa } 
    // }));
  }

  excluirEscola(escola: Escola) {
    console.log('Excluir escola:', escola.nome);
    
    // Aqui você pode adicionar confirmação
    // this.store.dispatch(EscolasActions.deleteEscola({ id: escola.id }));
  }

  getTipoSeverity(tipo: string): string {
    switch (tipo) {
      case TipoEscola.INFANTIL:
        return 'info';
      case TipoEscola.FUNDAMENTAL:
        return 'success';
      case TipoEscola.MEDIO:
        return 'warning';
      case TipoEscola.SUPERIOR:
        return 'danger';
      default:
        return 'secondary';
    }
  }
}