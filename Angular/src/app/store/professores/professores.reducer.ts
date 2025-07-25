import { createReducer, on } from '@ngrx/store';
import {
  ProfessorResumo,
  Professor,
  FiltrosProfessor,
  EstatisticasProfessor,
  OpcoesProfessor
} from '../../features/professores/models/professor.model';
import * as ProfessoresActions from './professores.actions';

export interface ProfessoresState {
  // Data
  professores: ProfessorResumo[];
  currentProfessor: Professor | null;
  statistics: EstatisticasProfessor | null;
  options: OpcoesProfessor | null;
  
  // Pagination
  totalItems: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
  
  // Filters
  currentFilters: FiltrosProfessor;
  
  // UI State
  loading: boolean;
  error: string | null;
  
  // Operation states
  creating: boolean;
  updating: boolean;
  deleting: boolean;
}

export const initialState: ProfessoresState = {
  // Data
  professores: [],
  currentProfessor: null,
  statistics: null,
  options: null,
  
  // Pagination
  totalItems: 0,
  currentPage: 1,
  pageSize: 10,
  totalPages: 0,
  
  // Filters
  currentFilters: {
    pagina: 1,
    tamanhoPagina: 10
  },
  
  // UI State
  loading: false,
  error: null,
  
  // Operation states
  creating: false,
  updating: false,
  deleting: false
};

export const professoresReducer = createReducer(
  initialState,

  // Load Professores
  on(ProfessoresActions.loadProfessores, (state, { filtros }) => ({
    ...state,
    loading: true,
    error: null,
    currentFilters: filtros
  })),

  on(ProfessoresActions.loadProfessoresSuccess, (state, { result }) => ({
    ...state,
    loading: false,
    professores: result.items,
    totalItems: result.totalItems,
    currentPage: result.pagina,
    pageSize: result.tamanhoPagina,
    totalPages: result.totalPaginas,
    error: null
  })),

  on(ProfessoresActions.loadProfessoresFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Load Professor Detail
  on(ProfessoresActions.loadProfessorDetail, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(ProfessoresActions.loadProfessorDetailSuccess, (state, { professor }) => ({
    ...state,
    loading: false,
    currentProfessor: professor,
    error: null
  })),

  on(ProfessoresActions.loadProfessorDetailFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Create Professor
  on(ProfessoresActions.createProfessor, (state) => ({
    ...state,
    creating: true,
    error: null
  })),

  on(ProfessoresActions.createProfessorSuccess, (state) => ({
    ...state,
    creating: false,
    error: null
  })),

  on(ProfessoresActions.createProfessorFailure, (state, { error }) => ({
    ...state,
    creating: false,
    error
  })),

  // Update Professor
  on(ProfessoresActions.updateProfessor, (state) => ({
    ...state,
    updating: true,
    error: null
  })),

  on(ProfessoresActions.updateProfessorSuccess, (state) => ({
    ...state,
    updating: false,
    error: null
  })),

  on(ProfessoresActions.updateProfessorFailure, (state, { error }) => ({
    ...state,
    updating: false,
    error
  })),

  // Deactivate Professor
  on(ProfessoresActions.deactivateProfessor, (state) => ({
    ...state,
    deleting: true,
    error: null
  })),

  on(ProfessoresActions.deactivateProfessorSuccess, (state, { id }) => ({
    ...state,
    deleting: false,
    professores: state.professores.map(p => 
      p.id === id ? { ...p, ativo: false } : p
    ),
    error: null
  })),

  on(ProfessoresActions.deactivateProfessorFailure, (state, { error }) => ({
    ...state,
    deleting: false,
    error
  })),

  // Assign/Remove Discipline
  on(ProfessoresActions.assignDiscipline, (state) => ({
    ...state,
    updating: true,
    error: null
  })),

  on(ProfessoresActions.assignDisciplineSuccess, (state) => ({
    ...state,
    updating: false,
    error: null
  })),

  on(ProfessoresActions.assignDisciplineFailure, (state, { error }) => ({
    ...state,
    updating: false,
    error
  })),

  on(ProfessoresActions.removeDiscipline, (state) => ({
    ...state,
    updating: true,
    error: null
  })),

  on(ProfessoresActions.removeDisciplineSuccess, (state) => ({
    ...state,
    updating: false,
    error: null
  })),

  on(ProfessoresActions.removeDisciplineFailure, (state, { error }) => ({
    ...state,
    updating: false,
    error
  })),

  // Search Professores
  on(ProfessoresActions.searchProfessores, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(ProfessoresActions.searchProfessoresSuccess, (state, { result }) => ({
    ...state,
    loading: false,
    professores: result.items,
    totalItems: result.totalItems,
    currentPage: result.pagina,
    pageSize: result.tamanhoPagina,
    totalPages: result.totalPaginas,
    error: null
  })),

  on(ProfessoresActions.searchProfessoresFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Load Statistics
  on(ProfessoresActions.loadStatistics, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(ProfessoresActions.loadStatisticsSuccess, (state, { statistics }) => ({
    ...state,
    loading: false,
    statistics,
    error: null
  })),

  on(ProfessoresActions.loadStatisticsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Load Options
  on(ProfessoresActions.loadOptions, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(ProfessoresActions.loadOptionsSuccess, (state, { options }) => ({
    ...state,
    loading: false,
    options,
    error: null
  })),

  on(ProfessoresActions.loadOptionsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // UI Actions
  on(ProfessoresActions.setCurrentFilters, (state, { filtros }) => ({
    ...state,
    currentFilters: filtros
  })),

  on(ProfessoresActions.clearProfessorDetail, (state) => ({
    ...state,
    currentProfessor: null
  })),

  on(ProfessoresActions.clearError, (state) => ({
    ...state,
    error: null
  })),

  on(ProfessoresActions.setLoading, (state, { loading }) => ({
    ...state,
    loading
  }))
);