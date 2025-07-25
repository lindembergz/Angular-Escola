import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ProfessoresState } from './professores.reducer';

export const selectProfessoresState = createFeatureSelector<ProfessoresState>('professores');

// Data Selectors
export const selectProfessores = createSelector(
  selectProfessoresState,
  (state: ProfessoresState) => state.professores
);

export const selectCurrentProfessor = createSelector(
  selectProfessoresState,
  (state: ProfessoresState) => state.currentProfessor
);

export const selectStatistics = createSelector(
  selectProfessoresState,
  (state: ProfessoresState) => state.statistics
);

export const selectOptions = createSelector(
  selectProfessoresState,
  (state: ProfessoresState) => state.options
);

// Pagination Selectors
export const selectPagination = createSelector(
  selectProfessoresState,
  (state: ProfessoresState) => ({
    totalItems: state.totalItems,
    currentPage: state.currentPage,
    pageSize: state.pageSize,
    totalPages: state.totalPages
  })
);

export const selectCurrentFilters = createSelector(
  selectProfessoresState,
  (state: ProfessoresState) => state.currentFilters
);

// UI State Selectors
export const selectLoading = createSelector(
  selectProfessoresState,
  (state: ProfessoresState) => state.loading
);

export const selectError = createSelector(
  selectProfessoresState,
  (state: ProfessoresState) => state.error
);

export const selectCreating = createSelector(
  selectProfessoresState,
  (state: ProfessoresState) => state.creating
);

export const selectUpdating = createSelector(
  selectProfessoresState,
  (state: ProfessoresState) => state.updating
);

export const selectDeleting = createSelector(
  selectProfessoresState,
  (state: ProfessoresState) => state.deleting
);

// Computed Selectors
export const selectProfessoresAtivos = createSelector(
  selectProfessores,
  (professores) => professores.filter(p => p.ativo)
);

export const selectProfessoresInativos = createSelector(
  selectProfessores,
  (professores) => professores.filter(p => !p.ativo)
);

export const selectProfessorById = (id: string) => createSelector(
  selectProfessores,
  (professores) => professores.find(p => p.id === id)
);

export const selectHasData = createSelector(
  selectProfessores,
  (professores) => professores.length > 0
);

export const selectIsEmpty = createSelector(
  selectProfessores,
  selectLoading,
  (professores, loading) => professores.length === 0 && !loading
);

// Combined Selectors
export const selectProfessoresWithPagination = createSelector(
  selectProfessores,
  selectPagination,
  selectLoading,
  selectError,
  (professores, pagination, loading, error) => ({
    professores,
    pagination,
    loading,
    error
  })
);

export const selectFormState = createSelector(
  selectCreating,
  selectUpdating,
  selectError,
  selectOptions,
  (creating, updating, error, options) => ({
    creating,
    updating,
    error,
    options,
    loading: creating || updating
  })
);