import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { MultiSelectModule } from 'primeng/multiselect';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { AccordionModule } from 'primeng/accordion';

export interface AlunosFilterCriteria {
  searchTerm?: string;
  escolaId?: string;
  turmaId?: string;
  ativo?: boolean;
  genero?: number;
  possuiDeficiencia?: boolean;
  faixaEtaria?: string;
  idadeMin?: number;
  idadeMax?: number;
  cidade?: string;
  estado?: string;
  possuiMatriculaAtiva?: boolean;
}

interface FilterOption {
  label: string;
  value: any;
}

@Component({
  selector: 'app-alunos-filter',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    CardModule,
    InputTextModule,
    DropdownModule,
    ButtonModule,
    MultiSelectModule,
    CalendarModule,
    CheckboxModule,
    AccordionModule
  ],
  template: `
    <p-card>
      <ng-template pTemplate="header">
        <div class="p-3">
          <div class="flex justify-content-between align-items-center">
            <h3 class="text-lg font-medium text-900 m-0">
              <i class="pi pi-filter mr-2"></i>
              Filtros de Busca
            </h3>
            <p-button 
              icon="pi pi-refresh" 
              styleClass="p-button-rounded p-button-text p-button-sm"
              pTooltip="Limpar Filtros"
              (onClick)="clearAllFilters()">
            </p-button>
          </div>
        </div>
      </ng-template>

      <div class="filter-content">
        <!-- Busca Rápida -->
        <div class="field mb-3">
          <label for="search" class="block text-900 font-medium mb-2">Busca Rápida</label>
          <span class="p-input-icon-left w-full">
            <i class="pi pi-search"></i>
            <input 
              pInputText 
              id="search"
              [(ngModel)]="filters.searchTerm" 
              placeholder="Nome, CPF, email ou telefone..."
              class="w-full"
              (input)="onFilterChange()">
          </span>
        </div>

        <!-- Filtros Básicos -->
        <div class="grid">
          <div class="col-12 md:col-6">
            <label for="escola" class="block text-900 font-medium mb-2">Escola</label>
            <p-dropdown 
              id="escola"
              [options]="escolaOptions" 
              [(ngModel)]="filters.escolaId"
              placeholder="Todas as escolas"
              optionLabel="label"
              optionValue="value"
              class="w-full"
              [showClear]="true"
              (onChange)="onFilterChange()">
            </p-dropdown>
          </div>

          <div class="col-12 md:col-6">
            <label for="turma" class="block text-900 font-medium mb-2">Turma</label>
            <p-dropdown 
              id="turma"
              [options]="turmaOptions" 
              [(ngModel)]="filters.turmaId"
              placeholder="Todas as turmas"
              optionLabel="label"
              optionValue="value"
              class="w-full"
              [showClear]="true"
              (onChange)="onFilterChange()">
            </p-dropdown>
          </div>

          <div class="col-12 md:col-4">
            <label for="status" class="block text-900 font-medium mb-2">Status</label>
            <p-dropdown 
              id="status"
              [options]="statusOptions" 
              [(ngModel)]="filters.ativo"
              placeholder="Todos os status"
              optionLabel="label"
              optionValue="value"
              class="w-full"
              [showClear]="true"
              (onChange)="onFilterChange()">
            </p-dropdown>
          </div>

          <div class="col-12 md:col-4">
            <label for="genero" class="block text-900 font-medium mb-2">Gênero</label>
            <p-dropdown 
              id="genero"
              [options]="generoOptions" 
              [(ngModel)]="filters.genero"
              placeholder="Todos os gêneros"
              optionLabel="label"
              optionValue="value"
              class="w-full"
              [showClear]="true"
              (onChange)="onFilterChange()">
            </p-dropdown>
          </div>

          <div class="col-12 md:col-4">
            <label for="matricula" class="block text-900 font-medium mb-2">Matrícula</label>
            <p-dropdown 
              id="matricula"
              [options]="matriculaOptions" 
              [(ngModel)]="filters.possuiMatriculaAtiva"
              placeholder="Todas as situações"
              optionLabel="label"
              optionValue="value"
              class="w-full"
              [showClear]="true"
              (onChange)="onFilterChange()">
            </p-dropdown>
          </div>
        </div>

        <!-- Filtros Avançados -->
        <p-accordion *ngIf="showAdvancedFilters">
          <p-accordionTab header="Filtros Avançados" [selected]="false">
            <div class="grid">
              <!-- Faixa Etária -->
              <div class="col-12 md:col-4">
                <label for="faixaEtaria" class="block text-900 font-medium mb-2">Faixa Etária Escolar</label>
                <p-dropdown 
                  id="faixaEtaria"
                  [options]="faixaEtariaOptions" 
                  [(ngModel)]="filters.faixaEtaria"
                  placeholder="Todas as faixas"
                  optionLabel="label"
                  optionValue="value"
                  class="w-full"
                  [showClear]="true"
                  (onChange)="onFilterChange()">
                </p-dropdown>
              </div>

              <!-- Idade Mínima -->
              <div class="col-12 md:col-4">
                <label for="idadeMin" class="block text-900 font-medium mb-2">Idade Mínima</label>
                <input 
                  pInputText 
                  id="idadeMin"
                  [(ngModel)]="filters.idadeMin" 
                  placeholder="Ex: 5"
                  type="number"
                  min="0"
                  max="100"
                  class="w-full"
                  (input)="onFilterChange()">
              </div>

              <!-- Idade Máxima -->
              <div class="col-12 md:col-4">
                <label for="idadeMax" class="block text-900 font-medium mb-2">Idade Máxima</label>
                <input 
                  pInputText 
                  id="idadeMax"
                  [(ngModel)]="filters.idadeMax" 
                  placeholder="Ex: 18"
                  type="number"
                  min="0"
                  max="100"
                  class="w-full"
                  (input)="onFilterChange()">
              </div>

              <!-- Localização -->
              <div class="col-12 md:col-6">
                <label for="cidade" class="block text-900 font-medium mb-2">Cidade</label>
                <input 
                  pInputText 
                  id="cidade"
                  [(ngModel)]="filters.cidade" 
                  placeholder="Nome da cidade"
                  class="w-full"
                  (input)="onFilterChange()">
              </div>

              <div class="col-12 md:col-6">
                <label for="estado" class="block text-900 font-medium mb-2">Estado</label>
                <p-dropdown 
                  id="estado"
                  [options]="estadoOptions" 
                  [(ngModel)]="filters.estado"
                  placeholder="Todos os estados"
                  optionLabel="label"
                  optionValue="value"
                  class="w-full"
                  [showClear]="true"
                  (onChange)="onFilterChange()">
                </p-dropdown>
              </div>

              <!-- Checkboxes -->
              <div class="col-12">
                <div class="flex gap-4">
                  <p-checkbox 
                    [(ngModel)]="filters.possuiDeficiencia"
                    [binary]="true"
                    label="Apenas alunos com deficiência"
                    (onChange)="onFilterChange()">
                  </p-checkbox>
                </div>
              </div>
            </div>
          </p-accordionTab>
        </p-accordion>

        <!-- Actions -->
        <div class="flex justify-content-between align-items-center mt-4">
          <div class="flex align-items-center gap-2">
            <span class="text-600 text-sm">{{ activeFiltersCount }} filtro(s) ativo(s)</span>
          </div>
          <div class="flex gap-2">
            <p-button 
              label="Limpar Tudo" 
              icon="pi pi-filter-slash" 
              styleClass="p-button-outlined p-button-sm"
              (onClick)="clearAllFilters()"
              [disabled]="activeFiltersCount === 0">
            </p-button>
            <p-button 
              label="Aplicar Filtros" 
              icon="pi pi-check" 
              styleClass="p-button-sm"
              (onClick)="applyFilters()">
            </p-button>
          </div>
        </div>
      </div>
    </p-card>
  `,
  styles: [`
    .filter-content {
      padding: 1rem;
    }

    .field {
      margin-bottom: 1rem;
    }

    :host ::ng-deep .p-card .p-card-body {
      padding: 0;
    }

    :host ::ng-deep .p-accordion .p-accordion-content {
      padding: 1rem;
    }

    :host ::ng-deep .p-dropdown,
    :host ::ng-deep .p-inputtext {
      height: 2.5rem;
    }

    :host ::ng-deep .p-checkbox .p-checkbox-label {
      margin-left: 0.5rem;
    }
  `]
})
export class AlunosFilterComponent implements OnInit {
  @Input() showAdvancedFilters = true;
  @Input() initialFilters: AlunosFilterCriteria = {};
  
  @Output() filtersChange = new EventEmitter<AlunosFilterCriteria>();
  @Output() filtersApply = new EventEmitter<AlunosFilterCriteria>();
  @Output() filtersClear = new EventEmitter<void>();

  filters: AlunosFilterCriteria = {};

  // Options for dropdowns
  escolaOptions: FilterOption[] = [
    { label: 'Escola Municipal João Silva', value: '1' },
    { label: 'Escola Estadual Maria Santos', value: '2' },
    { label: 'Colégio Particular ABC', value: '3' }
  ];

  turmaOptions: FilterOption[] = [
    { label: '1º Ano A', value: '1' },
    { label: '2º Ano A', value: '2' },
    { label: '3º Ano A', value: '3' },
    { label: '4º Ano A', value: '4' },
    { label: '5º Ano A', value: '5' },
    { label: '6º Ano A', value: '6' },
    { label: '7º Ano A', value: '7' },
    { label: '8º Ano A', value: '8' },
    { label: '9º Ano A', value: '9' }
  ];

  statusOptions: FilterOption[] = [
    { label: 'Ativos', value: true },
    { label: 'Inativos', value: false }
  ];

  generoOptions: FilterOption[] = [
    { label: 'Masculino', value: 1 },
    { label: 'Feminino', value: 2 },
    { label: 'Não Binário', value: 3 },
    { label: 'Não Informado', value: 0 }
  ];

  matriculaOptions: FilterOption[] = [
    { label: 'Com matrícula ativa', value: true },
    { label: 'Sem matrícula ativa', value: false }
  ];

  faixaEtariaOptions: FilterOption[] = [
    { label: 'Educação Infantil', value: 'Educação Infantil' },
    { label: 'Ensino Fundamental', value: 'Ensino Fundamental' },
    { label: 'Ensino Médio', value: 'Ensino Médio' },
    { label: 'Educação de Jovens e Adultos', value: 'Educação de Jovens e Adultos' }
  ];

  estadoOptions: FilterOption[] = [
    { label: 'São Paulo', value: 'SP' },
    { label: 'Rio de Janeiro', value: 'RJ' },
    { label: 'Minas Gerais', value: 'MG' },
    { label: 'Bahia', value: 'BA' },
    { label: 'Paraná', value: 'PR' },
    { label: 'Rio Grande do Sul', value: 'RS' },
    { label: 'Pernambuco', value: 'PE' },
    { label: 'Ceará', value: 'CE' },
    { label: 'Pará', value: 'PA' },
    { label: 'Santa Catarina', value: 'SC' }
  ];

  ngOnInit(): void {
    this.filters = { ...this.initialFilters };
  }

  get activeFiltersCount(): number {
    return Object.values(this.filters).filter(value => 
      value !== undefined && value !== null && value !== ''
    ).length;
  }

  onFilterChange(): void {
    this.filtersChange.emit({ ...this.filters });
  }

  applyFilters(): void {
    this.filtersApply.emit({ ...this.filters });
  }

  clearAllFilters(): void {
    this.filters = {};
    this.filtersClear.emit();
    this.onFilterChange();
  }

  clearFilter(filterKey: keyof AlunosFilterCriteria): void {
    delete this.filters[filterKey];
    this.onFilterChange();
  }

  hasActiveFilters(): boolean {
    return this.activeFiltersCount > 0;
  }

  getActiveFiltersText(): string {
    const activeFilters: string[] = [];
    
    if (this.filters.searchTerm) {
      activeFilters.push(`Busca: "${this.filters.searchTerm}"`);
    }
    
    if (this.filters.ativo !== undefined) {
      activeFilters.push(`Status: ${this.filters.ativo ? 'Ativo' : 'Inativo'}`);
    }
    
    if (this.filters.escolaId) {
      const escola = this.escolaOptions.find(e => e.value === this.filters.escolaId);
      if (escola) activeFilters.push(`Escola: ${escola.label}`);
    }
    
    if (this.filters.turmaId) {
      const turma = this.turmaOptions.find(t => t.value === this.filters.turmaId);
      if (turma) activeFilters.push(`Turma: ${turma.label}`);
    }
    
    return activeFilters.join(', ');
  }
}