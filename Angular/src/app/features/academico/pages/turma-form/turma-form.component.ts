import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil, filter } from 'rxjs/operators';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { MessageModule } from 'primeng/message';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

// Store
import { AppState } from '../../../../store/app.state';
import * as AcademicoActions from '../../../../store/academico/academico.actions';
import * as AcademicoSelectors from '../../../../store/academico/academico.selectors';

// Models
import { Turma, CriarTurmaRequest, AtualizarTurmaRequest, OpcoesTurma } from '../../models/turma.model';

@Component({
  selector: 'app-turma-form',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    CardModule,
    ButtonModule,
    InputTextModule,
    DropdownModule,
    InputNumberModule,
    MessageModule,
    ProgressSpinnerModule
  ],
  template: `
    <div class="turma-form-container">
      <p-card>
        <ng-template pTemplate="header">
          <div class="flex justify-content-between align-items-center">
            <h2 class="m-0">
              <i class="pi pi-plus mr-2" *ngIf="!isEditMode"></i>
              <i class="pi pi-pencil mr-2" *ngIf="isEditMode"></i>
              {{ isEditMode ? 'Editar Turma' : 'Nova Turma' }}
            </h2>
            <button 
              pButton 
              type="button" 
              icon="pi pi-times" 
              class="p-button-text p-button-rounded"
              (click)="voltar()">
            </button>
          </div>
        </ng-template>

        <div *ngIf="loading$ | async" class="flex justify-content-center p-4">
          <p-progressSpinner></p-progressSpinner>
        </div>

        <form [formGroup]="turmaForm" (ngSubmit)="onSubmit()" *ngIf="!(loading$ | async)">
          <div class="grid">
            <div class="col-12 md:col-6">
              <label for="nome" class="block text-900 font-medium mb-2">
                Nome da Turma <span class="text-red-500">*</span>
              </label>
              <input 
                pInputText 
                id="nome"
                formControlName="nome"
                placeholder="Ex: 1º Ano A"
                class="w-full"
                [class.ng-invalid]="turmaForm.get('nome')?.invalid && turmaForm.get('nome')?.touched">
              <small 
                class="p-error block mt-1" 
                *ngIf="turmaForm.get('nome')?.invalid && turmaForm.get('nome')?.touched">
                Nome da turma é obrigatório
              </small>
            </div>

            <div class="col-12 md:col-6">
              <label for="serie" class="block text-900 font-medium mb-2">
                Série <span class="text-red-500">*</span>
              </label>
              <p-dropdown
                id="serie"
                formControlName="serie"
                [options]="serieOptions"
                placeholder="Selecione a série"
                styleClass="w-full"
                [class.ng-invalid]="turmaForm.get('serie')?.invalid && turmaForm.get('serie')?.touched">
              </p-dropdown>
              <small 
                class="p-error block mt-1" 
                *ngIf="turmaForm.get('serie')?.invalid && turmaForm.get('serie')?.touched">
                Série é obrigatória
              </small>
            </div>

            <div class="col-12 md:col-6">
              <label for="turno" class="block text-900 font-medium mb-2">
                Turno <span class="text-red-500">*</span>
              </label>
              <p-dropdown
                id="turno"
                formControlName="turno"
                [options]="turnoOptions"
                placeholder="Selecione o turno"
                styleClass="w-full"
                [class.ng-invalid]="turmaForm.get('turno')?.invalid && turmaForm.get('turno')?.touched">
              </p-dropdown>
              <small 
                class="p-error block mt-1" 
                *ngIf="turmaForm.get('turno')?.invalid && turmaForm.get('turno')?.touched">
                Turno é obrigatório
              </small>
            </div>

            <div class="col-12 md:col-6">
              <label for="anoLetivo" class="block text-900 font-medium mb-2">
                Ano Letivo <span class="text-red-500">*</span>
              </label>
              <p-dropdown
                id="anoLetivo"
                formControlName="anoLetivo"
                [options]="anoLetivoOptions"
                placeholder="Selecione o ano letivo"
                styleClass="w-full"
                [class.ng-invalid]="turmaForm.get('anoLetivo')?.invalid && turmaForm.get('anoLetivo')?.touched">
              </p-dropdown>
              <small 
                class="p-error block mt-1" 
                *ngIf="turmaForm.get('anoLetivo')?.invalid && turmaForm.get('anoLetivo')?.touched">
                Ano letivo é obrigatório
              </small>
            </div>

            <div class="col-12 md:col-6">
              <label for="capacidadeMaxima" class="block text-900 font-medium mb-2">
                Capacidade Máxima <span class="text-red-500">*</span>
              </label>
              <p-inputNumber
                id="capacidadeMaxima"
                formControlName="capacidadeMaxima"
                [min]="1"
                [max]="100"
                placeholder="Ex: 30"
                styleClass="w-full"
                [class.ng-invalid]="turmaForm.get('capacidadeMaxima')?.invalid && turmaForm.get('capacidadeMaxima')?.touched">
              </p-inputNumber>
              <small 
                class="p-error block mt-1" 
                *ngIf="turmaForm.get('capacidadeMaxima')?.invalid && turmaForm.get('capacidadeMaxima')?.touched">
                Capacidade máxima deve ser entre 1 e 100 alunos
              </small>
            </div>

            <div class="col-12 md:col-6" *ngIf="!isEditMode">
              <label for="escolaId" class="block text-900 font-medium mb-2">
                Escola <span class="text-red-500">*</span>
              </label>
              <p-dropdown
                id="escolaId"
                formControlName="escolaId"
                [options]="escolaOptions"
                placeholder="Selecione a escola"
                styleClass="w-full"
                [class.ng-invalid]="turmaForm.get('escolaId')?.invalid && turmaForm.get('escolaId')?.touched">
              </p-dropdown>
              <small 
                class="p-error block mt-1" 
                *ngIf="turmaForm.get('escolaId')?.invalid && turmaForm.get('escolaId')?.touched">
                Escola é obrigatória
              </small>
            </div>
          </div>

          <div class="flex justify-content-end gap-2 mt-4">
            <button 
              pButton 
              type="button" 
              label="Cancelar" 
              icon="pi pi-times" 
              class="p-button-outlined"
              (click)="voltar()">
            </button>
            <button 
              pButton 
              type="submit" 
              [label]="isEditMode ? 'Atualizar' : 'Criar'" 
              [icon]="isEditMode ? 'pi pi-check' : 'pi pi-plus'"
              class="p-button-success"
              [disabled]="turmaForm.invalid || (loading$ | async)">
            </button>
          </div>
        </form>

        <p-message 
          *ngIf="error$ | async as error" 
          severity="error" 
          [text]="error"
          class="mt-3">
        </p-message>
      </p-card>
    </div>
  `,
  styles: [`
    .turma-form-container {
      padding: 1rem;
      max-width: 800px;
      margin: 0 auto;
    }
    
    .p-inputnumber {
      width: 100%;
    }
    
    .p-inputnumber input {
      width: 100%;
    }
  `]
})
export class TurmaFormComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  turmaForm: FormGroup;
  isEditMode = false;
  turmaId: string | null = null;
  
  loading$: Observable<boolean>;
  error$: Observable<string | null>;
  opcoesTurma$: Observable<OpcoesTurma | null>;
  selectedTurma$: Observable<Turma | null>;
  
  serieOptions: any[] = [];
  turnoOptions: any[] = [];
  anoLetivoOptions: any[] = [];
  escolaOptions: any[] = [
    { label: 'Escola Principal', value: '1' } // Mock - em produção viria do backend
  ];

  constructor(
    private store: Store<AppState>,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.loading$ = this.store.select(AcademicoSelectors.selectLoading);
    this.error$ = this.store.select(AcademicoSelectors.selectError);
    this.opcoesTurma$ = this.store.select(AcademicoSelectors.selectOpcoesTurma);
    this.selectedTurma$ = this.store.select(AcademicoSelectors.selectSelectedTurma);
    
    this.turmaForm = this.fb.group({
      nome: ['', [Validators.required, Validators.minLength(2)]],
      serie: ['', Validators.required],
      turno: ['', Validators.required],
      anoLetivo: [new Date().getFullYear(), Validators.required],
      capacidadeMaxima: [30, [Validators.required, Validators.min(1), Validators.max(100)]],
      escolaId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.verificarModoEdicao();
    this.carregarOpcoes();
    
    if (this.isEditMode && this.turmaId) {
      this.carregarTurma();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.store.dispatch(AcademicoActions.clearSelectedItems());
  }

  private verificarModoEdicao(): void {
    this.turmaId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.turmaId;
    
    if (!this.isEditMode) {
      // Remove o campo escolaId das validações no modo de edição
      this.turmaForm.removeControl('escolaId');
    }
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

  private carregarTurma(): void {
    if (this.turmaId) {
      this.store.dispatch(AcademicoActions.loadTurma({ id: this.turmaId }));
      
      this.selectedTurma$
        .pipe(
          takeUntil(this.destroy$),
          filter(turma => !!turma)
        )
        .subscribe(turma => {
          if (turma) {
            this.turmaForm.patchValue({
              nome: turma.nome,
              serie: turma.serie,
              turno: turma.turno,
              anoLetivo: turma.anoLetivo,
              capacidadeMaxima: turma.capacidadeMaxima
            });
          }
        });
    }
  }

  onSubmit(): void {
    if (this.turmaForm.valid) {
      const formValue = this.turmaForm.value;
      
      if (this.isEditMode && this.turmaId) {
        const request: AtualizarTurmaRequest = {
          id: this.turmaId,
          nome: formValue.nome,
          capacidadeMaxima: formValue.capacidadeMaxima
        };
        this.store.dispatch(AcademicoActions.updateTurma({ request }));
      } else {
        const request: CriarTurmaRequest = {
          nome: formValue.nome,
          serie: formValue.serie,
          turno: formValue.turno,
          anoLetivo: formValue.anoLetivo,
          capacidadeMaxima: formValue.capacidadeMaxima,
          escolaId: formValue.escolaId
        };
        this.store.dispatch(AcademicoActions.createTurma({ request }));
      }
    } else {
      // Marcar todos os campos como touched para mostrar erros
      Object.keys(this.turmaForm.controls).forEach(key => {
        this.turmaForm.get(key)?.markAsTouched();
      });
    }
  }

  voltar(): void {
    this.router.navigate(['/academico/turmas']);
  }
}