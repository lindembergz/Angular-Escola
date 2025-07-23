import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil, filter } from 'rxjs/operators';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputMaskModule } from 'primeng/inputmask';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';

import { CheckboxModule } from 'primeng/checkbox';
import { TabViewModule } from 'primeng/tabview';
import { DividerModule } from 'primeng/divider';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';

import { AlunosFacade } from '../../../../store/alunos/alunos.facade';
import { AlunosService } from '../../services/alunos.service';
import { 
  CriarAlunoRequest, 
  AtualizarAlunoRequest,
  TipoGenero,
  TipoResponsavel,
  OpcaoGenero,
  OpcaoDeficiencia
} from '../../models/aluno.model';

interface TipoResponsavelOption {
  label: string;
  value: number;
}

@Component({
  selector: 'app-aluno-form',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    CardModule,
    ButtonModule,
    InputTextModule,
    InputMaskModule,
    DropdownModule,
    CalendarModule,

    CheckboxModule,
    TabViewModule,
    DividerModule,
    MessagesModule,
    MessageModule,
    ProgressSpinnerModule,
    ConfirmDialogModule
  ],
  providers: [ConfirmationService], 
 template: `
    <div class="aluno-form-container">
      <!-- Header -->
      <div class="flex justify-content-between align-items-center mb-4">
        <div>
          <h1 class="text-3xl font-bold text-900 m-0">
            {{ isEditMode ? 'Editar Aluno' : 'Novo Aluno' }}
          </h1>
          <p class="text-600 mt-1 mb-0">
            {{ isEditMode ? 'Atualize as informações do aluno' : 'Cadastre um novo aluno na instituição' }}
          </p>
        </div>
        <p-button 
          label="Voltar" 
          icon="pi pi-arrow-left" 
          styleClass="p-button-outlined"
          routerLink="/alunos">
        </p-button>
      </div>

      <!-- Form -->
      <form [formGroup]="alunoForm" (ngSubmit)="onSubmit()">
        <p-tabView>
          <!-- Dados Pessoais -->
          <p-tabPanel header="Dados Pessoais" leftIcon="pi pi-user">
            <p-card>
              <div class="grid">
                <!-- Nome -->
                <div class="col-12 md:col-8">
                  <label for="nome" class="block text-900 font-medium mb-2">
                    Nome Completo *
                  </label>
                  <input 
                    pInputText 
                    id="nome"
                    formControlName="nome"
                    placeholder="Digite o nome completo"
                    class="w-full"
                    [class.ng-invalid]="isFieldInvalid('nome')"
                    maxlength="200">
                  <small class="p-error" *ngIf="isFieldInvalid('nome')">
                    Nome é obrigatório
                  </small>
                </div>

                <!-- CPF -->
                <div class="col-12 md:col-4">
                  <label for="cpf" class="block text-900 font-medium mb-2">
                    CPF *
                  </label>
                  <p-inputMask 
                    id="cpf"
                    formControlName="cpf"
                    mask="999.999.999-99"
                    placeholder="000.000.000-00"
                    class="w-full"
                    [class.ng-invalid]="isFieldInvalid('cpf')">
                  </p-inputMask>
                  <small class="p-error" *ngIf="isFieldInvalid('cpf')">
                    CPF é obrigatório e deve ser válido
                  </small>
                </div>

                <!-- Data de Nascimento -->
                <div class="col-12 md:col-4">
                  <label for="dataNascimento" class="block text-900 font-medium mb-2">
                    Data de Nascimento *
                  </label>
                  <p-calendar 
                    id="dataNascimento"
                    formControlName="dataNascimento"
                    placeholder="dd/mm/aaaa"
                    dateFormat="dd/mm/yy"
                    [showIcon]="true"
                    [maxDate]="maxDate"
                    class="w-full"
                    [class.ng-invalid]="isFieldInvalid('dataNascimento')">
                  </p-calendar>
                  <small class="p-error" *ngIf="isFieldInvalid('dataNascimento')">
                    Data de nascimento é obrigatória
                  </small>
                </div>

                <!-- Gênero -->
                <div class="col-12 md:col-4">
                  <label for="genero" class="block text-900 font-medium mb-2">
                    Gênero
                  </label>
                  <p-dropdown 
                    id="genero"
                    formControlName="genero"
                    [options]="opcoesGenero"
                    optionLabel="descricao"
                    optionValue="valor"
                    placeholder="Selecione o gênero"
                    class="w-full">
                  </p-dropdown>
                </div>

                <!-- Escola -->
                <div class="col-12 md:col-4">
                  <label for="escolaId" class="block text-900 font-medium mb-2">
                    Escola *
                  </label>
                  <p-dropdown 
                    id="escolaId"
                    formControlName="escolaId"
                    [options]="escolas"
                    optionLabel="nome"
                    optionValue="id"
                    placeholder="Selecione a escola"
                    class="w-full"
                    [class.ng-invalid]="isFieldInvalid('escolaId')">
                  </p-dropdown>
                  <small class="p-error" *ngIf="isFieldInvalid('escolaId')">
                    Escola é obrigatória
                  </small>
                </div>
              </div>
            </p-card>
          </p-tabPanel>

          <!-- Endereço -->
          <p-tabPanel header="Endereço" leftIcon="pi pi-map-marker">
            <p-card>
              <div class="grid">
                <div class="col-12 md:col-3">
                  <label for="cep" class="block text-900 font-medium mb-2">
                    CEP *
                  </label>
                  <p-inputMask 
                    id="cep"
                    formControlName="cep"
                    mask="99999-999"
                    placeholder="00000-000"
                    class="w-full"
                    [class.ng-invalid]="isFieldInvalid('cep')">
                  </p-inputMask>
                  <small class="p-error" *ngIf="isFieldInvalid('cep')">
                    CEP é obrigatório
                  </small>
                </div>

                <div class="col-12 md:col-6">
                  <label for="logradouro" class="block text-900 font-medium mb-2">
                    Logradouro *
                  </label>
                  <input 
                    pInputText 
                    id="logradouro"
                    formControlName="logradouro"
                    placeholder="Rua, Avenida, etc."
                    class="w-full"
                    [class.ng-invalid]="isFieldInvalid('logradouro')">
                  <small class="p-error" *ngIf="isFieldInvalid('logradouro')">
                    Logradouro é obrigatório
                  </small>
                </div>

                <div class="col-12 md:col-3">
                  <label for="numero" class="block text-900 font-medium mb-2">
                    Número *
                  </label>
                  <input 
                    pInputText 
                    id="numero"
                    formControlName="numero"
                    placeholder="123"
                    class="w-full"
                    [class.ng-invalid]="isFieldInvalid('numero')">
                  <small class="p-error" *ngIf="isFieldInvalid('numero')">
                    Número é obrigatório
                  </small>
                </div>

                <div class="col-12 md:col-4">
                  <label for="complemento" class="block text-900 font-medium mb-2">
                    Complemento
                  </label>
                  <input 
                    pInputText 
                    id="complemento"
                    formControlName="complemento"
                    placeholder="Apto, Casa, etc."
                    class="w-full">
                </div>

                <div class="col-12 md:col-4">
                  <label for="bairro" class="block text-900 font-medium mb-2">
                    Bairro *
                  </label>
                  <input 
                    pInputText 
                    id="bairro"
                    formControlName="bairro"
                    placeholder="Nome do bairro"
                    class="w-full"
                    [class.ng-invalid]="isFieldInvalid('bairro')">
                  <small class="p-error" *ngIf="isFieldInvalid('bairro')">
                    Bairro é obrigatório
                  </small>
                </div>

                <div class="col-12 md:col-4">
                  <label for="cidade" class="block text-900 font-medium mb-2">
                    Cidade *
                  </label>
                  <input 
                    pInputText 
                    id="cidade"
                    formControlName="cidade"
                    placeholder="Nome da cidade"
                    class="w-full"
                    [class.ng-invalid]="isFieldInvalid('cidade')">
                  <small class="p-error" *ngIf="isFieldInvalid('cidade')">
                    Cidade é obrigatória
                  </small>
                </div>

                <div class="col-12 md:col-4">
                  <label for="estado" class="block text-900 font-medium mb-2">
                    Estado *
                  </label>
                  <p-dropdown 
                    id="estado"
                    formControlName="estado"
                    [options]="estados"
                    optionLabel="nome"
                    optionValue="sigla"
                    placeholder="Selecione o estado"
                    class="w-full"
                    [class.ng-invalid]="isFieldInvalid('estado')">
                  </p-dropdown>
                  <small class="p-error" *ngIf="isFieldInvalid('estado')">
                    Estado é obrigatório
                  </small>
                </div>
              </div>
            </p-card>
          </p-tabPanel>

          <!-- Responsáveis -->
          <p-tabPanel header="Responsáveis" leftIcon="pi pi-users">
            <p-card>
              <div class="flex justify-content-between align-items-center mb-4">
                <h3 class="text-xl font-semibold text-900 m-0">Responsáveis pelo Aluno</h3>
                <p-button 
                  label="Adicionar Responsável" 
                  icon="pi pi-plus" 
                  styleClass="p-button-sm"
                  type="button"
                  (onClick)="addResponsavel()">
                </p-button>
              </div>

              <div formArrayName="responsaveis">
                <div *ngFor="let responsavel of responsaveisFormArray.controls; let i = index" 
                     [formGroupName]="i" 
                     class="mb-4">
                  
                  <p-card styleClass="responsavel-card">
                    <ng-template pTemplate="header">
                      <div class="flex justify-content-between align-items-center p-3">
                        <span class="font-semibold">Responsável {{ i + 1 }}</span>
                        <p-button 
                          *ngIf="responsaveisFormArray.length > 1"
                          icon="pi pi-trash" 
                          styleClass="p-button-rounded p-button-text p-button-danger p-button-sm"
                          type="button"
                          pTooltip="Remover responsável"
                          (onClick)="removeResponsavel(i)">
                        </p-button>
                      </div>
                    </ng-template>

                    <div class="grid">
                      <!-- Nome do Responsável -->
                      <div class="col-12 md:col-6">
                        <label [for]="'resp-nome-' + i" class="block text-900 font-medium mb-2">
                          Nome Completo *
                        </label>
                        <input 
                          pInputText 
                          [id]="'resp-nome-' + i"
                          formControlName="nome"
                          placeholder="Nome completo do responsável"
                          class="w-full"
                          [class.ng-invalid]="responsavel.get('nome')?.invalid && responsavel.get('nome')?.touched">
                        <small class="p-error" *ngIf="responsavel.get('nome')?.invalid && responsavel.get('nome')?.touched">
                          Nome é obrigatório
                        </small>
                      </div>

                      <!-- CPF do Responsável -->
                      <div class="col-12 md:col-3">
                        <label [for]="'resp-cpf-' + i" class="block text-900 font-medium mb-2">
                          CPF *
                        </label>
                        <p-inputMask 
                          [id]="'resp-cpf-' + i"
                          formControlName="cpf"
                          mask="999.999.999-99"
                          placeholder="000.000.000-00"
                          class="w-full"
                          [class.ng-invalid]="responsavel.get('cpf')?.invalid && responsavel.get('cpf')?.touched">
                        </p-inputMask>
                        <small class="p-error" *ngIf="responsavel.get('cpf')?.invalid && responsavel.get('cpf')?.touched">
                          CPF é obrigatório e deve ser válido
                        </small>
                      </div>

                      <!-- Tipo de Responsável -->
                      <div class="col-12 md:col-3">
                        <label [for]="'resp-tipo-' + i" class="block text-900 font-medium mb-2">
                          Tipo *
                        </label>
                        <p-dropdown 
                          [id]="'resp-tipo-' + i"
                          formControlName="tipo"
                          [options]="tiposResponsavel"
                          optionLabel="label"
                          optionValue="value"
                          placeholder="Selecione o tipo"
                          class="w-full"
                          [class.ng-invalid]="responsavel.get('tipo')?.invalid && responsavel.get('tipo')?.touched">
                        </p-dropdown>
                        <small class="p-error" *ngIf="responsavel.get('tipo')?.invalid && responsavel.get('tipo')?.touched">
                          Tipo é obrigatório
                        </small>
                      </div>

                      <!-- Telefone -->
                      <div class="col-12 md:col-4">
                        <label [for]="'resp-telefone-' + i" class="block text-900 font-medium mb-2">
                          Telefone *
                        </label>
                        <p-inputMask 
                          [id]="'resp-telefone-' + i"
                          formControlName="telefone"
                          mask="(99) 99999-9999"
                          placeholder="(00) 00000-0000"
                          class="w-full"
                          [class.ng-invalid]="responsavel.get('telefone')?.invalid && responsavel.get('telefone')?.touched">
                        </p-inputMask>
                        <small class="p-error" *ngIf="responsavel.get('telefone')?.invalid && responsavel.get('telefone')?.touched">
                          Telefone é obrigatório
                        </small>
                      </div>

                      <!-- Email -->
                      <div class="col-12 md:col-4">
                        <label [for]="'resp-email-' + i" class="block text-900 font-medium mb-2">
                          Email
                        </label>
                        <input 
                          pInputText 
                          [id]="'resp-email-' + i"
                          formControlName="email"
                          placeholder="email@exemplo.com"
                          type="email"
                          class="w-full"
                          [class.ng-invalid]="responsavel.get('email')?.invalid && responsavel.get('email')?.touched">
                        <small class="p-error" *ngIf="responsavel.get('email')?.invalid && responsavel.get('email')?.touched">
                          Email deve ter formato válido
                        </small>
                      </div>

                      <!-- Profissão -->
                      <div class="col-12 md:col-4">
                        <label [for]="'resp-profissao-' + i" class="block text-900 font-medium mb-2">
                          Profissão
                        </label>
                        <input 
                          pInputText 
                          [id]="'resp-profissao-' + i"
                          formControlName="profissao"
                          placeholder="Profissão do responsável"
                          class="w-full">
                      </div>

                      <!-- Checkboxes de Responsabilidades -->
                      <div class="col-12">
                        <p-divider align="left">
                          <span class="text-sm text-600">Responsabilidades</span>
                        </p-divider>
                        <div class="flex flex-wrap gap-4">
                          <div class="flex align-items-center">
                            <p-checkbox 
                              [id]="'resp-financeiro-' + i"
                              formControlName="responsavelFinanceiro"
                              [binary]="true">
                            </p-checkbox>
                            <label [for]="'resp-financeiro-' + i" class="ml-2">Responsável Financeiro</label>
                          </div>
                          <div class="flex align-items-center">
                            <p-checkbox 
                              [id]="'resp-academico-' + i"
                              formControlName="responsavelAcademico"
                              [binary]="true">
                            </p-checkbox>
                            <label [for]="'resp-academico-' + i" class="ml-2">Responsável Acadêmico</label>
                          </div>
                          <div class="flex align-items-center">
                            <p-checkbox 
                              [id]="'resp-buscar-' + i"
                              formControlName="autorizadoBuscar"
                              [binary]="true">
                            </p-checkbox>
                            <label [for]="'resp-buscar-' + i" class="ml-2">Autorizado a Buscar</label>
                          </div>
                        </div>
                      </div>
                    </div>
                  </p-card>
                </div>
              </div>

              <!-- Mensagem se não há responsáveis -->
              <div *ngIf="responsaveisFormArray.length === 0" class="text-center p-4">
                <i class="pi pi-users text-4xl text-400 mb-3"></i>
                <p class="text-600 mb-3">Nenhum responsável cadastrado</p>
                <p-button 
                  label="Adicionar Primeiro Responsável" 
                  icon="pi pi-plus" 
                  type="button"
                  (onClick)="addResponsavel()">
                </p-button>
              </div>
            </p-card>
          </p-tabPanel>
        </p-tabView>

        <!-- Actions -->
        <div class="flex justify-content-end gap-2 mt-4">
          <p-button 
            label="Cancelar" 
            icon="pi pi-times" 
            styleClass="p-button-outlined"
            type="button"
            (onClick)="onCancel()">
          </p-button>
          <p-button 
            [label]="isEditMode ? 'Atualizar' : 'Cadastrar'" 
            icon="pi pi-check" 
            type="submit"
            [loading]="loading$ | async"
            [disabled]="alunoForm.invalid">
          </p-button>
        </div>
      </form>
    </div>

    <!-- Confirm Dialog -->
    <p-confirmDialog></p-confirmDialog>
  `, 
 styles: [`
    .aluno-form-container {
      padding: 1rem;
    }

    :host ::ng-deep .p-tabview .p-tabview-panels {
      padding: 0;
    }

    :host ::ng-deep .p-card .p-card-body {
      padding: 1.5rem;
    }
  `]
})
export class AlunoFormComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  alunoForm!: FormGroup;
  isEditMode = false;
  alunoId: string | null = null;
  maxDate = new Date();

  // Observables
  loading$;
  selectedAluno$;
  opcoes$;

  // Options
  opcoesGenero: OpcaoGenero[] = [];
  opcoesTiposDeficiencia: OpcaoDeficiencia[] = [];
  escolas: any[] = [
    { id: '1', nome: 'Escola Municipal João Silva' },
    { id: '2', nome: 'Escola Estadual Maria Santos' },
    { id: '3', nome: 'Colégio Particular ABC' }
  ];
  estados: any[] = [
    { nome: 'São Paulo', sigla: 'SP' },
    { nome: 'Rio de Janeiro', sigla: 'RJ' },
    { nome: 'Minas Gerais', sigla: 'MG' },
    { nome: 'Bahia', sigla: 'BA' },
    { nome: 'Paraná', sigla: 'PR' },
    { nome: 'Rio Grande do Sul', sigla: 'RS' },
    { nome: 'Pernambuco', sigla: 'PE' },
    { nome: 'Ceará', sigla: 'CE' },
    { nome: 'Pará', sigla: 'PA' },
    { nome: 'Santa Catarina', sigla: 'SC' }
  ];

  tiposResponsavel: TipoResponsavelOption[] = [
    { label: 'Pai', value: TipoResponsavel.Pai },
    { label: 'Mãe', value: TipoResponsavel.Mae },
    { label: 'Responsável', value: TipoResponsavel.Responsavel },
    { label: 'Tutor', value: TipoResponsavel.Tutor }
  ];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private alunosFacade: AlunosFacade,
    private alunosService: AlunosService,
    private confirmationService: ConfirmationService
  ) {
    this.loading$ = this.alunosFacade.loading$;
    this.selectedAluno$ = this.alunosFacade.selectedAluno$;
    this.opcoes$ = this.alunosFacade.opcoes$;
    this.initializeForm();
  }

  ngOnInit(): void {
    this.loadOptions();
    this.checkEditMode();
    this.setupFormSubscriptions();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeForm(): void {
    this.alunoForm = this.fb.group({
      nome: ['', [Validators.required, Validators.maxLength(200)]],
      cpf: ['', [Validators.required, this.cpfValidator.bind(this)]],
      dataNascimento: ['', Validators.required],
      genero: [TipoGenero.NaoInformado],
      escolaId: ['', Validators.required],
      tipoDeficiencia: [null],
      descricaoDeficiencia: [''],
      telefone: [''],
      email: ['', Validators.email],
      observacoes: [''],
      cep: ['', Validators.required],
      logradouro: ['', Validators.required],
      numero: ['', Validators.required],
      complemento: [''],
      bairro: ['', Validators.required],
      cidade: ['', Validators.required],
      estado: ['', Validators.required],
      responsaveis: this.fb.array([])
    });

    this.addResponsavel();
  }

  private loadOptions(): void {
    this.alunosFacade.loadOpcoes();
    
    this.opcoes$.pipe(
      filter(opcoes => !!opcoes),
      takeUntil(this.destroy$)
    ).subscribe(opcoes => {
      this.opcoesGenero = opcoes!.generos;
      this.opcoesTiposDeficiencia = opcoes!.tiposDeficiencia;
    });
  }

  private checkEditMode(): void {
    this.alunoId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.alunoId && this.route.snapshot.url.some(segment => segment.path === 'editar');
    
    if (this.isEditMode && this.alunoId) {
      this.alunosFacade.loadAluno(this.alunoId);
    }
  }

  private setupFormSubscriptions(): void {
    if (this.isEditMode) {
      this.selectedAluno$.pipe(
        filter(aluno => !!aluno),
        takeUntil(this.destroy$)
      ).subscribe(aluno => {
        this.populateForm(aluno!);
      });
    }
  }

  private populateForm(aluno: any): void {
    this.alunoForm.patchValue({
      nome: aluno.nome,
      cpf: aluno.cpf,
      dataNascimento: new Date(aluno.dataNascimento),
      genero: aluno.genero?.tipo || TipoGenero.NaoInformado,
      escolaId: aluno.escolaId,
      tipoDeficiencia: aluno.deficiencia?.tipo,
      descricaoDeficiencia: aluno.deficiencia?.descricao,
      telefone: aluno.telefone,
      email: aluno.email,
      observacoes: aluno.observacoes,
      cep: aluno.endereco?.cep,
      logradouro: aluno.endereco?.logradouro,
      numero: aluno.endereco?.numero,
      complemento: aluno.endereco?.complemento,
      bairro: aluno.endereco?.bairro,
      cidade: aluno.endereco?.cidade,
      estado: aluno.endereco?.estado
    });

    this.responsaveisFormArray.clear();
    aluno.responsaveis?.forEach((resp: any) => {
      this.addResponsavel(resp);
    });
  }

  get responsaveisFormArray(): FormArray {
    return this.alunoForm.get('responsaveis') as FormArray;
  }

  addResponsavel(data?: any): void {
    const responsavelForm = this.fb.group({
      nome: [data?.nome || '', Validators.required],
      cpf: [data?.cpf || '', [Validators.required, this.cpfValidator.bind(this)]],
      telefone: [data?.telefone || '', Validators.required],
      email: [data?.email || '', Validators.email],
      tipo: [data?.tipo || TipoResponsavel.Responsavel, Validators.required],
      profissao: [data?.profissao || ''],
      localTrabalho: [data?.localTrabalho || ''],
      telefoneTrabalho: [data?.telefoneTrabalho || ''],
      responsavelFinanceiro: [data?.responsavelFinanceiro || false],
      responsavelAcademico: [data?.responsavelAcademico || false],
      autorizadoBuscar: [data?.autorizadoBuscar || false],
      observacoes: [data?.observacoes || '']
    });

    this.responsaveisFormArray.push(responsavelForm);
  }

  removeResponsavel(index: number): void {
    if (this.responsaveisFormArray.length > 1) {
      this.responsaveisFormArray.removeAt(index);
    }
  }

  onSubmit(): void {
    if (this.alunoForm.valid) {
      const formValue = this.alunoForm.value;
      
      if (this.isEditMode) {
        const request: AtualizarAlunoRequest = {
          id: this.alunoId!,
          nome: formValue.nome,
          logradouro: formValue.logradouro,
          numero: formValue.numero,
          complemento: formValue.complemento,
          bairro: formValue.bairro,
          cidade: formValue.cidade,
          estado: formValue.estado,
          cep: formValue.cep,
          genero: formValue.genero,
          tipoDeficiencia: formValue.tipoDeficiencia,
          descricaoDeficiencia: formValue.descricaoDeficiencia,
          telefone: formValue.telefone,
          email: formValue.email,
          observacoes: formValue.observacoes,
          responsaveis: formValue.responsaveis
        };
        
        this.alunosFacade.updateAluno(request);
      } else {
        const request: CriarAlunoRequest = {
          nome: formValue.nome,
          cpf: formValue.cpf,
          dataNascimento: formValue.dataNascimento,
          logradouro: formValue.logradouro,
          numero: formValue.numero,
          complemento: formValue.complemento,
          bairro: formValue.bairro,
          cidade: formValue.cidade,
          estado: formValue.estado,
          cep: formValue.cep,
          genero: formValue.genero,
          tipoDeficiencia: formValue.tipoDeficiencia,
          descricaoDeficiencia: formValue.descricaoDeficiencia,
          telefone: formValue.telefone,
          email: formValue.email,
          observacoes: formValue.observacoes,
          escolaId: formValue.escolaId,
          responsaveis: formValue.responsaveis
        };
        
        this.alunosFacade.createAluno(request);
      }
    } else {
      this.markFormGroupTouched(this.alunoForm);
    }
  }

  onCancel(): void {
    this.confirmationService.confirm({
      message: 'Tem certeza que deseja cancelar? Todas as alterações serão perdidas.',
      header: 'Confirmar Cancelamento',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim, cancelar',
      rejectLabel: 'Continuar editando',
      accept: () => {
        this.router.navigate(['/alunos']);
      }
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.alunoForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  private cpfValidator(control: any): { [key: string]: any } | null {
    if (!control.value) {
      return null;
    }
    
    const cpf = control.value.replace(/\D/g, '');
    const isValid = this.alunosService.validarCPF(cpf);
    
    return isValid ? null : { cpfInvalid: true };
  }
}