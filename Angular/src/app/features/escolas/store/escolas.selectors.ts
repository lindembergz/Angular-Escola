import { createFeatureSelector, createSelector } from '@ngrx/store';
import { EscolasState } from './escolas.reducer';
import { escolaAdapter, redeEscolarAdapter } from './escolas.reducer';

// Feature Selector
export const selectEscolasState = createFeatureSelector<EscolasState>('escolas');

// Escola Selectors
export const selectEscolaState = createSelector(
  selectEscolasState,
  (state) => state.escolas
);

export const {
  selectIds: selectEscolaIds,
  selectEntities: selectEscolaEntities,
  selectAll: selectAllEscolas,
  selectTotal: selectTotalEscolas,
} = escolaAdapter.getSelectors(selectEscolaState);

export const selectEscolasLoading = createSelector(
  selectEscolaState,
  (state) => state.loading
);

export const selectEscolasError = createSelector(
  selectEscolaState,
  (state) => state.error
);

export const selectSelectedEscolaId = createSelector(
  selectEscolaState,
  (state) => state.selectedEscolaId
);

export const selectSelectedEscola = createSelector(
  selectEscolaEntities,
  selectSelectedEscolaId,
  (entities, selectedId) => selectedId ? entities[selectedId] : null
);

export const selectEscolasTotalCount = createSelector(
  selectEscolaState,
  (state) => state.totalCount
);

export const selectEscolasCurrentPage = createSelector(
  selectEscolaState,
  (state) => state.currentPage
);

export const selectEscolasPageSize = createSelector(
  selectEscolaState,
  (state) => state.pageSize
);

export const selectEscolasFilter = createSelector(
  selectEscolaState,
  (state) => state.filter
);

export const selectEscolasPagination = createSelector(
  selectEscolasTotalCount,
  selectEscolasCurrentPage,
  selectEscolasPageSize,
  (totalCount, currentPage, pageSize) => ({
    totalCount,
    currentPage,
    pageSize,
    totalPages: Math.ceil(totalCount / pageSize)
  })
);

// Rede Escolar Selectors
export const selectRedeEscolarState = createSelector(
  selectEscolasState,
  (state) => state.redesEscolares
);

export const {
  selectIds: selectRedeEscolarIds,
  selectEntities: selectRedeEscolarEntities,
  selectAll: selectAllRedesEscolares,
  selectTotal: selectTotalRedesEscolares,
} = redeEscolarAdapter.getSelectors(selectRedeEscolarState);

export const selectRedesEscolaresLoading = createSelector(
  selectRedeEscolarState,
  (state) => state.loading
);

export const selectRedesEscolaresError = createSelector(
  selectRedeEscolarState,
  (state) => state.error
);

// Combined Selectors
export const selectEscolasByRedeEscolar = createSelector(
  selectAllEscolas,
  (escolas: any[], props: { redeEscolarId: string }) => 
    escolas.filter((escola: any) => escola.redeEscolarId === props.redeEscolarId)
);

export const selectEscolasByTipo = createSelector(
  selectAllEscolas,
  (escolas: any[], props: { tipo: string }) => 
    escolas.filter((escola: any) => escola.tipo === props.tipo)
);

export const selectEscolasAtivas = createSelector(
  selectAllEscolas,
  (escolas) => escolas.filter(escola => escola.ativa)
);

export const selectEscolasInativas = createSelector(
  selectAllEscolas,
  (escolas) => escolas.filter(escola => !escola.ativa)
);