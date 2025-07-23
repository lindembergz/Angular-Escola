import { createAction, props } from '@ngrx/store';
import { 
  Aluno, 
  CriarAlunoRequest, 
  CriarAlunoResponse,
  AtualizarAlunoRequest,
  AtualizarAlunoResponse,
  MatricularAlunoRequest,
  MatricularAlunoResponse,
  ObterAlunosResponse,
  OpcoesAluno
} from '../../features/alunos/models/aluno.model';

// Load Actions
export const loadAlunos = createAction(
  '[Alunos] Load Alunos',
  props<{ 
    page?: number; 
    pageSize?: number; 
    searchTerm?: string; 
    escolaId?: string;
    turmaId?: string;
    ativo?: boolean;
  }>()
);

export const loadAlunosSuccess = createAction(
  '[Alunos] Load Alunos Success',
  props<{ response: ObterAlunosResponse }>()
);

export const loadAlunosFailure = createAction(
  '[Alunos] Load Alunos Failure',
  props<{ error: string }>()
);

// Load Single Aluno
export const loadAluno = createAction(
  '[Alunos] Load Aluno',
  props<{ id: string }>()
);

export const loadAlunoSuccess = createAction(
  '[Alunos] Load Aluno Success',
  props<{ aluno: Aluno }>()
);

export const loadAlunoFailure = createAction(
  '[Alunos] Load Aluno Failure',
  props<{ error: string }>()
);

// Create Aluno
export const createAluno = createAction(
  '[Alunos] Create Aluno',
  props<{ request: CriarAlunoRequest }>()
);

export const createAlunoSuccess = createAction(
  '[Alunos] Create Aluno Success',
  props<{ response: CriarAlunoResponse }>()
);

export const createAlunoFailure = createAction(
  '[Alunos] Create Aluno Failure',
  props<{ error: string }>()
);

// Update Aluno
export const updateAluno = createAction(
  '[Alunos] Update Aluno',
  props<{ request: AtualizarAlunoRequest }>()
);

export const updateAlunoSuccess = createAction(
  '[Alunos] Update Aluno Success',
  props<{ response: AtualizarAlunoResponse }>()
);

export const updateAlunoFailure = createAction(
  '[Alunos] Update Aluno Failure',
  props<{ error: string }>()
);

// Delete Aluno
export const deleteAluno = createAction(
  '[Alunos] Delete Aluno',
  props<{ id: string }>()
);

export const deleteAlunoSuccess = createAction(
  '[Alunos] Delete Aluno Success',
  props<{ id: string }>()
);

export const deleteAlunoFailure = createAction(
  '[Alunos] Delete Aluno Failure',
  props<{ error: string }>()
);

// Load Options
export const loadOpcoesAluno = createAction('[Alunos] Load Opcoes Aluno');

export const loadOpcoesAlunoSuccess = createAction(
  '[Alunos] Load Opcoes Aluno Success',
  props<{ opcoes: OpcoesAluno }>()
);

export const loadOpcoesAlunoFailure = createAction(
  '[Alunos] Load Opcoes Aluno Failure',
  props<{ error: string }>()
);

// UI Actions
export const setSelectedAluno = createAction(
  '[Alunos] Set Selected Aluno',
  props<{ aluno: Aluno | null }>()
);

export const clearSelectedAluno = createAction('[Alunos] Clear Selected Aluno');

export const setLoading = createAction(
  '[Alunos] Set Loading',
  props<{ loading: boolean }>()
);

export const clearErrors = createAction('[Alunos] Clear Errors');

// Filter Actions
export const setFilters = createAction(
  '[Alunos] Set Filters',
  props<{ 
    searchTerm?: string; 
    escolaId?: string;
    turmaId?: string;
    ativo?: boolean;
  }>()
);

export const clearFilters = createAction('[Alunos] Clear Filters');

// Pagination Actions
export const setPagination = createAction(
  '[Alunos] Set Pagination',
  props<{ page: number; pageSize: number }>()
);

// Matricular Aluno
export const matricularAluno = createAction(
  '[Alunos] Matricular Aluno',
  props<{ alunoId: string; request: MatricularAlunoRequest }>()
);

export const matricularAlunoSuccess = createAction(
  '[Alunos] Matricular Aluno Success',
  props<{ response: MatricularAlunoResponse }>()
);

export const matricularAlunoFailure = createAction(
  '[Alunos] Matricular Aluno Failure',
  props<{ error: string }>()
);