import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Aluno, AlunoResumo, OpcoesAluno } from '../../features/alunos/models/aluno.model';
import * as AlunosActions from './alunos.actions';

export interface AlunosState extends EntityState<AlunoResumo> {
  selectedAluno: Aluno | null;
  opcoes: OpcoesAluno | null;
  loading: boolean;
  error: string | null;
  
  // Pagination
  totalRegistros: number;
  totalPaginas: number;
  paginaAtual: number;
  tamanhoPagina: number;
  
  // Filters
  filters: {
    searchTerm?: string;
    escolaId?: string;
    turmaId?: string;
    ativo?: boolean;
  };
}

export const adapter: EntityAdapter<AlunoResumo> = createEntityAdapter<AlunoResumo>({
  selectId: (aluno: AlunoResumo) => aluno.id,
  sortComparer: (a: AlunoResumo, b: AlunoResumo) => a.nome.localeCompare(b.nome)
});

export const initialState: AlunosState = adapter.getInitialState({
  selectedAluno: null,
  opcoes: null,
  loading: false,
  error: null,
  totalRegistros: 0,
  totalPaginas: 0,
  paginaAtual: 1,
  tamanhoPagina: 20,
  filters: {}
});

export const alunosReducer = createReducer(
  initialState,

  // Load Alunos
  on(AlunosActions.loadAlunos, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(AlunosActions.loadAlunosSuccess, (state, { response }) => {
    console.log('🔄 Reducer loadAlunosSuccess called with:', response);
    
    if (!response) {
      console.error('❌ Response is undefined in reducer');
      return {
        ...state,
        loading: false,
        error: 'Resposta inválida do servidor'
      };
    }
    
    console.log('📊 Alunos to be set in store:', response.alunos);
    console.log('📊 Total records:', response.totalRegistros);
    
    const newState = adapter.setAll(response.alunos || [], {
      ...state,
      loading: false,
      error: null,
      totalRegistros: response.totalRegistros || 0,
      totalPaginas: response.totalPaginas || 0,
      paginaAtual: response.paginaAtual || 1,
      tamanhoPagina: response.tamanhoPagina || 20
    });
    
    console.log('🏪 New state after setAll:', newState);
    return newState;
  }),

  on(AlunosActions.loadAlunosFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Load Single Aluno
  on(AlunosActions.loadAluno, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(AlunosActions.loadAlunoSuccess, (state, { aluno }) => ({
    ...state,
    selectedAluno: aluno,
    loading: false,
    error: null
  })),

  on(AlunosActions.loadAlunoFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Create Aluno
  on(AlunosActions.createAluno, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(AlunosActions.createAlunoSuccess, (state, { response: _ }) => ({
    ...state,
    loading: false,
    error: null
  })),

  on(AlunosActions.createAlunoFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Update Aluno
  on(AlunosActions.updateAluno, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(AlunosActions.updateAlunoSuccess, (state, { response: _ }) => ({
    ...state,
    loading: false,
    error: null
  })),

  on(AlunosActions.updateAlunoFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Delete Aluno
  on(AlunosActions.deleteAluno, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(AlunosActions.deleteAlunoSuccess, (state, { id }) =>
    adapter.removeOne(id, {
      ...state,
      loading: false,
      error: null
    })
  ),

  on(AlunosActions.deleteAlunoFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Load Options
  on(AlunosActions.loadOpcoesAluno, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(AlunosActions.loadOpcoesAlunoSuccess, (state, { opcoes }) => ({
    ...state,
    opcoes,
    loading: false,
    error: null
  })),

  on(AlunosActions.loadOpcoesAlunoFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // UI Actions
  on(AlunosActions.setSelectedAluno, (state, { aluno }) => ({
    ...state,
    selectedAluno: aluno
  })),

  on(AlunosActions.clearSelectedAluno, (state) => ({
    ...state,
    selectedAluno: null
  })),

  on(AlunosActions.setLoading, (state, { loading }) => ({
    ...state,
    loading
  })),

  on(AlunosActions.clearErrors, (state) => ({
    ...state,
    error: null
  })),

  // Filter Actions
  on(AlunosActions.setFilters, (state, { searchTerm, escolaId, turmaId, ativo }) => ({
    ...state,
    filters: {
      ...state.filters,
      ...(searchTerm !== undefined && { searchTerm }),
      ...(escolaId !== undefined && { escolaId }),
      ...(turmaId !== undefined && { turmaId }),
      ...(ativo !== undefined && { ativo })
    }
  })),

  on(AlunosActions.clearFilters, (state) => ({
    ...state,
    filters: {}
  })),

  // Pagination Actions
  on(AlunosActions.setPagination, (state, { page, pageSize }) => ({
    ...state,
    paginaAtual: page,
    tamanhoPagina: pageSize
  })),

  // Matricular Aluno
  on(AlunosActions.matricularAluno, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(AlunosActions.matricularAlunoSuccess, (state, { response: _ }) => ({
    ...state,
    loading: false,
    error: null
  })),

  on(AlunosActions.matricularAlunoFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  }))
);

// Export selectors
export const {
  selectIds,
  selectEntities,
  selectAll,
  selectTotal,
} = adapter.getSelectors();