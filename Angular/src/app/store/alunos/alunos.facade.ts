import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';

import { AppState } from '../app.state';
import * as AlunosActions from './alunos.actions';
import * as AlunosSelectors from './alunos.selectors';
import { 
  Aluno, 
  AlunoResumo, 
  CriarAlunoRequest, 
  AtualizarAlunoRequest,
  MatricularAlunoRequest
} from '../../features/alunos/models/aluno.model';

@Injectable({
  providedIn: 'root'
})
export class AlunosFacade {

  // Selectors
  readonly alunos$;
  readonly selectedAluno$;
  readonly loading$;
  readonly error$;
  readonly opcoes$;
  readonly paginationInfo$;
  readonly filters$;
  readonly filteredAlunos$;
  readonly alunosStats$;

  constructor(private store: Store<AppState>) {
    this.alunos$ = this.store.select(AlunosSelectors.selectAllAlunos);
    this.selectedAluno$ = this.store.select(AlunosSelectors.selectSelectedAluno);
    this.loading$ = this.store.select(AlunosSelectors.selectAlunosLoading);
    this.error$ = this.store.select(AlunosSelectors.selectAlunosError);
    this.opcoes$ = this.store.select(AlunosSelectors.selectOpcoesAluno);
    this.paginationInfo$ = this.store.select(AlunosSelectors.selectPaginationInfo);
    this.filters$ = this.store.select(AlunosSelectors.selectFilters);
    this.filteredAlunos$ = this.store.select(AlunosSelectors.selectFilteredAlunos);
    this.alunosStats$ = this.store.select(AlunosSelectors.selectAlunosStats);
  }

  // Actions
  loadAlunos(params: {
    page?: number;
    pageSize?: number;
    searchTerm?: string;
    escolaId?: string;
    turmaId?: string;
    ativo?: boolean;
  } = {}): void {
    this.store.dispatch(AlunosActions.loadAlunos(params));
  }

  loadAluno(id: string): void {
    this.store.dispatch(AlunosActions.loadAluno({ id }));
  }

  createAluno(request: CriarAlunoRequest): void {
    this.store.dispatch(AlunosActions.createAluno({ request }));
  }

  updateAluno(request: AtualizarAlunoRequest): void {
    this.store.dispatch(AlunosActions.updateAluno({ request }));
  }

  deleteAluno(id: string): void {
    this.store.dispatch(AlunosActions.deleteAluno({ id }));
  }

  matricularAluno(alunoId: string, request: MatricularAlunoRequest): void {
    this.store.dispatch(AlunosActions.matricularAluno({ alunoId, request }));
  }

  loadOpcoes(): void {
    this.store.dispatch(AlunosActions.loadOpcoesAluno());
  }

  setSelectedAluno(aluno: Aluno | null): void {
    this.store.dispatch(AlunosActions.setSelectedAluno({ aluno }));
  }

  clearSelectedAluno(): void {
    this.store.dispatch(AlunosActions.clearSelectedAluno());
  }

  setFilters(filters: {
    searchTerm?: string;
    escolaId?: string;
    turmaId?: string;
    ativo?: boolean;
  }): void {
    this.store.dispatch(AlunosActions.setFilters(filters));
  }

  clearFilters(): void {
    this.store.dispatch(AlunosActions.clearFilters());
  }

  setPagination(page: number, pageSize: number): void {
    this.store.dispatch(AlunosActions.setPagination({ page, pageSize }));
  }

  clearErrors(): void {
    this.store.dispatch(AlunosActions.clearErrors());
  }

  // Utility methods
  getAlunoById(id: string): Observable<AlunoResumo | undefined> {
    return this.store.select(AlunosSelectors.selectAlunoById(id));
  }

  searchAlunos(searchTerm: string): void {
    this.setFilters({ searchTerm });
    this.loadAlunos({ searchTerm });
  }

  filterByEscola(escolaId: string): void {
    this.setFilters({ escolaId });
    this.loadAlunos({ escolaId });
  }

  filterByTurma(turmaId: string): void {
    this.setFilters({ turmaId });
    this.loadAlunos({ turmaId });
  }

  filterByStatus(ativo: boolean): void {
    this.setFilters({ ativo });
    this.loadAlunos({ ativo });
  }

  changePage(page: number): void {
    this.setPagination(page, 20); // Default page size
    this.loadAlunos({ page });
  }

  changePageSize(pageSize: number): void {
    this.setPagination(1, pageSize); // Reset to first page
    this.loadAlunos({ page: 1, pageSize });
  }

  refreshAlunos(): void {
    this.loadAlunos();
  }
}