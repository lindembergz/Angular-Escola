import { ActionReducerMap, MetaReducer } from '@ngrx/store';
import { environment } from '../../environments/environment';

// Import feature states
import { AuthState, authReducer } from './auth/auth.reducer';

export interface AppState {
  auth: AuthState;
  // Outros estados serão adicionados conforme necessário
}

export const reducers: ActionReducerMap<AppState> = {
  auth: authReducer,
};

export const metaReducers: MetaReducer<AppState>[] = !environment.production ? [] : [];

// Selectors
export const selectAppState = (state: AppState) => state;