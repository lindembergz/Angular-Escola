import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Escola, RedeEscolar, EscolasFilter } from '../models/escola.model';
import * as EscolasActions from './escolas.actions';

// Escola Entity State
export interface EscolaEntityState extends EntityState<Escola> {
  selectedEscolaId: string | null;
  loading: boolean;
  error: string | null;
  totalCount: number;
  currentPage: number;
  pageSize: number;
  filter: EscolasFilter | null;
}

// Rede Escolar Entity State
export interface RedeEscolarEntityState extends EntityState<RedeEscolar> {
  loading: boolean;
  error: string | null;
}

// Combined Escolas State
export interface EscolasState {
  escolas: EscolaEntityState;
  redesEscolares: RedeEscolarEntityState;
}

// Entity Adapters
export const escolaAdapter: EntityAdapter<Escola> = createEntityAdapter<Escola>();
export const redeEscolarAdapter: EntityAdapter<RedeEscolar> = createEntityAdapter<RedeEscolar>();

// Initial States
export const initialEscolaState: EscolaEntityState = escolaAdapter.getInitialState({
  selectedEscolaId: null,
  loading: false,
  error: null,
  totalCount: 0,
  currentPage: 1,
  pageSize: 10,
  filter: null
});

export const initialRedeEscolarState: RedeEscolarEntityState = redeEscolarAdapter.getInitialState({
  loading: false,
  error: null
});

export const initialEscolasState: EscolasState = {
  escolas: initialEscolaState,
  redesEscolares: initialRedeEscolarState
};

// Escola Reducer
export const escolaReducer = createReducer(
  initialEscolaState,
  
  // Load Escolas
  on(EscolasActions.loadEscolas, (state, { filter }) => ({
    ...state,
    loading: true,
    error: null,
    filter: filter || null
  })),
  
  on(EscolasActions.loadEscolasSuccess, (state, { escolas }) => 
    escolaAdapter.setAll(escolas.items, {
      ...state,
      loading: false,
      error: null,
      totalCount: escolas.totalCount,
      currentPage: escolas.page,
      pageSize: escolas.pageSize
    })
  ),
  
  on(EscolasActions.loadEscolasFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Load Escola by ID
  on(EscolasActions.loadEscolaById, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  
  on(EscolasActions.loadEscolaByIdSuccess, (state, { escola }) => 
    escolaAdapter.upsertOne(escola, {
      ...state,
      loading: false,
      error: null,
      selectedEscolaId: escola.id
    })
  ),
  
  on(EscolasActions.loadEscolaByIdFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Create Escola
  on(EscolasActions.createEscola, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  
  on(EscolasActions.createEscolaSuccess, (state, { escola }) => 
    escolaAdapter.addOne(escola, {
      ...state,
      loading: false,
      error: null,
      totalCount: state.totalCount + 1
    })
  ),
  
  on(EscolasActions.createEscolaFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Update Escola
  on(EscolasActions.updateEscola, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  
  on(EscolasActions.updateEscolaSuccess, (state, { escola }) => 
    escolaAdapter.updateOne({ id: escola.id, changes: escola }, {
      ...state,
      loading: false,
      error: null
    })
  ),
  
  on(EscolasActions.updateEscolaFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Delete Escola
  on(EscolasActions.deleteEscola, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  
  on(EscolasActions.deleteEscolaSuccess, (state, { id }) => 
    escolaAdapter.removeOne(id, {
      ...state,
      loading: false,
      error: null,
      totalCount: state.totalCount - 1,
      selectedEscolaId: state.selectedEscolaId === id ? null : state.selectedEscolaId
    })
  ),
  
  on(EscolasActions.deleteEscolaFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // UI Actions
  on(EscolasActions.setSelectedEscola, (state, { escola }) => ({
    ...state,
    selectedEscolaId: escola?.id || null
  })),
  
  on(EscolasActions.setEscolasFilter, (state, { filter }) => ({
    ...state,
    filter
  })),
  
  on(EscolasActions.clearEscolasError, (state) => ({
    ...state,
    error: null
  }))
);

// Rede Escolar Reducer
export const redeEscolarReducer = createReducer(
  initialRedeEscolarState,
  
  // Load Redes Escolares
  on(EscolasActions.loadRedesEscolares, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  
  on(EscolasActions.loadRedesEscolaresSuccess, (state, { redes }) => 
    redeEscolarAdapter.setAll(redes, {
      ...state,
      loading: false,
      error: null
    })
  ),
  
  on(EscolasActions.loadRedesEscolaresFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Create Rede Escolar
  on(EscolasActions.createRedeEscolar, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  
  on(EscolasActions.createRedeEscolarSuccess, (state, { rede }) => 
    redeEscolarAdapter.addOne(rede, {
      ...state,
      loading: false,
      error: null
    })
  ),
  
  on(EscolasActions.createRedeEscolarFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  }))
);

// Combined Reducer
export const escolasReducer = createReducer(
  initialEscolasState,
  
  // Escola actions
  on(EscolasActions.loadEscolas, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.loadEscolasSuccess, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.loadEscolasFailure, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.loadEscolaById, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.loadEscolaByIdSuccess, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.loadEscolaByIdFailure, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.createEscola, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.createEscolaSuccess, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.createEscolaFailure, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.updateEscola, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.updateEscolaSuccess, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.updateEscolaFailure, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.deleteEscola, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.deleteEscolaSuccess, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.deleteEscolaFailure, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.setSelectedEscola, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.setEscolasFilter, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),
  
  on(EscolasActions.clearEscolasError, (state, action) => ({
    ...state,
    escolas: escolaReducer(state.escolas, action)
  })),

  // Rede Escolar actions
  on(EscolasActions.loadRedesEscolares, (state, action) => ({
    ...state,
    redesEscolares: redeEscolarReducer(state.redesEscolares, action)
  })),
  
  on(EscolasActions.loadRedesEscolaresSuccess, (state, action) => ({
    ...state,
    redesEscolares: redeEscolarReducer(state.redesEscolares, action)
  })),
  
  on(EscolasActions.loadRedesEscolaresFailure, (state, action) => ({
    ...state,
    redesEscolares: redeEscolarReducer(state.redesEscolares, action)
  })),
  
  on(EscolasActions.createRedeEscolar, (state, action) => ({
    ...state,
    redesEscolares: redeEscolarReducer(state.redesEscolares, action)
  })),
  
  on(EscolasActions.createRedeEscolarSuccess, (state, action) => ({
    ...state,
    redesEscolares: redeEscolarReducer(state.redesEscolares, action)
  })),
  
  on(EscolasActions.createRedeEscolarFailure, (state, action) => ({
    ...state,
    redesEscolares: redeEscolarReducer(state.redesEscolares, action)
  }))
);