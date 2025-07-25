import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { DividerModule } from 'primeng/divider';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { SkeletonModule } from 'primeng/skeleton';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';

import { MessageService, ConfirmationService } from 'primeng/api';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';

// Store
import { AppState } from '../../../../store/app.state';
import * as ProfessoresActions from '../../../../store/professores/professores.actions';
import * as ProfessoresSelectors from '../../../../store/professores/professores.selectors';

// Models
import { Professor, AtribuirDisciplinaRequest, DisciplinaInfo } from '../../models/professor.model';

// Services
import { ProfessoresService } from '../../services/professores.service';

@Component({
  selector: 'app-professor-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    CardModule,
    ButtonModule,
    TagModule,
    DividerModule,
    TableModule,
    TooltipModule,
    SkeletonModule,
    ToastModule,
    ConfirmDialogModule,
    DialogModule,
    InputTextModule,
    InputNumberModule,
    DropdownModule
  ],
  providers: [MessageService, ConfirmationService],
  template: `
    <div class="professor-detail-container">
      <p-toast></p-toast>
      <p-confirmDialog></p-confirmDialog>

      <!-- Header -->
      <div class="page-header mb-4">
        <div class="flex justify-content-between align-items-center">
          <div>
            <h1 class="text-3xl font-bold text-900">Detalhes do Professor</h1>
            <p class="text-600 mt-2">Informações completas do professor</p>
          </div>
          
          <div class="flex gap-2">
            <p-button 
              label="Voltar" 
              icon="pi pi-arrow-left" 
              routerLink="/professores"
              class="p-button-outlined">
            </p-button>
            
            <p-button 
              *ngIf="professor"
              label="Editar" 
              icon="pi pi-pencil" 
              [routerLink]="['/professores', professor.id, 'editar']"
              class="p-button-warning">
            </p-button>
          </div>
        </div>
      </div>

      <!-- Loading -->
      <div *ngIf="loading$ | async" class="flex justify-content-center p-4">
        <div class="grid w-full">
          <div class="col-12 md:col-8">
            <p-card>
              <p-skeleton height="2rem" class="mb-3"></p-skeleton>
              <p-skeleton height="1rem" class="mb-2"></p-skeleton>
              <p-skeleton height="1rem" class="mb-2"></p-skeleton>
              <p-skeleton height="1rem"></p-skeleton>
            </p-card>
          </div>
          <div class="col-12 md:col-4">
            <p-card>
              <p-skeleton height="2rem" class="mb-3"></p-skeleton>
              <p-skeleton height="1rem" class="mb-2"></p-skeleton>
              <p-skeleton height="1rem"></p-skeleton>
            </p-card>
          </div>
        </div>
      </div>

      <!-- Content -->
      <div *ngIf="professor && !(loading$ | async)" class="grid">
        
        <!-- Main Info -->
        <div class="col-12 md:col-8">
          
          <!-- Dados Pessoais -->
          <p-card header="Dados Pessoais" class="mb-4">
            <div class="grid">
              <div class="col-12 md:col-6">
                <div class="field">
                  <label class="font-medium text-900">Nome Completo</label>
                  <p class="text-700 mt-1">{{ professor.nome }}</p>
                </div>
              </div>

              <div class="col-12 md:col-3">
                <div class="field">
                  <label class="font-medium text-900">CPF</label>
                  <p class="text-700 mt-1">{{ formatCPF(professor.cpf) }}</p>
                </div>
              </div>

              <div class="col-12 md:col-3">
                <div class="field">
                  <label class="font-medium text-900">Registro</label>
                  <p class="text-700 mt-1">{{ professor.registro }}</p>
                </div>
              </div>

              <div class="col-12 md:col-6">
                <div class="field">
                  <label class="font-medium text-900">E-mail</label>
                  <p class="text-700 mt-1">
                    {{ professor.email || 'Não informado' }}
                  </p>
                </div>
              </div>

              <div class="col-12 md:col-6">
                <div class="field">
                  <label class="font-medium text-900">Telefone</label>
                  <p class="text-700 mt-1">
                    {{ professor.telefone || 'Não informado' }}
                  </p>
                </div>
              </div>

              <div class="col-12 md:col-4">
                <div class="field">
                  <label class="font-medium text-900">Data de Nascimento</label>
                  <p class="text-700 mt-1">{{ formatDate(professor.dataNascimento) }}</p>
                </div>
              </div>

              <div class="col-12 md:col-4">
                <div class="field">
                  <label class="font-medium text-900">Idade</label>
                  <p class="text-700 mt-1">{{ professor.idade }} anos</p>
                </div>
              </div>

              <div class="col-12 md:col-4">
                <div class="field">
                  <label class="font-medium text-900">Data de Contratação</label>
                  <p class="text-700 mt-1">{{ formatDate(professor.dataContratacao) }}</p>
                </div>
              </div>

              <div class="col-12" *ngIf="professor.observacoes">
                <div class="field">
                  <label class="font-medium text-900">Observações</label>
                  <p class="text-700 mt-1">{{ professor.observacoes }}</p>
                </div>
              </div>
            </div>
          </p-card>

          <!-- Títulos Acadêmicos -->
          <p-card class="mb-4">
            <ng-template pTemplate="header">
              <span class="font-bold">Títulos Acadêmicos</span>
            </ng-template>

            <div *ngIf="professor.titulos.length > 0; else noTitulos">
              <div 
                *ngFor="let titulo of professor.titulos; let i = index" 
                class="border-1 border-200 border-round p-3 mb-3">
                <div class="grid">
                  <div class="col-12 md:col-3">
                    <div class="field">
                      <label class="font-medium text-900">Tipo</label>
                      <p class="text-700 mt-1">{{ titulo.tipo }}</p>
                    </div>
                  </div>

                  <div class="col-12 md:col-5">
                    <div class="field">
                      <label class="font-medium text-900">Curso</label>
                      <p class="text-700 mt-1">{{ titulo.curso }}</p>
                    </div>
                  </div>

                  <div class="col-12 md:col-4">
                    <div class="field">
                      <label class="font-medium text-900">Instituição</label>
                      <p class="text-700 mt-1">{{ titulo.instituicao }}</p>
                    </div>
                  </div>

                  <div class="col-12 md:col-3">
                    <div class="field">
                      <label class="font-medium text-900">Ano de Formatura</label>
                      <p class="text-700 mt-1">{{ titulo.anoFormatura }}</p>
                    </div>
                  </div>

                  <div class="col-12 md:col-9" *ngIf="titulo.descricao">
                    <div class="field">
                      <label class="font-medium text-900">Descrição</label>
                      <p class="text-700 mt-1">{{ titulo.descricao }}</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <ng-template #noTitulos>
              <div class="text-center p-4">
                <i class="pi pi-graduation-cap text-4xl text-400 mb-3"></i>
                <p class="text-600">Nenhum título acadêmico cadastrado</p>
              </div>
            </ng-template>
          </p-card>

          <!-- Disciplinas -->
          <p-card>
            <ng-template pTemplate="header">
              <div class="flex justify-content-between align-items-center w-full">
                <span class="font-bold">Disciplinas</span>
                <p-button 
                  label="Atribuir Disciplina" 
                  icon="pi pi-plus" 
                  (onClick)="showAddDisciplineDialog()"
                  class="p-button-sm p-button-outlined">
                </p-button>
              </div>
            </ng-template>

            <p-table 
              [value]="professor.disciplinas" 
              [paginator]="false"
              styleClass="p-datatable-striped">

              <ng-template pTemplate="emptymessage">
                <tr>
                  <td colspan="5">
                    <div class="text-center p-4">
                      <i class="pi pi-book text-4xl text-400 mb-3"></i>
                      <p class="text-600">Nenhuma disciplina atribuída</p>
                      <p class="text-500 text-sm">Clique em "Atribuir Disciplina" para começar</p>
                    </div>
                  </td>
                </tr>
              </ng-template>

              <ng-template pTemplate="header">
                <tr>
                  <th>Disciplina</th>
                  <th>Carga Horária</th>
                  <th>Data de Atribuição</th>
                  <th>Status</th>
                  <th>Ações</th>
                </tr>
              </ng-template>

              <ng-template pTemplate="body" let-disciplina>
                <tr>
                  <td>{{ disciplina.disciplina?.nome || 'Carregando...' }}</td>
                  <td>{{ disciplina.cargaHorariaSemanal }}h/semana</td>
                  <td>{{ formatDate(disciplina.dataAtribuicao) }}</td>
                  <td>
                    <p-tag 
                      [value]="disciplina.ativa ? 'Ativa' : 'Inativa'"
                      [severity]="disciplina.ativa ? 'success' : 'danger'">
                    </p-tag>
                  </td>
                  <td>
                    <p-button 
                      *ngIf="disciplina.ativa"
                      icon="pi pi-times" 
                      (onClick)="confirmRemoveDiscipline(disciplina)"
                      pTooltip="Remover disciplina"
                      class="p-button-rounded p-button-text p-button-danger">
                    </p-button>
                  </td>
                </tr>
              </ng-template>
            </p-table>
          </p-card>
        </div>

        <!-- Sidebar -->
        <div class="col-12 md:col-4">
          
          <!-- Status Card -->
          <p-card header="Status" class="mb-4">
            <div class="flex flex-column gap-3">
              <div class="flex justify-content-between align-items-center">
                <span class="font-medium">Status:</span>
                <p-tag 
                  [value]="professor.ativo ? 'Ativo' : 'Inativo'"
                  [severity]="professor.ativo ? 'success' : 'danger'">
                </p-tag>
              </div>

              <div class="flex justify-content-between align-items-center">
                <span class="font-medium">Tempo de Serviço:</span>
                <span class="text-700">{{ professor.tempoServico }} anos</span>
              </div>

              <div class="flex justify-content-between align-items-center">
                <span class="font-medium">Carga Horária Total:</span>
                <span class="text-700">{{ professor.cargaHorariaTotal }}h/semana</span>
              </div>

              <div class="flex justify-content-between align-items-center">
                <span class="font-medium">Disciplinas:</span>
                <span class="text-700">{{ professor.disciplinas.length }}</span>
              </div>

              <p-divider></p-divider>

              <div class="flex justify-content-between align-items-center">
                <span class="font-medium">Cadastrado em:</span>
                <span class="text-700">{{ formatDate(professor.dataCadastro) }}</span>
              </div>
            </div>
          </p-card>

          <!-- Actions Card -->
          <p-card header="Ações" *ngIf="professor.ativo">
            <div class="flex flex-column gap-2">
              <p-button 
                label="Editar Professor" 
                icon="pi pi-pencil" 
                [routerLink]="['/professores', professor.id, 'editar']"
                class="w-full p-button-warning">
              </p-button>

              <p-button 
                label="Desativar Professor" 
                icon="pi pi-times" 
                (onClick)="confirmDeactivate()"
                class="w-full p-button-danger p-button-outlined">
              </p-button>
            </div>
          </p-card>
        </div>
      </div>

      <!-- Add Discipline Dialog -->
      <p-dialog 
        header="Atribuir Disciplina" 
        [(visible)]="showDisciplineDialog" 
        [modal]="true" 
        [style]="{width: '500px'}"
        [closable]="true">
        
        <form [formGroup]="disciplineForm" (ngSubmit)="onAddDiscipline()">
          <div class="grid">
            <div class="col-12">
              <label for="disciplinaId" class="block text-900 font-medium mb-2">
                Disciplina *
              </label>
              <p-dropdown
                id="disciplinaId"
                formControlName="disciplinaId"
                [options]="disciplinasDisponiveis"
                optionLabel="nome"
                optionValue="id"
                placeholder="Selecione uma disciplina"
                class="w-full">
              </p-dropdown>
            </div>

            <div class="col-12">
              <label for="cargaHorariaSemanal" class="block text-900 font-medium mb-2">
                Carga Horária Semanal *
              </label>
              <p-inputNumber
                id="cargaHorariaSemanal"
                formControlName="cargaHorariaSemanal"
                [min]="1"
                [max]="40"
                suffix=" horas"
                placeholder="Ex: 4"
                class="w-full">
              </p-inputNumber>
            </div>

            <div class="col-12">
              <label for="observacoes" class="block text-900 font-medium mb-2">
                Observações
              </label>
              <textarea 
                id="observacoes"
                formControlName="observacoes"
                rows="3"
                placeholder="Observações sobre a atribuição da disciplina (opcional)"
                class="w-full p-inputtext">
              </textarea>
            </div>
          </div>

          <div class="flex justify-content-end gap-2 mt-4">
            <p-button 
              label="Cancelar" 
              icon="pi pi-times" 
              (onClick)="hideDisciplineDialog()"
              class="p-button-outlined">
            </p-button>
            
            <p-button 
              label="Atribuir" 
              icon="pi pi-check" 
              type="submit"
              [disabled]="disciplineForm.invalid"
              class="p-button-success">
            </p-button>
          </div>
        </form>
      </p-dialog>
    </div>
  `,
  styles: [`
    .professor-detail-container {
      padding: 1rem;
      max-width: 1200px;
      margin: 0 auto;
    }

    .field {
      margin-bottom: 1rem;
    }

    .field label {
      display: block;
      margin-bottom: 0.25rem;
    }

    :host ::ng-deep {
      .p-card .p-card-body {
        padding: 1.5rem;
      }

      .p-datatable .p-datatable-tbody > tr > td {
        padding: 0.75rem;
      }
    }
  `]
})
export class ProfessorDetailComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  professor: Professor | null = null;
  professorId: string | null = null;
  
  // Dialog
  showDisciplineDialog = false;
  disciplineForm!: FormGroup;
  disciplinasDisponiveis: DisciplinaInfo[] = [];

  // Observables
  loading$: Observable<boolean>;
  error$: Observable<string | null>;

  constructor(
    private route: ActivatedRoute,
    private store: Store<AppState>,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private professoresService: ProfessoresService,
    private fb: FormBuilder
  ) {
    this.loading$ = this.store.select(ProfessoresSelectors.selectLoading);
    this.error$ = this.store.select(ProfessoresSelectors.selectError);
  }

  ngOnInit(): void {
    this.initializeDisciplineForm();
    this.loadProfessor();
    this.setupSubscriptions();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.store.dispatch(ProfessoresActions.clearProfessorDetail());
  }

  private initializeDisciplineForm(): void {
    this.disciplineForm = this.fb.group({
      disciplinaId: ['', [Validators.required]],
      cargaHorariaSemanal: [4, [Validators.required, Validators.min(1), Validators.max(40)]],
      observacoes: ['']
    });
  }

  private loadProfessor(): void {
    this.professorId = this.route.snapshot.paramMap.get('id');
    if (this.professorId) {
      this.store.dispatch(ProfessoresActions.loadProfessorDetail({ 
        id: this.professorId 
      }));
    }
  }

  private setupSubscriptions(): void {
    // Subscribe to professor data
    this.store.select(ProfessoresSelectors.selectCurrentProfessor).pipe(
      takeUntil(this.destroy$)
    ).subscribe(professor => {
      this.professor = professor;
    });

    // Subscribe to errors
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

  showAddDisciplineDialog(): void {
    this.loadDisciplinasDisponiveis();
    this.showDisciplineDialog = true;
    this.disciplineForm.reset({
      disciplinaId: '',
      cargaHorariaSemanal: 4,
      observacoes: ''
    });
  }

  hideDisciplineDialog(): void {
    this.showDisciplineDialog = false;
  }

  onAddDiscipline(): void {
    if (this.disciplineForm.valid && this.professor) {
      const formValue = this.disciplineForm.value;
      const disciplinaSelecionada = this.disciplinasDisponiveis.find(d => d.id === formValue.disciplinaId);
      
      const request: AtribuirDisciplinaRequest = {
        disciplinaId: formValue.disciplinaId,
        cargaHorariaSemanal: formValue.cargaHorariaSemanal,
        observacoes: formValue.observacoes
      };

      this.professoresService.atribuirDisciplina(this.professor.id, request).subscribe({
        next: () => {
          this.hideDisciplineDialog();
          
          this.messageService.add({
            severity: 'success',
            summary: 'Disciplina Atribuída',
            detail: `${disciplinaSelecionada?.nome || 'Disciplina'} foi atribuída com sucesso`,
            life: 3000
          });

          // Reload professor data
          if (this.professorId) {
            this.store.dispatch(ProfessoresActions.loadProfessorDetail({ 
              id: this.professorId 
            }));
          }
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Erro',
            detail: 'Erro ao atribuir disciplina: ' + error.message,
            life: 5000
          });
        }
      });
    }
  }

  confirmRemoveDiscipline(disciplina: any): void {
    const nomeDisciplina = disciplina.disciplina?.nome || 'Disciplina';
    
    this.confirmationService.confirm({
      message: `Tem certeza que deseja remover a disciplina ${nomeDisciplina}?`,
      header: 'Confirmar Remoção',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim, remover',
      rejectLabel: 'Cancelar',
      accept: () => {
        if (this.professor) {
          this.professoresService.removerDisciplina(this.professor.id, disciplina.disciplinaId).subscribe({
            next: () => {
              this.messageService.add({
                severity: 'success',
                summary: 'Disciplina Removida',
                detail: `${nomeDisciplina} foi removida com sucesso`,
                life: 3000
              });

              // Reload professor data
              if (this.professorId) {
                this.store.dispatch(ProfessoresActions.loadProfessorDetail({ 
                  id: this.professorId 
                }));
              }
            },
            error: (error) => {
              this.messageService.add({
                severity: 'error',
                summary: 'Erro',
                detail: 'Erro ao remover disciplina: ' + error.message,
                life: 5000
              });
            }
          });
        }
      }
    });
  }

  confirmDeactivate(): void {
    if (!this.professor) return;

    this.confirmationService.confirm({
      message: `Tem certeza que deseja desativar o professor ${this.professor.nome}?`,
      header: 'Confirmar Desativação',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim, desativar',
      rejectLabel: 'Cancelar',
      accept: () => {
        if (this.professor) {
          this.store.dispatch(ProfessoresActions.deactivateProfessor({ 
            id: this.professor.id 
          }));
          
          this.messageService.add({
            severity: 'success',
            summary: 'Professor Desativado',
            detail: `${this.professor.nome} foi desativado com sucesso`,
            life: 3000
          });
        }
      }
    });
  }

  formatCPF(cpf: string): string {
    return this.professoresService.formatarCPF(cpf);
  }

  formatDate(date: Date | string): string {
    const d = new Date(date);
    return d.toLocaleDateString('pt-BR');
  }

  private loadDisciplinasDisponiveis(): void {
    if (this.professor?.escolaId) {
      this.professoresService.obterDisciplinasDisponiveis(this.professor.escolaId).subscribe({
        next: (disciplinas) => {
          this.disciplinasDisponiveis = disciplinas;
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Erro',
            detail: 'Erro ao carregar disciplinas disponíveis: ' + error.message,
            life: 5000
          });
          this.disciplinasDisponiveis = [];
        }
      });
    }
  }
}