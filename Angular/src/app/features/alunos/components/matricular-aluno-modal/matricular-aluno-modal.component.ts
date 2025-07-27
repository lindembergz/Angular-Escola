import { Component, Input, Output, EventEmitter, OnInit, OnDestroy, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

// PrimeNG Imports
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextarea } from 'primeng/inputtextarea';
import { ButtonModule } from 'primeng/button';

import { AlunosFacade } from '../../../../store/alunos/alunos.facade';
import { MatricularAlunoRequest } from '../../models/aluno.model';
import { AppState } from '../../../../store/app.state';
import * as AcademicoActions from '../../../../store/academico/academico.actions';
import * as AcademicoSelectors from '../../../../store/academico/academico.selectors';
import { Turma } from '../../../academico/models/turma.model';

@Component({
  selector: 'app-matricular-aluno-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DialogModule,
    DropdownModule,
    InputNumberModule,
    InputTextarea,
    ButtonModule
  ],
  templateUrl: './matricular-aluno-modal.component.html',
  styleUrls: ['./matricular-aluno-modal.component.scss']
})
export class MatricularAlunoModalComponent implements OnInit, OnDestroy, OnChanges {
  @Input() alunoId!: string;
  @Input() alunoNome!: string;
  @Input() visible: boolean = false;
  @Output() close = new EventEmitter<void>();

  matriculaForm!: FormGroup;
  turmas$: Observable<Turma[]>;
  loading$: Observable<boolean>;
  loading = false;

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private alunosFacade: AlunosFacade,
    private store: Store<AppState>
  ) {
    // Usar apenas turmas ativas para matrícula
    this.turmas$ = this.store.select(AcademicoSelectors.selectTurmasAtivas);
    this.loading$ = this.store.select(AcademicoSelectors.selectLoading);
    
    // Debug: verificar o estado completo do academico
    this.store.select(AcademicoSelectors.selectAcademicoState).subscribe(state => {
      console.log('🏛️ Estado completo do academico:', state);
    });
  }

  ngOnInit(): void {
    this.initializeForm();
    
    // Debug: verificar se as turmas estão chegando
    this.turmas$.pipe(takeUntil(this.destroy$)).subscribe(turmas => {
      console.log('🏫 Turmas recebidas no modal:', turmas);
      console.log('🏫 Quantidade de turmas:', turmas?.length || 0);
    });
  }

  ngOnChanges(): void {
    // Carregar turmas quando o modal ficar visível
    if (this.visible) {
      console.log('👁️ Modal ficou visível, carregando turmas...');
      this.loadTurmas();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeForm(): void {
    this.matriculaForm = this.fb.group({
      turmaId: ['', Validators.required],
      anoLetivo: [new Date().getFullYear(), [Validators.required, Validators.min(2020), Validators.max(2050)]],
      observacoes: ['']
    });
  }

  private loadTurmas(): void {
    // Load active turmas from the store
    console.log('🔄 Carregando turmas ativas para o ano:', new Date().getFullYear());
    this.store.dispatch(AcademicoActions.loadTurmas({
      ativa: true,
      anoLetivo: new Date().getFullYear()
    }));
  }

  onSubmit(): void {
    if (this.matriculaForm.valid && !this.loading) {
      this.loading = true;

      const request: MatricularAlunoRequest = {
        turmaId: this.matriculaForm.value.turmaId,
        anoLetivo: this.matriculaForm.value.anoLetivo,
        observacoes: this.matriculaForm.value.observacoes || undefined
      };

      this.alunosFacade.matricularAluno(this.alunoId, request);

      // Subscribe to loading state to close modal when operation completes
      this.alunosFacade.loading$
        .pipe(takeUntil(this.destroy$))
        .subscribe(loading => {
          if (!loading && this.loading) {
            this.loading = false;
            this.onCancel();
          }
        });
    }
  }

  onCancel(): void {
    this.matriculaForm.reset();
    this.loading = false;
    this.close.emit();
  }

  get isFormValid(): boolean {
    return this.matriculaForm.valid;
  }

  get turmaIdControl() {
    return this.matriculaForm.get('turmaId');
  }

  get anoLetivoControl() {
    return this.matriculaForm.get('anoLetivo');
  }
}