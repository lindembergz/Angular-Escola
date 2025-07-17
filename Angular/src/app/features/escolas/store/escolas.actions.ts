import { createAction, props } from '@ngrx/store';
import { 
  Escola, 
  RedeEscolar, 
  CreateEscolaRequest, 
  UpdateEscolaRequest, 
  CreateRedeEscolarRequest,
  EscolasFilter,
  PagedResult
} from '../models/escola.model';

// Load Escolas Actions
export const loadEscolas = createAction(
  '[Escolas] Load Escolas',
  props<{ filter?: EscolasFilter }>()
);

export const loadEscolasSuccess = createAction(
  '[Escolas] Load Escolas Success',
  props<{ escolas: PagedResult<Escola> }>()
);

export const loadEscolasFailure = createAction(
  '[Escolas] Load Escolas Failure',
  props<{ error: string }>()
);

// Load Escola by ID Actions
export const loadEscolaById = createAction(
  '[Escolas] Load Escola By ID',
  props<{ id: string }>()
);

export const loadEscolaByIdSuccess = createAction(
  '[Escolas] Load Escola By ID Success',
  props<{ escola: Escola }>()
);

export const loadEscolaByIdFailure = createAction(
  '[Escolas] Load Escola By ID Failure',
  props<{ error: string }>()
);

// Create Escola Actions
export const createEscola = createAction(
  '[Escolas] Create Escola',
  props<{ escola: CreateEscolaRequest }>()
);

export const createEscolaSuccess = createAction(
  '[Escolas] Create Escola Success',
  props<{ escola: Escola }>()
);

export const createEscolaFailure = createAction(
  '[Escolas] Create Escola Failure',
  props<{ error: string }>()
);

// Update Escola Actions
export const updateEscola = createAction(
  '[Escolas] Update Escola',
  props<{ escola: UpdateEscolaRequest }>()
);

export const updateEscolaSuccess = createAction(
  '[Escolas] Update Escola Success',
  props<{ escola: Escola }>()
);

export const updateEscolaFailure = createAction(
  '[Escolas] Update Escola Failure',
  props<{ error: string }>()
);

// Delete Escola Actions
export const deleteEscola = createAction(
  '[Escolas] Delete Escola',
  props<{ id: string }>()
);

export const deleteEscolaSuccess = createAction(
  '[Escolas] Delete Escola Success',
  props<{ id: string }>()
);

export const deleteEscolaFailure = createAction(
  '[Escolas] Delete Escola Failure',
  props<{ error: string }>()
);

// Load Redes Escolares Actions
export const loadRedesEscolares = createAction(
  '[Escolas] Load Redes Escolares'
);

export const loadRedesEscolaresSuccess = createAction(
  '[Escolas] Load Redes Escolares Success',
  props<{ redes: RedeEscolar[] }>()
);

export const loadRedesEscolaresFailure = createAction(
  '[Escolas] Load Redes Escolares Failure',
  props<{ error: string }>()
);

// Create Rede Escolar Actions
export const createRedeEscolar = createAction(
  '[Escolas] Create Rede Escolar',
  props<{ rede: CreateRedeEscolarRequest }>()
);

export const createRedeEscolarSuccess = createAction(
  '[Escolas] Create Rede Escolar Success',
  props<{ rede: RedeEscolar }>()
);

export const createRedeEscolarFailure = createAction(
  '[Escolas] Create Rede Escolar Failure',
  props<{ error: string }>()
);

// UI Actions
export const setSelectedEscola = createAction(
  '[Escolas] Set Selected Escola',
  props<{ escola: Escola | null }>()
);

export const setEscolasFilter = createAction(
  '[Escolas] Set Escolas Filter',
  props<{ filter: EscolasFilter }>()
);

export const clearEscolasError = createAction(
  '[Escolas] Clear Escolas Error'
);