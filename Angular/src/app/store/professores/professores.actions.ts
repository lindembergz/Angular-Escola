import { createAction, props } from '@ngrx/store';
import {
  ProfessorResumo,
  Professor,
  CriarProfessorRequest,
  AtualizarProfessorRequest,
  AtribuirDisciplinaRequest,
  FiltrosProfessor,
  PaginatedResult,
  EstatisticasProfessor,
  OpcoesProfessor
} from '../../features/professores/models/professor.model';

// Load Professores Actions
export const loadProfessores = createAction(
  '[Professores] Load Professores',
  props<{ filtros: FiltrosProfessor }>()
);

export const loadProfessoresSuccess = createAction(
  '[Professores] Load Professores Success',
  props<{ result: PaginatedResult<ProfessorResumo> }>()
);

export const loadProfessoresFailure = createAction(
  '[Professores] Load Professores Failure',
  props<{ error: string }>()
);

// Load Professor Detail Actions
export const loadProfessorDetail = createAction(
  '[Professores] Load Professor Detail',
  props<{ id: string }>()
);

export const loadProfessorDetailSuccess = createAction(
  '[Professores] Load Professor Detail Success',
  props<{ professor: Professor }>()
);

export const loadProfessorDetailFailure = createAction(
  '[Professores] Load Professor Detail Failure',
  props<{ error: string }>()
);

// Create Professor Actions
export const createProfessor = createAction(
  '[Professores] Create Professor',
  props<{ request: CriarProfessorRequest }>()
);

export const createProfessorSuccess = createAction(
  '[Professores] Create Professor Success',
  props<{ professorId: string }>()
);

export const createProfessorFailure = createAction(
  '[Professores] Create Professor Failure',
  props<{ error: string }>()
);

// Update Professor Actions
export const updateProfessor = createAction(
  '[Professores] Update Professor',
  props<{ request: AtualizarProfessorRequest }>()
);

export const updateProfessorSuccess = createAction(
  '[Professores] Update Professor Success'
);

export const updateProfessorFailure = createAction(
  '[Professores] Update Professor Failure',
  props<{ error: string }>()
);

// Deactivate Professor Actions
export const deactivateProfessor = createAction(
  '[Professores] Deactivate Professor',
  props<{ id: string }>()
);

export const deactivateProfessorSuccess = createAction(
  '[Professores] Deactivate Professor Success',
  props<{ id: string }>()
);

export const deactivateProfessorFailure = createAction(
  '[Professores] Deactivate Professor Failure',
  props<{ error: string }>()
);

// Assign Discipline Actions
export const assignDiscipline = createAction(
  '[Professores] Assign Discipline',
  props<{ professorId: string; request: AtribuirDisciplinaRequest }>()
);

export const assignDisciplineSuccess = createAction(
  '[Professores] Assign Discipline Success'
);

export const assignDisciplineFailure = createAction(
  '[Professores] Assign Discipline Failure',
  props<{ error: string }>()
);

// Remove Discipline Actions
export const removeDiscipline = createAction(
  '[Professores] Remove Discipline',
  props<{ professorId: string; disciplinaId: string }>()
);

export const removeDisciplineSuccess = createAction(
  '[Professores] Remove Discipline Success'
);

export const removeDisciplineFailure = createAction(
  '[Professores] Remove Discipline Failure',
  props<{ error: string }>()
);

// Search Professores Actions
export const searchProfessores = createAction(
  '[Professores] Search Professores',
  props<{ nome: string; escolaId?: string; filtros?: Partial<FiltrosProfessor> }>()
);

export const searchProfessoresSuccess = createAction(
  '[Professores] Search Professores Success',
  props<{ result: PaginatedResult<ProfessorResumo> }>()
);

export const searchProfessoresFailure = createAction(
  '[Professores] Search Professores Failure',
  props<{ error: string }>()
);

// Load Statistics Actions
export const loadStatistics = createAction(
  '[Professores] Load Statistics',
  props<{ escolaId: string }>()
);

export const loadStatisticsSuccess = createAction(
  '[Professores] Load Statistics Success',
  props<{ statistics: EstatisticasProfessor }>()
);

export const loadStatisticsFailure = createAction(
  '[Professores] Load Statistics Failure',
  props<{ error: string }>()
);

// Load Options Actions
export const loadOptions = createAction(
  '[Professores] Load Options'
);

export const loadOptionsSuccess = createAction(
  '[Professores] Load Options Success',
  props<{ options: OpcoesProfessor }>()
);

export const loadOptionsFailure = createAction(
  '[Professores] Load Options Failure',
  props<{ error: string }>()
);

// UI Actions
export const setCurrentFilters = createAction(
  '[Professores] Set Current Filters',
  props<{ filtros: FiltrosProfessor }>()
);

export const clearProfessorDetail = createAction(
  '[Professores] Clear Professor Detail'
);

export const clearError = createAction(
  '[Professores] Clear Error'
);

export const setLoading = createAction(
  '[Professores] Set Loading',
  props<{ loading: boolean }>()
);