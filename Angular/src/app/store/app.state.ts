import { ActionReducerMap, MetaReducer } from '@ngrx/store';
import { environment } from '../../environments/environment';

// Import feature states
import { AuthState, authReducer } from './auth/auth.reducer';
import { AlunosState, alunosReducer } from './alunos/alunos.reducer';

export interface AppState {
  auth: AuthState;
  alunos: AlunosState;
  // Outros estados serão adicionados conforme necessário
}

export const reducers: ActionReducerMap<AppState> = {
  auth: authReducer,
  alunos: alunosReducer,
};

export const metaReducers: MetaReducer<AppState>[] = !environment.production ? [] : [];

// Selectors
export const selectAppState = (state: AppState) => state;