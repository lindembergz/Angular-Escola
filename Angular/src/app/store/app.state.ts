import { ActionReducerMap, MetaReducer } from '@ngrx/store';
import { environment } from '../../environments/environment';

// Import feature states
import { AuthState, authReducer } from './auth/auth.reducer';
import { EscolasState, escolasReducer } from '../features/escolas/store/escolas.reducer';

export interface AppState {
  auth: AuthState;
  escolas: EscolasState;
  // Outros estados serão adicionados conforme necessário
}

export const reducers: ActionReducerMap<AppState> = {
  auth: authReducer,
  escolas: escolasReducer,
};

export const metaReducers: MetaReducer<AppState>[] = !environment.production ? [] : [];

// Selectors
export const selectAppState = (state: AppState) => state;