import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil, filter } from 'rxjs/operators';

// PrimeNG Components (Mixed standalone and module imports for v19)
import { Card } from 'primeng/card';
import { Button } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { ToastModule } from 'primeng/toast';
import { ProgressSpinner } from 'primeng/progressspinner';
import { MenuItem, MessageService } from 'primeng/api';

// Local Imports
import { PageHeaderComponent } from '../../../../shared/components/page-header/page-header.component';
import { Escola, RedeEscolar, CreateEscolaRequest, UpdateEscolaRequest, TipoEscola } from '../../models/escola.model';
import * as EscolasActions from '../../store/escolas.actions';
import * as EscolasSelectors from '../../store/escolas.selectors';

@Component({
  selector: 'app-escola-form',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    Card,
    Button,
    InputText,
    DropdownModule,
    ToastModule,
    ProgressSpinner,
    PageHeaderComponent
  ],
  providers: [MessageService],
  template: `
    <app-page-header
      [title]="isEditMode ? 'Editar Escola' : 'Nova Escola'"
      [subtitle]="isEditMode ? 'Edite as informações da escola' : 'Cadastre uma nova unidade escolar'"
      icon="pi pi-building"
      [breadcrumbItems]="breadcrumbItems">
    </app-page-header>

    <div class="content-container">
      <p-card>
        <div *ngIf="loading$ | async" class="loading-container">
          <p-progressSpinner></p-progressSpinner>
          <p>Carregando dados...</p>
        </div>

        <form [formGroup]="escolaForm" (ngSubmit)="onSubmit()" *ngIf="!(loading$ | async)">
          <div class="form-grid">
            <!-- Nome da Escola -->
            <div class="form-field">
              <label for="nome">Nome da Escola *</label>
              <input 
                pInputText 
                id="nome" 
                formControlName="nome"
                placeholder="Digite o nome da escola"
                [class.ng-invalid]="isFieldInvalid('nome')">
              <small class="p-error" *ngIf="isFieldInvalid('nome')">
                {{ getFieldError('nome') }}
              </small>
            </div>

            <!-- Tipo de Escola -->
            <div class="form-field">
              <label for="tipo">Tipo de Escola *</label>
              <p-dropdown 
                id="tipo"
                formControlName="tipo"
                [options]="tiposEscola"
                placeholder="Selecione o tipo"
                optionLabel="label"
                optionValue="value"
                [class.ng-invalid]="isFieldInvalid('tipo')">
              </p-dropdown>
              <small class="p-error" *ngIf="isFieldInvalid('tipo')">
                {{ getFieldError('tipo') }}
              </small>
            </div>

            <!-- CNPJ -->
            <div class="form-field">
              <label for="cnpj">CNPJ</label>
              <input 
                pInputText 
                id="cnpj" 
                formControlName="cnpj"
                placeholder="00.000.000/0000-00"
                [class.ng-invalid]="isFieldInvalid('cnpj')">
              <small class="p-error" *ngIf="isFieldInvalid('cnpj')">
                {{ getFieldError('cnpj') }}
              </small>
            </div>

            <!-- Rede Escolar -->
            <div class="form-field">
              <label for="redeEscolarId">Rede Escolar</label>
              <p-dropdown 
                id="redeEscolarId"
                formControlName="redeEscolarId"
                [options]="(redesEscolares$ | async) || []"
                placeholder="Selecione uma rede (opcional)"
                optionLabel="nome"
                optionValue="id"
                [showClear]="true">
              </p-dropdown>
            </div>

            <!-- Endereço - Logradouro -->
            <div class="form-field full-width">
              <label for="logradouro">Endereço *</label>
              <input 
                pInputText 
                id="logradouro" 
                formControlName="logradouro"
                placeholder="Rua, Avenida, etc."
                [class.ng-invalid]="isFieldInvalid('logradouro')">
              <small class="p-error" *ngIf="isFieldInvalid('logradouro')">
                {{ getFieldError('logradouro') }}
              </small>
            </div>

            <!-- Número -->
            <div class="form-field">
              <label for="numero">Número *</label>
              <input 
                pInputText 
                id="numero" 
                formControlName="numero"
                placeholder="123"
                [class.ng-invalid]="isFieldInvalid('numero')">
              <small class="p-error" *ngIf="isFieldInvalid('numero')">
                {{ getFieldError('numero') }}
              </small>
            </div>

            <!-- Complemento -->
            <div class="form-field">
              <label for="complemento">Complemento</label>
              <input 
                pInputText 
                id="complemento" 
                formControlName="complemento"
                placeholder="Apto, Sala, etc.">
            </div>

            <!-- Bairro -->
            <div class="form-field">
              <label for="bairro">Bairro *</label>
              <input 
                pInputText 
                id="bairro" 
                formControlName="bairro"
                placeholder="Nome do bairro"
                [class.ng-invalid]="isFieldInvalid('bairro')">
              <small class="p-error" *ngIf="isFieldInvalid('bairro')">
                {{ getFieldError('bairro') }}
              </small>
            </div>

            <!-- Cidade -->
            <div class="form-field">
              <label for="cidade">Cidade *</label>
              <input 
                pInputText 
                id="cidade" 
                formControlName="cidade"
                placeholder="Nome da cidade"
                [class.ng-invalid]="isFieldInvalid('cidade')">
              <small class="p-error" *ngIf="isFieldInvalid('cidade')">
                {{ getFieldError('cidade') }}
              </small>
            </div>

            <!-- Estado -->
            <div class="form-field">
              <label for="estado">Estado *</label>
              <p-dropdown 
                id="estado"
                formControlName="estado"
                [options]="estados"
                placeholder="Selecione o estado"
                optionLabel="label"
                optionValue="value"
                [class.ng-invalid]="isFieldInvalid('estado')">
              </p-dropdown>
              <small class="p-error" *ngIf="isFieldInvalid('estado')">
                {{ getFieldError('estado') }}
              </small>
            </div>

            <!-- CEP -->
            <div class="form-field">
              <label for="cep">CEP *</label>
              <input 
                pInputText 
                id="cep" 
                formControlName="cep"
                placeholder="00000-000"
                [class.ng-invalid]="isFieldInvalid('cep')">
              <small class="p-error" *ngIf="isFieldInvalid('cep')">
                {{ getFieldError('cep') }}
              </small>
            </div>
          </div>

          <div class="form-actions">
            <p-button 
              label="Cancelar" 
              icon="pi pi-times" 
              severity="secondary"
              [outlined]="true"
              type="button"
              (onClick)="onCancel()">
            </p-button>
            <p-button 
              [label]="isEditMode ? 'Atualizar' : 'Salvar'" 
              icon="pi pi-check" 
              type="submit"
              [loading]="submitting"
              [disabled]="escolaForm.invalid || submitting">
            </p-button>
          </div>
        </form>
      </p-card>
    </div>

    <p-toast></p-toast>
  `,
  styles: [`
    .content-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 0 1rem;
    }

    .loading-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 3rem;
      gap: 1rem;
      
      p {
        color: var(--text-color-secondary);
        margin: 0;
      }
    }

    .form-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 1.5rem;
      margin-bottom: 2rem;
    }

    .form-field {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;

      &.full-width {
        grid-column: 1 / -1;
      }

      label {
        font-weight: 500;
        color: var(--text-color);
      }

      .p-error {
        margin-top: 0.25rem;
      }
    }

    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
      padding-top: 1rem;
      border-top: 1px solid var(--surface-border);
    }

    :host ::ng-deep {
      .p-inputtext.ng-invalid.ng-dirty,
      .p-dropdown.ng-invalid.ng-dirty {
        border-color: var(--red-500);
      }
    }

    @media screen and (max-width: 768px) {
      .form-grid {
        grid-template-columns: 1fr;
      }

      .form-actions {
        flex-direction: column-reverse;
        
        p-button {
          width: 100%;
        }
      }
    }
  `]
})
export class EscolaFormComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  escolaForm!: FormGroup;
  isEditMode = false;
  escolaId: string | null = null;
  submitting = false;
  
  breadcrumbItems: MenuItem[] = [];

  // Observables
  loading$!: Observable<boolean>;
  error$!: Observable<string | null>;
  redesEscolares$!: Observable<RedeEscolar[]>;

  // Options
  tiposEscola = [
    { label: 'Infantil', value: TipoEscola.INFANTIL },
    { label: 'Fundamental', value: TipoEscola.FUNDAMENTAL },
    { label: 'Médio', value: TipoEscola.MEDIO },
    { label: 'Fundamental e Médio', value: TipoEscola.FUNDAMENTAL_E_MEDIO },
    { label: 'Técnico', value: TipoEscola.TECNICO },
    { label: 'EJA', value: TipoEscola.EJA }
  ];

  estados = [
    { label: 'Acre', value: 'AC' },
    { label: 'Alagoas', value: 'AL' },
    { label: 'Amapá', value: 'AP' },
    { label: 'Amazonas', value: 'AM' },
    { label: 'Bahia', value: 'BA' },
    { label: 'Ceará', value: 'CE' },
    { label: 'Distrito Federal', value: 'DF' },
    { label: 'Espírito Santo', value: 'ES' },
    { label: 'Goiás', value: 'GO' },
    { label: 'Maranhão', value: 'MA' },
    { label: 'Mato Grosso', value: 'MT' },
    { label: 'Mato Grosso do Sul', value: 'MS' },
    { label: 'Minas Gerais', value: 'MG' },
    { label: 'Pará', value: 'PA' },
    { label: 'Paraíba', value: 'PB' },
    { label: 'Paraná', value: 'PR' },
    { label: 'Pernambuco', value: 'PE' },
    { label: 'Piauí', value: 'PI' },
    { label: 'Rio de Janeiro', value: 'RJ' },
    { label: 'Rio Grande do Norte', value: 'RN' },
    { label: 'Rio Grande do Sul', value: 'RS' },
    { label: 'Rondônia', value: 'RO' },
    { label: 'Roraima', value: 'RR' },
    { label: 'Santa Catarina', value: 'SC' },
    { label: 'São Paulo', value: 'SP' },
    { label: 'Sergipe', value: 'SE' },
    { label: 'Tocantins', value: 'TO' }
  ];

  constructor(
    private fb: FormBuilder,
    private store: Store,
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService
  ) {
    this.createForm();
    this.initializeObservables();
  }

  ngOnInit(): void {
    this.checkEditMode();
    this.loadRedesEscolares();
    this.setupSuccessHandling();
    this.setupErrorHandling();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private createForm(): void {
    this.escolaForm = this.fb.group({
      nome: ['', [Validators.required, Validators.minLength(3)]],
      tipo: ['', Validators.required],
      cnpj: ['', [Validators.pattern(/^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$/)]],
      redeEscolarId: [null],
      logradouro: ['', Validators.required],
      numero: ['', Validators.required],
      complemento: [''],
      bairro: ['', Validators.required],
      cidade: ['', Validators.required],
      estado: ['', Validators.required],
      cep: ['', [Validators.required, Validators.pattern(/^\d{5}-?\d{3}$/)]]
    });
  }

  private initializeObservables(): void {
    this.loading$ = this.store.select(EscolasSelectors.selectEscolasLoading);
    this.error$ = this.store.select(EscolasSelectors.selectEscolasError);
    this.redesEscolares$ = this.store.select(EscolasSelectors.selectAllRedesEscolares);
  }

  private checkEditMode(): void {
    this.route.params
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        if (params['id']) {
          this.isEditMode = true;
          this.escolaId = params['id'];
          this.loadEscolaForEdit();
          this.updateBreadcrumbs();
        } else {
          this.updateBreadcrumbs();
        }
      });
  }

  private loadEscolaForEdit(): void {
    if (this.escolaId) {
      this.store.dispatch(EscolasActions.loadEscolaById({ id: this.escolaId }));
      
      // Subscribe to selected escola and populate form
      this.store.select(EscolasSelectors.selectSelectedEscola)
        .pipe(
          takeUntil(this.destroy$),
          filter(escola => !!escola)
        )
        .subscribe(escola => {
          if (escola) {
            this.populateForm(escola);
          }
        });
    }
  }

  private populateForm(escola: Escola): void {
    this.escolaForm.patchValue({
      nome: escola.nome,
      tipo: escola.tipo,
      cnpj: escola.cnpj,
      redeEscolarId: escola.redeEscolarId,
      logradouro: escola.endereco?.logradouro || '',
      numero: escola.endereco?.numero || '',
      complemento: escola.endereco?.complemento || '',
      bairro: escola.endereco?.bairro || '',
      cidade: escola.endereco?.cidade || '',
      estado: escola.endereco?.estado || '',
      cep: escola.endereco?.cep || ''
    });
  }

  private loadRedesEscolares(): void {
    this.store.dispatch(EscolasActions.loadRedesEscolares());
  }

  private updateBreadcrumbs(): void {
    this.breadcrumbItems = [
      { label: 'Escolas', routerLink: '/escolas' },
      { label: this.isEditMode ? 'Editar Escola' : 'Nova Escola' }
    ];
  }

  private setupSuccessHandling(): void {
    // Handle create success
    this.store.select(EscolasSelectors.selectEscolasError)
      .pipe(
        takeUntil(this.destroy$),
        filter(error => error === null)
      )
      .subscribe(() => {
        if (this.submitting) {
          this.submitting = false;
          this.messageService.add({
            severity: 'success',
            summary: 'Sucesso',
            detail: this.isEditMode ? 'Escola atualizada com sucesso!' : 'Escola criada com sucesso!'
          });
          
          // Navigate back to list after a short delay
          setTimeout(() => {
            this.router.navigate(['/escolas']);
          }, 1500);
        }
      });
  }

  private setupErrorHandling(): void {
    this.error$
      .pipe(takeUntil(this.destroy$))
      .subscribe(error => {
        if (error && this.submitting) {
          this.submitting = false;
          this.messageService.add({
            severity: 'error',
            summary: 'Erro',
            detail: error
          });
        }
      });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.escolaForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.escolaForm.get(fieldName);
    if (field && field.errors) {
      if (field.errors['required']) {
        return 'Este campo é obrigatório';
      }
      if (field.errors['minlength']) {
        return `Mínimo de ${field.errors['minlength'].requiredLength} caracteres`;
      }
      if (field.errors['pattern']) {
        if (fieldName === 'cnpj') {
          return 'CNPJ deve estar no formato 00.000.000/0000-00';
        }
        if (fieldName === 'cep') {
          return 'CEP deve estar no formato 00000-000';
        }
      }
      if (field.errors['email']) {
        return 'Email deve ter um formato válido';
      }
    }
    return '';
  }

  onSubmit(): void {
    if (this.escolaForm.valid && !this.submitting) {
      this.submitting = true;
      
      const formValue = this.escolaForm.value;
      
      if (this.isEditMode && this.escolaId) {
        const updateData: UpdateEscolaRequest = {
          id: this.escolaId,
          nome: formValue.nome,
          endereco: {
            logradouro: formValue.logradouro,
            numero: formValue.numero,
            complemento: formValue.complemento,
            bairro: formValue.bairro,
            cidade: formValue.cidade,
            estado: formValue.estado,
            cep: formValue.cep
          },
          tipo: formValue.tipo
        };
        
        this.store.dispatch(EscolasActions.updateEscola({ escola: updateData }));
      } else {
        const createData: CreateEscolaRequest = {
          nome: formValue.nome,
          cnpj: formValue.cnpj,
          endereco: {
            logradouro: formValue.logradouro,
            numero: formValue.numero,
            complemento: formValue.complemento,
            bairro: formValue.bairro,
            cidade: formValue.cidade,
            estado: formValue.estado,
            cep: formValue.cep
          },
          tipo: formValue.tipo,
          redeEscolarId: formValue.redeEscolarId
        };
        
        this.store.dispatch(EscolasActions.createEscola({ escola: createData }));
      }
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.escolaForm.controls).forEach(key => {
        this.escolaForm.get(key)?.markAsTouched();
      });
      
      this.messageService.add({
        severity: 'warn',
        summary: 'Atenção',
        detail: 'Por favor, corrija os erros no formulário'
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/escolas']);
  }
}