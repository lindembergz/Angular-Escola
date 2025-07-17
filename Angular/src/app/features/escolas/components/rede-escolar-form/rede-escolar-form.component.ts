import { Component, OnInit, OnDestroy, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

// PrimeNG Components (Mixed standalone and module imports for v19)
import { DialogModule } from 'primeng/dialog';
import { Button } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

// Local Imports
import { RedeEscolar, CreateRedeEscolarRequest } from '../../models/escola.model';
import * as EscolasActions from '../../store/escolas.actions';
import * as EscolasSelectors from '../../store/escolas.selectors';

@Component({
  selector: 'app-rede-escolar-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DialogModule,
    Button,
    InputText,
    DropdownModule,
    ToastModule
  ],
  providers: [MessageService],
  template: `
    <p-dialog 
      [header]="isEditMode ? 'Editar Rede Escolar' : 'Nova Rede Escolar'"
      [visible]="visible"
      [modal]="true"
      [closable]="true"
      [draggable]="false"
      [resizable]="false"
      styleClass="p-fluid"
      [style]="{ width: '600px' }"
      (onHide)="onCancel()">
      
      <form [formGroup]="redeForm" (ngSubmit)="onSubmit()">
        <div class="form-grid">
          <!-- Nome da Rede -->
          <div class="form-field">
            <label for="nome">Nome da Rede Escolar *</label>
            <input 
              pInputText 
              id="nome" 
              formControlName="nome"
              placeholder="Digite o nome da rede escolar"
              [class.ng-invalid]="isFieldInvalid('nome')">
            <small class="p-error" *ngIf="isFieldInvalid('nome')">
              {{ getFieldError('nome') }}
            </small>
          </div>

          <!-- CNPJ da Mantenedora -->
          <div class="form-field">
            <label for="cnpjMantenedora">CNPJ da Mantenedora *</label>
            <input 
              pInputText 
              id="cnpjMantenedora" 
              formControlName="cnpjMantenedora"
              placeholder="00.000.000/0000-00"
              [class.ng-invalid]="isFieldInvalid('cnpjMantenedora')">
            <small class="p-error" *ngIf="isFieldInvalid('cnpjMantenedora')">
              {{ getFieldError('cnpjMantenedora') }}
            </small>
          </div>

          <!-- Endereço da Sede - Logradouro -->
          <div class="form-field full-width">
            <label for="logradouro">Endereço da Sede *</label>
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
      </form>

      <ng-template pTemplate="footer">
        <div class="dialog-footer">
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
            type="button"
            [loading]="submitting"
            [disabled]="redeForm.invalid || submitting"
            (onClick)="onSubmit()">
          </p-button>
        </div>
      </ng-template>
    </p-dialog>

    <p-toast></p-toast>
  `,
  styles: [`
    .form-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1.5rem;
      margin-bottom: 1rem;
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
        font-size: 0.875rem;
      }

      .p-error {
        margin-top: 0.25rem;
      }
    }

    .dialog-footer {
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
    }

    :host ::ng-deep {
      .p-inputtext.ng-invalid.ng-dirty,
      .p-dropdown.ng-invalid.ng-dirty {
        border-color: var(--red-500);
      }

      .p-dialog .p-dialog-content {
        padding: 1.5rem;
      }

      .p-dialog .p-dialog-footer {
        padding: 1rem 1.5rem;
        border-top: 1px solid var(--surface-border);
      }
    }

    @media screen and (max-width: 768px) {
      .form-grid {
        grid-template-columns: 1fr;
      }

      .dialog-footer {
        flex-direction: column-reverse;
        
        p-button {
          width: 100%;
        }
      }
    }
  `]
})
export class RedeEscolarFormComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  @Input() visible = false;
  @Input() redeEscolar: RedeEscolar | null = null;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() saved = new EventEmitter<RedeEscolar>();
  @Output() cancelled = new EventEmitter<void>();

  redeForm!: FormGroup;
  isEditMode = false;
  submitting = false;

  // Observables
  loading$!: Observable<boolean>;
  error$!: Observable<string | null>;

  // Options
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
    private messageService: MessageService
  ) {
    this.createForm();
    this.initializeObservables();
  }

  ngOnInit(): void {
    this.setupSuccessHandling();
    this.setupErrorHandling();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  ngOnChanges(): void {
    if (this.redeEscolar) {
      this.isEditMode = true;
      this.populateForm(this.redeEscolar);
    } else {
      this.isEditMode = false;
      this.redeForm.reset();
    }
  }

  private createForm(): void {
    this.redeForm = this.fb.group({
      nome: ['', [Validators.required, Validators.minLength(3)]],
      cnpjMantenedora: ['', [Validators.required, Validators.pattern(/^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$/)]],
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
    this.loading$ = this.store.select(EscolasSelectors.selectRedesEscolaresLoading);
    this.error$ = this.store.select(EscolasSelectors.selectRedesEscolaresError);
  }

  private populateForm(rede: RedeEscolar): void {
    this.redeForm.patchValue({
      nome: rede.nome,
      cnpjMantenedora: rede.cnpjMantenedora,
      logradouro: rede.enderecoSede?.logradouro || '',
      numero: rede.enderecoSede?.numero || '',
      complemento: rede.enderecoSede?.complemento || '',
      bairro: rede.enderecoSede?.bairro || '',
      cidade: rede.enderecoSede?.cidade || '',
      estado: rede.enderecoSede?.estado || '',
      cep: rede.enderecoSede?.cep || ''
    });
  }

  private setupSuccessHandling(): void {
    // Handle create/update success
    this.store.select(EscolasSelectors.selectRedesEscolaresError)
      .pipe(
        takeUntil(this.destroy$)
      )
      .subscribe(error => {
        if (error === null && this.submitting) {
          this.submitting = false;
          this.messageService.add({
            severity: 'success',
            summary: 'Sucesso',
            detail: this.isEditMode ? 'Rede escolar atualizada com sucesso!' : 'Rede escolar criada com sucesso!'
          });
          
          this.onCancel();
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
    const field = this.redeForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.redeForm.get(fieldName);
    if (field && field.errors) {
      if (field.errors['required']) {
        return 'Este campo é obrigatório';
      }
      if (field.errors['minlength']) {
        return `Mínimo de ${field.errors['minlength'].requiredLength} caracteres`;
      }
      if (field.errors['pattern']) {
        if (fieldName === 'cnpjMantenedora') {
          return 'CNPJ deve estar no formato 00.000.000/0000-00';
        }
        if (fieldName === 'cep') {
          return 'CEP deve estar no formato 00000-000';
        }
      }
    }
    return '';
  }

  onSubmit(): void {
    if (this.redeForm.valid && !this.submitting) {
      this.submitting = true;
      
      const formValue = this.redeForm.value;
      
      const redeData: CreateRedeEscolarRequest = {
        nome: formValue.nome,
        cnpjMantenedora: formValue.cnpjMantenedora,
        enderecoSede: {
          logradouro: formValue.logradouro,
          numero: formValue.numero,
          complemento: formValue.complemento,
          bairro: formValue.bairro,
          cidade: formValue.cidade,
          estado: formValue.estado,
          cep: formValue.cep
        }
      };
      
      this.store.dispatch(EscolasActions.createRedeEscolar({ rede: redeData }));
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.redeForm.controls).forEach(key => {
        this.redeForm.get(key)?.markAsTouched();
      });
      
      this.messageService.add({
        severity: 'warn',
        summary: 'Atenção',
        detail: 'Por favor, corrija os erros no formulário'
      });
    }
  }

  onCancel(): void {
    this.visible = false;
    this.visibleChange.emit(false);
    this.cancelled.emit();
    this.redeForm.reset();
    this.submitting = false;
  }
}