import { ActionReducerMap, MetaReducer } from '@ngrx/store';
import { environment } from '../../environments/environment';

// Import feature states
import { AuthState, authReducer } from './auth/auth.reducer';
import { AlunosState, alunosReducer } from './alunos/alunos.reducer';
import { ProfessoresState, professoresReducer } from './professores/professores.reducer';
import { AcademicoState, academicoReducer } from './academico/academico.reducer';

export interface AppState {
  auth: AuthState;
  alunos: AlunosState;
  professores: ProfessoresState;
  academico: AcademicoState;
  // Outros estados serão adicionados conforme necessário
}

export const reducers: ActionReducerMap<AppState> = {
  auth: authReducer,
  alunos: alunosReducer,
  professores: professoresReducer,
  academico: academicoReducer,
};

export const metaReducers: MetaReducer<AppState>[] = !environment.production ? [] : [];

// Selectors
export const selectAppState = (state: AppState) => state;