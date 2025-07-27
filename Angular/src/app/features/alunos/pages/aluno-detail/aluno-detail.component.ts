import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil, filter } from 'rxjs/operators';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { DividerModule } from 'primeng/divider';
import { TabViewModule } from 'primeng/tabview';
import { TableModule } from 'primeng/table';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TooltipModule } from 'primeng/tooltip';
import { ChipModule } from 'primeng/chip';
import { AlunosFacade } from '../../../../store/alunos/alunos.facade';
import { Aluno } from '../../models/aluno.model';
import { AlunoIdHelperComponent } from '../../components/aluno-id-helper/aluno-id-helper.component';
import { MatricularAlunoModalComponent } from '../../components/matricular-aluno-modal/matricular-aluno-modal.component';

@Component({
  selector: 'app-aluno-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    CardModule,
    ButtonModule,
    TagModule,
    DividerModule,
    TabViewModule,
    TableModule,
    ProgressSpinnerModule,
    TooltipModule,
    ChipModule,
    AlunoIdHelperComponent,
    MatricularAlunoModalComponent
  ],
  template: `
    <div class="aluno-detail-container">
      <!-- Header -->
      <div class="flex justify-content-between align-items-center mb-4">
        <div>
          <h1 class="text-3xl font-bold text-900 m-0">Detalhes do Aluno</h1>
          <p class="text-600 mt-1 mb-0">Informações completas do aluno</p>
        </div>
        <div class="flex gap-2">
          <p-button 
            label="Matricular" 
            icon="pi pi-id-card" 
            styleClass="p-button-success"
            (onClick)="matricularAluno()"
            *ngIf="aluno && !aluno.matriculaAtiva">
          </p-button>
          <p-button 
            label="Editar" 
            icon="pi pi-pencil" 
            styleClass="p-button-outlined"
            [routerLink]="['/alunos', alunoId, 'editar']"
            *ngIf="aluno">
          </p-button>
          <p-button 
            label="Voltar" 
            icon="pi pi-arrow-left" 
            styleClass="p-button-outlined"
            routerLink="/alunos">
          </p-button>
        </div>
      </div>

      <!-- Loading -->
      <div *ngIf="loading$ | async" class="flex justify-content-center p-4">
        <p-progressSpinner></p-progressSpinner>
      </div>

      <!-- Content -->
      <div *ngIf="aluno && !(loading$ | async)">
        <!-- Quick Info Card -->
        <p-card styleClass="mb-4">
          <div class="flex align-items-center">
            <div class="flex-1">
              <h2 class="text-2xl font-bold text-900 m-0">{{ aluno.nome }}</h2>
              <div class="flex align-items-center gap-2 mt-2">
                <p-tag 
                  [value]="aluno.ativo ? 'Ativo' : 'Inativo'"
                  [severity]="aluno.ativo ? 'success' : 'danger'">
                </p-tag>
                <p-tag 
                  *ngIf="aluno.matriculaAtiva"
                  [value]="'Matriculado - ' + aluno.matriculaAtiva.nomeTurma"
                  severity="info">
                </p-tag>
                <p-tag 
                  *ngIf="aluno.deficiencia.possuiDeficiencia"
                  value="PcD"
                  severity="warning"
                  [pTooltip]="aluno.deficiencia.descricao">
                </p-tag>
              </div>
            </div>
            <div class="text-right">
              <div class="text-2xl font-bold text-primary">{{ aluno.idade }}</div>
              <div class="text-600">anos</div>
            </div>
          </div>
        </p-card>

        <!-- Detailed Information -->
        <p-tabView>
          <!-- Dados Pessoais -->
          <p-tabPanel header="Dados Pessoais" leftIcon="pi pi-user">
            <div class="grid">
              <div class="col-12 md:col-6">
                <p-card>
                  <ng-template pTemplate="header">
                    <div class="p-3">
                      <h3 class="text-lg font-medium text-900 m-0">Informações Básicas</h3>
                    </div>
                  </ng-template>
                  
                  <div class="field-group">
                    <label class="field-label">Nome Completo</label>
                    <div class="field-value">{{ aluno.nome }}</div>
                  </div>
                  
                  <div class="field-group">
                    <label class="field-label">CPF</label>
                    <div class="field-value">{{ aluno.cpf }}</div>
                  </div>
                  
                  <div class="field-group">
                    <label class="field-label">Data de Nascimento</label>
                    <div class="field-value">{{ aluno.dataNascimento | date:'dd/MM/yyyy' }}</div>
                  </div>
                  
                  <div class="field-group">
                    <label class="field-label">Idade</label>
                    <div class="field-value">{{ aluno.idade }} anos</div>
                  </div>
                  
                  <div class="field-group">
                    <label class="field-label">Faixa Etária Escolar</label>
                    <div class="field-value">{{ aluno.faixaEtariaEscolar }}</div>
                  </div>
                  
                  <div class="field-group">
                    <label class="field-label">Gênero</label>
                    <div class="field-value">{{ aluno.genero.descricao }}</div>
                  </div>
                </p-card>
              </div>

              <div class="col-12 md:col-6">
                <p-card>
                  <ng-template pTemplate="header">
                    <div class="p-3">
                      <h3 class="text-lg font-medium text-900 m-0">Contato</h3>
                    </div>
                  </ng-template>
                  
                  <div class="field-group" *ngIf="aluno.telefone">
                    <label class="field-label">Telefone</label>
                    <div class="field-value">{{ aluno.telefone }}</div>
                  </div>
                  
                  <div class="field-group" *ngIf="aluno.email">
                    <label class="field-label">E-mail</label>
                    <div class="field-value">{{ aluno.email }}</div>
                  </div>
                  
                  <div class="field-group">
                    <label class="field-label">Escola</label>
                    <div class="field-value">{{ aluno.nomeEscola }}</div>
                  </div>
                  
                  <div class="field-group">
                    <label class="field-label">Data de Cadastro</label>
                    <div class="field-value">{{ aluno.dataCadastro | date:'dd/MM/yyyy HH:mm' }}</div>
                  </div>
                  
                  <div class="field-group" *ngIf="aluno.observacoes">
                    <label class="field-label">Observações</label>
                    <div class="field-value">{{ aluno.observacoes }}</div>
                  </div>
                </p-card>
              </div>

              <!-- Deficiência -->
              <div class="col-12" *ngIf="aluno.deficiencia.possuiDeficiencia">
                <p-card>
                  <ng-template pTemplate="header">
                    <div class="p-3">
                      <h3 class="text-lg font-medium text-900 m-0">Informações sobre Deficiência</h3>
                    </div>
                  </ng-template>
                  
                  <div class="field-group">
                    <label class="field-label">Tipo de Deficiência</label>
                    <div class="field-value">{{ aluno.deficiencia.tipoDescricao }}</div>
                  </div>
                  
                  <div class="field-group">
                    <label class="field-label">Descrição das Adaptações</label>
                    <div class="field-value">{{ aluno.deficiencia.descricao }}</div>
                  </div>
                </p-card>
              </div>
            </div>
          </p-tabPanel>

          <!-- Endereço -->
          <p-tabPanel header="Endereço" leftIcon="pi pi-map-marker">
            <p-card>
              <div class="grid">
                <div class="col-12 md:col-8">
                  <div class="field-group">
                    <label class="field-label">Endereço Completo</label>
                    <div class="field-value">{{ aluno.endereco.enderecoCompleto }}</div>
                  </div>
                </div>
                <div class="col-12 md:col-4">
                  <div class="field-group">
                    <label class="field-label">CEP</label>
                    <div class="field-value">{{ aluno.endereco.cepFormatado }}</div>
                  </div>
                </div>
                
                <div class="col-12 md:col-6">
                  <div class="field-group">
                    <label class="field-label">Logradouro</label>
                    <div class="field-value">{{ aluno.endereco.logradouro }}</div>
                  </div>
                </div>
                
                <div class="col-12 md:col-3">
                  <div class="field-group">
                    <label class="field-label">Número</label>
                    <div class="field-value">{{ aluno.endereco.numero }}</div>
                  </div>
                </div>
                
                <div class="col-12 md:col-3" *ngIf="aluno.endereco.complemento">
                  <div class="field-group">
                    <label class="field-label">Complemento</label>
                    <div class="field-value">{{ aluno.endereco.complemento }}</div>
                  </div>
                </div>
                
                <div class="col-12 md:col-4">
                  <div class="field-group">
                    <label class="field-label">Bairro</label>
                    <div class="field-value">{{ aluno.endereco.bairro }}</div>
                  </div>
                </div>
                
                <div class="col-12 md:col-4">
                  <div class="field-group">
                    <label class="field-label">Cidade</label>
                    <div class="field-value">{{ aluno.endereco.cidade }}</div>
                  </div>
                </div>
                
                <div class="col-12 md:col-4">
                  <div class="field-group">
                    <label class="field-label">Estado</label>
                    <div class="field-value">{{ aluno.endereco.estado }}</div>
                  </div>
                </div>
              </div>
            </p-card>
          </p-tabPanel>

          <!-- Responsáveis -->
          <p-tabPanel header="Responsáveis" leftIcon="pi pi-users">
            <div class="grid">
              <div class="col-12 md:col-6" *ngFor="let responsavel of aluno.responsaveis">
                <p-card>
                  <ng-template pTemplate="header">
                    <div class="p-3">
                      <div class="flex justify-content-between align-items-center">
                        <h3 class="text-lg font-medium text-900 m-0">{{ responsavel.nome }}</h3>
                        <div class="flex gap-1">
                          <p-chip 
                            *ngIf="responsavel.responsavelFinanceiro"
                            label="Financeiro"
                            styleClass="p-chip-sm">
                          </p-chip>
                          <p-chip 
                            *ngIf="responsavel.responsavelAcademico"
                            label="Acadêmico"
                            styleClass="p-chip-sm">
                          </p-chip>
                          <p-chip 
                            *ngIf="responsavel.autorizadoBuscar"
                            label="Autorizado"
                            styleClass="p-chip-sm">
                          </p-chip>
                        </div>
                      </div>
                    </div>
                  </ng-template>
                  
                  <div class="field-group">
                    <label class="field-label">Tipo</label>
                    <div class="field-value">{{ responsavel.tipoDescricao }}</div>
                  </div>
                  
                  <div class="field-group">
                    <label class="field-label">CPF</label>
                    <div class="field-value">{{ responsavel.cpf }}</div>
                  </div>
                  
                  <div class="field-group">
                    <label class="field-label">Telefone</label>
                    <div class="field-value">{{ responsavel.telefone }}</div>
                  </div>
                  
                  <div class="field-group" *ngIf="responsavel.email">
                    <label class="field-label">E-mail</label>
                    <div class="field-value">{{ responsavel.email }}</div>
                  </div>
                  
                  <div class="field-group" *ngIf="responsavel.profissao">
                    <label class="field-label">Profissão</label>
                    <div class="field-value">{{ responsavel.profissao }}</div>
                  </div>
                  
                  <div class="field-group" *ngIf="responsavel.localTrabalho">
                    <label class="field-label">Local de Trabalho</label>
                    <div class="field-value">{{ responsavel.localTrabalho }}</div>
                  </div>
                  
                  <div class="field-group" *ngIf="responsavel.telefoneTrabalho">
                    <label class="field-label">Telefone do Trabalho</label>
                    <div class="field-value">{{ responsavel.telefoneTrabalho }}</div>
                  </div>
                </p-card>
              </div>
            </div>
          </p-tabPanel>

          <!-- Matrículas -->
          <p-tabPanel header="Matrículas" leftIcon="pi pi-id-card">
            <p-table 
              [value]="aluno.matriculas" 
              [paginator]="false"
              styleClass="p-datatable-gridlines"
              responsiveLayout="scroll">
              
              <ng-template pTemplate="header">
                <tr>
                  <th>Número</th>
                  <th>Turma</th>
                  <th>Ano Letivo</th>
                  <th>Data Matrícula</th>
                  <th>Status</th>
                  <th>Dias Matriculado</th>
                </tr>
              </ng-template>
              
              <ng-template pTemplate="body" let-matricula>
                <tr>
                  <td>{{ matricula.numeroMatricula }}</td>
                  <td>{{ matricula.nomeTurma }}</td>
                  <td>{{ matricula.anoLetivo }}</td>
                  <td>{{ matricula.dataMatricula | date:'dd/MM/yyyy' }}</td>
                  <td>
                    <p-tag 
                      [value]="matricula.statusDescricao"
                      [severity]="matricula.ativa ? 'success' : 'danger'">
                    </p-tag>
                  </td>
                  <td>{{ matricula.diasMatriculado }} dias</td>
                </tr>
              </ng-template>
              
              <ng-template pTemplate="emptymessage">
                <tr>
                  <td colspan="6" class="text-center py-4">
                    <div class="flex flex-column align-items-center">
                      <i class="pi pi-id-card text-4xl text-400 mb-3"></i>
                      <p class="text-600 mb-0">Nenhuma matrícula encontrada</p>
                    </div>
                  </td>
                </tr>
              </ng-template>
            </p-table>
          </p-tabPanel>
        </p-tabView>

        <!-- Pendências -->
        <p-card *ngIf="aluno.pendencias && aluno.pendencias.length > 0" styleClass="mt-4">
          <ng-template pTemplate="header">
            <div class="p-3">
              <h3 class="text-lg font-medium text-900 m-0 flex align-items-center">
                <i class="pi pi-exclamation-triangle text-orange-500 mr-2"></i>
                Pendências
              </h3>
            </div>
          </ng-template>
          
          <ul class="list-none p-0 m-0">
            <li *ngFor="let pendencia of aluno.pendencias" class="flex align-items-center p-2 border-bottom-1 surface-border">
              <i class="pi pi-circle-fill text-orange-500 mr-2" style="font-size: 0.5rem;"></i>
              {{ pendencia }}
            </li>
          </ul>
        </p-card>
      </div>

      <!-- Error State -->
      <div *ngIf="error$ | async as error" class="text-center p-4">
        <p-card>
          <div class="flex flex-column align-items-center p-4">
            <i class="pi pi-exclamation-triangle text-6xl text-red-500 mb-4"></i>
            <h3 class="text-xl font-bold text-900 mb-3">Erro ao carregar aluno</h3>
            <p class="text-600 mb-4 text-center">{{ error }}</p>
            <div class="flex gap-2">
              <p-button 
                label="Voltar à Lista" 
                icon="pi pi-arrow-left" 
                routerLink="/alunos">
              </p-button>
              <p-button 
                label="Tentar Novamente" 
                icon="pi pi-refresh" 
                styleClass="p-button-outlined"
                (onClick)="retry()"
                *ngIf="alunoId">
              </p-button>
            </div>
          </div>
        </p-card>
      </div>

      <!-- Invalid ID State -->
      <div *ngIf="invalidId">
        <div class="text-center p-4 mb-4">
          <p-card>
            <div class="flex flex-column align-items-center p-4">
              <i class="pi pi-times-circle text-6xl text-red-500 mb-4"></i>
              <h3 class="text-xl font-bold text-900 mb-3">ID de Aluno Inválido</h3>
              <p class="text-600 mb-2 text-center">
                O ID fornecido <strong>"{{ alunoId }}"</strong> não é um identificador válido.
              </p>
              <p class="text-500 mb-4 text-center text-sm">
                IDs de aluno devem ser no formato GUID (ex: de9ddec1-671c-11f0-9519-365ff6291686)
              </p>
              <div class="flex gap-2">
                <p-button 
                  label="Ir para Lista de Alunos" 
                  icon="pi pi-list" 
                  routerLink="/alunos">
                </p-button>
              </div>
            </div>
          </p-card>
        </div>

        <!-- Helper Component showing valid IDs -->
        <app-aluno-id-helper></app-aluno-id-helper>
      </div>

      <!-- Matriculation Modal -->
      <app-matricular-aluno-modal
        [alunoId]="alunoId!"
        [alunoNome]="aluno?.nome || ''"
        [visible]="showMatricularModal"
        (close)="onMatricularCancel()">
      </app-matricular-aluno-modal>
    </div>
  `,
  styles: [`
    .aluno-detail-container {
      padding: 1rem;
    }

    .field-group {
      margin-bottom: 1rem;
    }

    .field-label {
      display: block;
      font-weight: 600;
      color: var(--text-color-secondary);
      margin-bottom: 0.25rem;
      font-size: 0.875rem;
    }

    .field-value {
      color: var(--text-color);
      font-size: 1rem;
    }

    :host ::ng-deep .p-card .p-card-body {
      padding: 1.5rem;
    }

    :host ::ng-deep .p-chip {
      font-size: 0.75rem;
    }

    :host ::ng-deep .p-tabview .p-tabview-panels {
      padding: 0;
    }
  `]
})
export class AlunoDetailComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  alunoId: string | null = null;
  aluno: Aluno | null = null;
  invalidId = false;

  // Matriculation modal
  showMatricularModal = false;

  // Observables
  loading$;
  error$;

  constructor(
    private route: ActivatedRoute,
    private alunosFacade: AlunosFacade
  ) {
    this.loading$ = this.alunosFacade.loading$;
    this.error$ = this.alunosFacade.error$;
  }

  ngOnInit(): void {
    this.alunoId = this.route.snapshot.paramMap.get('id');
    
    if (this.alunoId) {
      // Validar se o ID parece ser um GUID válido
      if (!this.isValidGuid(this.alunoId)) {
        console.error('❌ ID inválido fornecido:', this.alunoId);
        this.invalidId = true;
        return;
      }

      this.alunosFacade.loadAluno(this.alunoId);
      
      this.alunosFacade.selectedAluno$.pipe(
        filter(aluno => !!aluno),
        takeUntil(this.destroy$)
      ).subscribe(aluno => {
        this.aluno = aluno;
      });
    }
  }

  private isValidGuid(id: string): boolean {
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
    return guidRegex.test(id);
  }

  // Matriculation methods
  matricularAluno(): void {
    this.showMatricularModal = true;
  }

  onMatricularCancel(): void {
    this.showMatricularModal = false;
  }

  retry(): void {
    if (this.alunoId && this.isValidGuid(this.alunoId)) {
      this.alunosFacade.clearErrors();
      this.alunosFacade.loadAluno(this.alunoId);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.alunosFacade.clearSelectedAluno();
  }
}