import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AlunosState, selectAll, selectEntities } from './alunos.reducer';

export const selectAlunosState = createFeatureSelector<AlunosState>('alunos');

// Entity selectors
export const selectAllAlunos = createSelector(
  selectAlunosState,
  state => {
    const alunos = selectAll(state);
    console.log('🔍 selectAllAlunos called, state:', state);
    console.log('🔍 selectAllAlunos returning:', alunos);
    return alunos;
  }
);

export const selectAlunosEntities = createSelector(
  selectAlunosState,
  selectEntities
);

export const selectAlunoById = (id: string) => createSelector(
  selectAlunosEntities,
  (entities) => entities[id]
);

// UI selectors
export const selectSelectedAluno = createSelector(
  selectAlunosState,
  (state) => state.selectedAluno
);

export const selectAlunosLoading = createSelector(
  selectAlunosState,
  (state) => state.loading
);

export const selectAlunosError = createSelector(
  selectAlunosState,
  (state) => state.error
);

export const selectOpcoesAluno = createSelector(
  selectAlunosState,
  (state) => state.opcoes
);

// Pagination selectors
export const selectPaginationInfo = createSelector(
  selectAlunosState,
  (state) => ({
    totalRegistros: state.totalRegistros,
    totalPaginas: state.totalPaginas,
    paginaAtual: state.paginaAtual,
    tamanhoPagina: state.tamanhoPagina
  })
);

export const selectCurrentPage = createSelector(
  selectAlunosState,
  (state) => state.paginaAtual
);

export const selectPageSize = createSelector(
  selectAlunosState,
  (state) => state.tamanhoPagina
);

export const selectTotalRecords = createSelector(
  selectAlunosState,
  (state) => state.totalRegistros
);

// Filter selectors
export const selectFilters = createSelector(
  selectAlunosState,
  (state) => state.filters
);

export const selectSearchTerm = createSelector(
  selectFilters,
  (filters) => filters.searchTerm
);

export const selectEscolaFilter = createSelector(
  selectFilters,
  (filters) => filters.escolaId
);

export const selectTurmaFilter = createSelector(
  selectFilters,
  (filters) => filters.turmaId
);

export const selectAtivoFilter = createSelector(
  selectFilters,
  (filters) => filters.ativo
);

// Computed selectors
export const selectFilteredAlunos = createSelector(
  selectAllAlunos,
  selectFilters,
  (alunos, filters) => {
    let filtered = alunos;

    if (filters.searchTerm) {
      const searchTerm = filters.searchTerm.toLowerCase();
      filtered = filtered.filter(aluno => 
        aluno.nome.toLowerCase().includes(searchTerm) ||
        aluno.cpf.includes(searchTerm) ||
        aluno.email?.toLowerCase().includes(searchTerm)
      );
    }

    if (filters.ativo !== undefined) {
      filtered = filtered.filter(aluno => aluno.ativo === filters.ativo);
    }

    return filtered;
  }
);

export const selectAlunosCount = createSelector(
  selectAllAlunos,
  (alunos) => alunos.length
);

export const selectActiveAlunosCount = createSelector(
  selectAllAlunos,
  (alunos) => alunos.filter(aluno => aluno.ativo).length
);

export const selectInactiveAlunosCount = createSelector(
  selectAllAlunos,
  (alunos) => alunos.filter(aluno => !aluno.ativo).length
);

export const selectAlunosWithActiveEnrollment = createSelector(
  selectAllAlunos,
  (alunos) => alunos.filter(aluno => aluno.possuiMatriculaAtiva)
);

// Dashboard selectors for responsáveis
export const selectAlunosByResponsavel = (_responsavelId: string) => createSelector(
  selectAllAlunos,
  (_alunos) => 
    // This would need to be implemented based on the actual relationship structure
    // For now, returning empty array as placeholder
    []
);

export const selectAlunosStats = createSelector(
  selectAllAlunos,
  (alunos) => ({
    total: alunos.length,
    ativos: alunos.filter(a => a.ativo).length,
    inativos: alunos.filter(a => !a.ativo).length,
    comMatriculaAtiva: alunos.filter(a => a.possuiMatriculaAtiva).length,
    semMatriculaAtiva: alunos.filter(a => !a.possuiMatriculaAtiva).length,
    comDeficiencia: alunos.filter(a => a.possuiDeficiencia).length
  })
);