import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AlunosFacade } from '../../../../store/alunos/alunos.facade';
import { MatricularAlunoRequest } from '../../models/aluno.model';

// Mock interface for Turma - this should come from the Turmas module when implemented
interface Turma {
  id: string;
  nome: string;
  serie: string;
  anoLetivo: number;
}

@Component({
  selector: 'app-matricular-aluno-modal',
  templateUrl: './matricular-aluno-modal.component.html',
  styleUrls: ['./matricular-aluno-modal.component.scss']
})
export class MatricularAlunoModalComponent implements OnInit {
  @Input() alunoId!: string;
  @Input() alunoNome!: string;
  @Input() visible: boolean = false;
  @Output() close = new EventEmitter<void>();

  matriculaForm!: FormGroup;
  turmas: Turma[] = []; // This should come from TurmasFacade when implemented
  loading = false;

  constructor(
    private fb: FormBuilder,
    private alunosFacade: AlunosFacade
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.loadTurmas();
  }

  private initializeForm(): void {
    this.matriculaForm = this.fb.group({
      turmaId: ['', Validators.required],
      anoLetivo: [new Date().getFullYear(), [Validators.required, Validators.min(2020), Validators.max(2050)]],
      observacoes: ['']
    });
  }

  private loadTurmas(): void {
    // Mock data - this should come from TurmasFacade when implemented
    this.turmas = [
      { id: '1', nome: '1º Ano A', serie: '1º Ano', anoLetivo: 2024 },
      { id: '2', nome: '1º Ano B', serie: '1º Ano', anoLetivo: 2024 },
      { id: '3', nome: '2º Ano A', serie: '2º Ano', anoLetivo: 2024 },
      { id: '4', nome: '3º Ano A', serie: '3º Ano', anoLetivo: 2024 }
    ];
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
      this.alunosFacade.loading$.subscribe(loading => {
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