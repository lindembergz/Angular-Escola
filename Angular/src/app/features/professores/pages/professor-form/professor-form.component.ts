import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil, filter } from 'rxjs/operators';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputMaskModule } from 'primeng/inputmask';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextarea } from 'primeng/inputtextarea';
import { ToastModule } from 'primeng/toast';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DividerModule } from 'primeng/divider';
import { FieldsetModule } from 'primeng/fieldset';

import { MessageService } from 'primeng/api';

// Store
import { AppState } from '../../../../store/app.state';
import * as ProfessoresActions from '../../../../store/professores/professores.actions';
import * as ProfessoresSelectors from '../../../../store/professores/professores.selectors';

// Models
import { 
  Professor, 
  CriarProfessorRequest, 
  AtualizarProfessorRequest,
  CriarTituloRequest
} from '../../models/professor.model';

// Services
import { ProfessoresService } from '../../services/professores.service';

@Component({
  selector: 'app-professor-form',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    CardModule,
    ButtonModule,
    InputTextModule,
    InputMaskModule,
    CalendarModule,
    DropdownModule,
    InputTextarea,
    ToastModule,
    ProgressSpinnerModule,
    DividerModule,
    FieldsetModule
  ],
  providers: [MessageService],
  template: `
    <div class="professor-form-container">
      <p-toast></p-toast>

      <!-- Header -->
      <div class="page-header mb-4">
        <div class="flex justify-content-between align-items-center">
          <div>
            <h1 class="text-3xl font-bold text-900">
              {{ isEditMode ? 'Editar Professor' : 'Novo Professor' }}
            </h1>
            <p class="text-600 mt-2">
              {{ isEditMode ? 'Atualize as informações do professor' : 'Cadastre um novo professor no sistema' }}
            </p>
          </div>
          
          <p-button 
            label="Voltar" 
            icon="pi pi-arrow-left" 
            routerLink="/professores"
            class="p-button-outlined">
          </p-button>
        </div>
      </div>

      <!-- Loading Spinner -->
      <div *ngIf="(formState$ | async)?.loading" class="flex justify-content-center p-4">
        <p-progressSpinner></p-progressSpinner>
      </div>

      <!-- Form -->
      <form [formGroup]="professorForm" (ngSubmit)="onSubmit()" *ngIf="!(formState$ | async)?.loading">
        
        <!-- Dados Pessoais -->
        <p-card header="Dados Pessoais" class="mb-4">
          <div class="grid">
            <div class="col-12 md:col-8">
              <label for="nome" class="block text-900 font-medium mb-2">
                Nome Completo *
              </label>
              <input 
                id="nome"
                type="text" 
                pInputText 
                formControlName="nome"
                placeholder="Digite o nome completo"
                class="w-full"
                [class.ng-invalid]="isFieldInvalid('nome')"
              />
              <small 
                *ngIf="isFieldInvalid('nome')" 
                class="p-error block mt-1">
                Nome é obrigatório
              </small>
            </div>

            <div class="col-12 md:col-4">
              <label for="cpf" class="block text-900 font-medium mb-2">
                CPF *
              </label>
              <p-inputMask 
                id="cpf"
                mask="999.999.999-99"
                formControlName="cpf"
                placeholder="000.000.000-00"
                class="w-full"
                [class.ng-invalid]="isFieldInvalid('cpf')"
              ></p-inputMask>
              <small 
                *ngIf="isFieldInvalid('cpf')" 
                class="p-error block mt-1">
                CPF é obrigatório e deve ser válido
              </small>
            </div>

            <div class="col-12 md:col-4">
              <label for="registro" class="block text-900 font-medium mb-2">
                Registro Profissional *
              </label>
              <input 
                id="registro"
                type="text" 
                pInputText 
                formControlName="registro"
                placeholder="Ex: 123456"
                class="w-full"
                [class.ng-invalid]="isFieldInvalid('registro')"
              />
              <small 
                *ngIf="isFieldInvalid('registro')" 
                class="p-error block mt-1">
                Registro é obrigatório
              </small>
            </div>

            <div class="col-12 md:col-4">
              <label for="email" class="block text-900 font-medium mb-2">
                E-mail
              </label>
              <input 
                id="email"
                type="email" 
                pInputText 
                formControlName="email"
                placeholder="professor@escola.com"
                class="w-full"
                [class.ng-invalid]="isFieldInvalid('email')"
              />
              <small 
                *ngIf="isFieldInvalid('email')" 
                class="p-error block mt-1">
                E-mail deve ter formato válido
              </small>
            </div>

            <div class="col-12 md:col-4">
              <label for="telefone" class="block text-900 font-medium mb-2">
                Telefone
              </label>
              <p-inputMask 
                id="telefone"
                mask="(99) 99999-9999"
                formControlName="telefone"
                placeholder="(11) 99999-9999"
                class="w-full"
              ></p-inputMask>
            </div>

            <div class="col-12 md:col-6">
              <label for="dataNascimento" class="block text-900 font-medium mb-2">
                Data de Nascimento *
              </label>
              <p-calendar 
                id="dataNascimento"
                formControlName="dataNascimento"
                [showIcon]="true"
                dateFormat="dd/mm/yy"
                placeholder="Selecione a data"
                class="w-full"
                [class.ng-invalid]="isFieldInvalid('dataNascimento')"
              ></p-calendar>
              <small 
                *ngIf="isFieldInvalid('dataNascimento')" 
                class="p-error block mt-1">
                Data de nascimento é obrigatória
              </small>
            </div>

            <div class="col-12 md:col-6">
              <label for="dataContratacao" class="block text-900 font-medium mb-2">
                Data de Contratação *
              </label>
              <p-calendar 
                id="dataContratacao"
                formControlName="dataContratacao"
                [showIcon]="true"
                dateFormat="dd/mm/yy"
                placeholder="Selecione a data"
                class="w-full"
                [class.ng-invalid]="isFieldInvalid('dataContratacao')"
              ></p-calendar>
              <small 
                *ngIf="isFieldInvalid('dataContratacao')" 
                class="p-error block mt-1">
                Data de contratação é obrigatória
              </small>
            </div>

            <div class="col-12">
              <label for="observacoes" class="block text-900 font-medium mb-2">
                Observações
              </label>
              <textarea 
                id="observacoes"
                pInputTextarea 
                formControlName="observacoes"
                placeholder="Observações adicionais sobre o professor"
                [rows]="3"
                class="w-full"
              ></textarea>
            </div>
          </div>
        </p-card>

        <!-- Títulos Acadêmicos -->
        <p-card class="mb-4">
          <ng-template pTemplate="header">
            <div class="flex justify-content-between align-items-center w-full">
              <span class="font-bold">Títulos Acadêmicos</span>
              <p-button 
                label="Adicionar Título" 
                icon="pi pi-plus" 
                (onClick)="addTitulo()"
                class="p-button-sm p-button-outlined">
              </p-button>
            </div>
          </ng-template>

          <div formArrayName="titulos">
            <div 
              *ngFor="let titulo of titulosArray.controls; let i = index" 
              [formGroupName]="i"
              class="mb-4">
              
              <p-fieldset [legend]="'Título ' + (i + 1)" [toggleable]="true">
                <div class="grid">
                  <div class="col-12 md:col-3">
                    <label class="block text-900 font-medium mb-2">
                      Tipo *
                    </label>
                    <p-dropdown 
                      formControlName="tipo"
                      [options]="tiposTitulo"
                      optionLabel="descricao"
                      optionValue="valor"
                      placeholder="Selecione"
                      class="w-full"
                    ></p-dropdown>
                  </div>

                  <div class="col-12 md:col-5">
                    <label class="block text-900 font-medium mb-2">
                      Curso *
                    </label>
                    <input 
                      type="text" 
                      pInputText 
                      formControlName="curso"
                      placeholder="Nome do curso"
                      class="w-full"
                    />
                  </div>

                  <div class="col-12 md:col-4">
                    <label class="block text-900 font-medium mb-2">
                      Instituição *
                    </label>
                    <input 
                      type="text" 
                      pInputText 
                      formControlName="instituicao"
                      placeholder="Nome da instituição"
                      class="w-full"
                    />
                  </div>

                  <div class="col-12 md:col-3">
                    <label class="block text-900 font-medium mb-2">
                      Ano de Formatura *
                    </label>
                    <input 
                      type="number" 
                      pInputText 
                      formControlName="anoFormatura"
                      placeholder="2020"
                      class="w-full"
                      min="1950"
                      [max]="currentYear"
                    />
                  </div>

                  <div class="col-12 md:col-7">
                    <label class="block text-900 font-medium mb-2">
                      Descrição
                    </label>
                    <input 
                      type="text" 
                      pInputText 
                      formControlName="descricao"
                      placeholder="Descrição adicional"
                      class="w-full"
                    />
                  </div>

                  <div class="col-12 md:col-2 flex align-items-end">
                    <p-button 
                      icon="pi pi-trash" 
                      (onClick)="removeTitulo(i)"
                      class="p-button-danger p-button-outlined w-full"
                      pTooltip="Remover título">
                    </p-button>
                  </div>
                </div>
              </p-fieldset>
            </div>

            <div *ngIf="titulosArray.length === 0" class="text-center p-4 border-2 border-dashed border-300 border-round">
              <i class="pi pi-graduation-cap text-4xl text-400 mb-3"></i>
              <p class="text-600">Nenhum título acadêmico adicionado</p>
              <p class="text-500 text-sm">Clique em "Adicionar Título" para começar</p>
            </div>
          </div>
        </p-card>

        <!-- Actions -->
        <div class="flex justify-content-end gap-2">
          <p-button 
            label="Cancelar" 
            icon="pi pi-times" 
            routerLink="/professores"
            class="p-button-outlined">
          </p-button>
          
          <p-button 
            label="{{ isEditMode ? 'Atualizar' : 'Salvar' }}" 
            icon="pi pi-check" 
            type="submit"
            [loading]="(formState$ | async)?.loading"
            [disabled]="professorForm.invalid || (formState$ | async)?.loading"
            class="p-button-success">
          </p-button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .professor-form-container {
      padding: 1rem;
      max-width: 1200px;
      margin: 0 auto;
    }

    :host ::ng-deep {
      .p-card .p-card-body {
        padding: 1.5rem;
      }

      .p-fieldset .p-fieldset-content {
        padding: 1rem;
      }

      .p-calendar,
      .p-dropdown {
        width: 100%;
      }

      .p-inputtext,
      .p-inputtextarea {
        width: 100%;
      }

      .ng-invalid.ng-touched {
        border-color: #e24c4c;
      }
    }
  `]
})
export class ProfessorFormComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  professorForm!: FormGroup;
  isEditMode = false;
  professorId: string | null = null;
  currentYear = new Date().getFullYear();

  // Observables
  formState$: Observable<any>;
  currentProfessor$: Observable<Professor | null>;

  // Options
  tiposTitulo: any[] = [];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private store: Store<AppState>,
    private messageService: MessageService,
    private professoresService: ProfessoresService
  ) {
    this.formState$ = this.store.select(ProfessoresSelectors.selectFormState);
    this.currentProfessor$ = this.store.select(ProfessoresSelectors.selectCurrentProfessor);
  }

  ngOnInit(): void {
    this.initializeForm();
    this.loadOptions();
    this.checkEditMode();
    this.setupFormStateHandling();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.store.dispatch(ProfessoresActions.clearProfessorDetail());
  }

  private initializeForm(): void {
    this.professorForm = this.fb.group({
      nome: ['', [Validators.required, Validators.minLength(2)]],
      cpf: ['', [Validators.required, this.cpfValidator.bind(this)]],
      registro: ['', [Validators.required]],
      email: ['', [Validators.email]],
      telefone: [''],
      dataNascimento: ['', [Validators.required]],
      dataContratacao: ['', [Validators.required]],
      observacoes: [''],
      titulos: this.fb.array([])
    });
  }

  private loadOptions(): void {
    this.store.dispatch(ProfessoresActions.loadOptions());
    
    this.store.select(ProfessoresSelectors.selectOptions).pipe(
      filter(options => !!options),
      takeUntil(this.destroy$)
    ).subscribe(options => {
      this.tiposTitulo = options!.tiposTitulo;
    });
  }

  private checkEditMode(): void {
    this.professorId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.professorId && this.router.url.includes('editar');

    if (this.isEditMode && this.professorId) {
      this.store.dispatch(ProfessoresActions.loadProfessorDetail({ 
        id: this.professorId 
      }));

      this.currentProfessor$.pipe(
        filter(professor => !!professor),
        takeUntil(this.destroy$)
      ).subscribe(professor => {
        this.populateForm(professor!);
      });
    }
  }

  private setupFormStateHandling(): void {
    this.formState$.pipe(
      takeUntil(this.destroy$)
    ).subscribe(state => {
      if (state.error) {
        this.messageService.add({
          severity: 'error',
          summary: 'Erro',
          detail: state.error,
          life: 5000
        });
      }
    });
  }

  private populateForm(professor: Professor): void {
    this.professorForm.patchValue({
      nome: professor.nome,
      cpf: professor.cpf,
      registro: professor.registro,
      email: professor.email,
      telefone: professor.telefone,
      dataNascimento: new Date(professor.dataNascimento),
      dataContratacao: new Date(professor.dataContratacao),
      observacoes: professor.observacoes
    });

    // Populate títulos
    const titulosArray = this.titulosArray;
    titulosArray.clear();
    
    professor.titulos.forEach(titulo => {
      const tituloForm = this.createTituloForm();
      tituloForm.patchValue({
        tipo: this.getTipoTituloValue(titulo.tipo),
        curso: titulo.curso,
        instituicao: titulo.instituicao,
        anoFormatura: titulo.anoFormatura,
        descricao: titulo.descricao
      });
      titulosArray.push(tituloForm);
    });
  }

  private getTipoTituloValue(tipoDescricao: string): number {
    const tipo = this.tiposTitulo.find(t => t.descricao === tipoDescricao);
    return tipo ? tipo.valor : 1;
  }

  get titulosArray(): FormArray {
    return this.professorForm.get('titulos') as FormArray;
  }

  addTitulo(): void {
    this.titulosArray.push(this.createTituloForm());
  }

  removeTitulo(index: number): void {
    this.titulosArray.removeAt(index);
  }

  private createTituloForm(): FormGroup {
    return this.fb.group({
      tipo: ['', [Validators.required]],
      curso: ['', [Validators.required]],
      instituicao: ['', [Validators.required]],
      anoFormatura: ['', [Validators.required, Validators.min(1950), Validators.max(this.currentYear)]],
      descricao: ['']
    });
  }

  onSubmit(): void {
    if (this.professorForm.valid) {
      const formValue = this.professorForm.value;
      
      const titulos: CriarTituloRequest[] = formValue.titulos.map((titulo: any) => ({
        tipo: titulo.tipo,
        curso: titulo.curso,
        instituicao: titulo.instituicao,
        anoFormatura: titulo.anoFormatura,
        descricao: titulo.descricao || ''
      }));

      if (this.isEditMode && this.professorId) {
        const request: AtualizarProfessorRequest = {
          id: this.professorId,
          nome: formValue.nome,
          cpf: formValue.cpf.replace(/\D/g, ''),
          registro: formValue.registro,
          email: formValue.email || undefined,
          telefone: formValue.telefone || undefined,
          dataNascimento: formValue.dataNascimento,
          dataContratacao: formValue.dataContratacao,
          observacoes: formValue.observacoes || undefined,
          titulos
        };

        this.store.dispatch(ProfessoresActions.updateProfessor({ request }));
      } else {
        const request: CriarProfessorRequest = {
          nome: formValue.nome,
          cpf: formValue.cpf.replace(/\D/g, ''),
          registro: formValue.registro,
          email: formValue.email || undefined,
          telefone: formValue.telefone || undefined,
          dataNascimento: formValue.dataNascimento,
          dataContratacao: formValue.dataContratacao,
          escolaId: 'escola-padrao', // TODO: Get from user context
          observacoes: formValue.observacoes || undefined,
          titulos
        };

        this.store.dispatch(ProfessoresActions.createProfessor({ request }));
      }
    } else {
      this.markFormGroupTouched(this.professorForm);
      this.messageService.add({
        severity: 'warn',
        summary: 'Formulário Inválido',
        detail: 'Por favor, corrija os erros no formulário',
        life: 3000
      });
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.professorForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      } else if (control instanceof FormArray) {
        control.controls.forEach(arrayControl => {
          if (arrayControl instanceof FormGroup) {
            this.markFormGroupTouched(arrayControl);
          }
        });
      }
    });
  }

  private cpfValidator(control: any): { [key: string]: any } | null {
    if (!control.value) return null;
    
    const cpf = control.value.replace(/\D/g, '');
    const isValid = this.professoresService.validarCPF(cpf);
    
    return isValid ? null : { cpfInvalido: true };
  }
}