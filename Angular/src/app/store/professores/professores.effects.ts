import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, catchError, switchMap, tap } from 'rxjs/operators';
import { Router } from '@angular/router';

import { ProfessoresService } from '../../features/professores/services/professores.service';
import * as ProfessoresActions from './professores.actions';

@Injectable()
export class ProfessoresEffects {
  private actions$ = inject(Actions);
  private professoresService = inject(ProfessoresService);
  private router = inject(Router);

  constructor() {}

  // Load Professores Effect
  loadProfessores$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProfessoresActions.loadProfessores),
      switchMap(({ filtros }) =>
        this.professoresService.obterProfessores(filtros).pipe(
          map(result => ProfessoresActions.loadProfessoresSuccess({ result })),
          catchError(error => of(ProfessoresActions.loadProfessoresFailure({ 
            error: error.message || 'Erro ao carregar professores' 
          })))
        )
      )
    )
  );

  // Load Professor Detail Effect
  loadProfessorDetail$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProfessoresActions.loadProfessorDetail),
      switchMap(({ id }) =>
        this.professoresService.obterProfessorDetalhe(id).pipe(
          map(professor => ProfessoresActions.loadProfessorDetailSuccess({ professor })),
          catchError(error => of(ProfessoresActions.loadProfessorDetailFailure({ 
            error: error.message || 'Erro ao carregar detalhes do professor' 
          })))
        )
      )
    )
  );

  // Create Professor Effect
  createProfessor$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProfessoresActions.createProfessor),
      switchMap(({ request }) =>
        this.professoresService.criarProfessor(request).pipe(
          map(response => ProfessoresActions.createProfessorSuccess({ 
            professorId: response.professorId 
          })),
          catchError(error => of(ProfessoresActions.createProfessorFailure({ 
            error: error.message || 'Erro ao criar professor' 
          })))
        )
      )
    )
  );

  // Create Professor Success Navigation
  createProfessorSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProfessoresActions.createProfessorSuccess),
      tap(({ professorId }) => {
        this.router.navigate(['/professores', professorId]);
      })
    ),
    { dispatch: false }
  );

  // Update Professor Effect
  updateProfessor$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProfessoresActions.updateProfessor),
      switchMap(({ request }) =>
        this.professoresService.atualizarProfessor(request).pipe(
          map(() => ProfessoresActions.updateProfessorSuccess()),
          catchError(error => of(ProfessoresActions.updateProfessorFailure({ 
            error: error.message || 'Erro ao atualizar professor' 
          })))
        )
      )
    )
  );

  // Update Professor Success Navigation
  updateProfessorSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProfessoresActions.updateProfessorSuccess),
      tap(() => {
        this.router.navigate(['/professores']);
      })
    ),
    { dispatch: false }
  );

  // Deactivate Professor Effect
  deactivateProfessor$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProfessoresActions.deactivateProfessor),
      switchMap(({ id }) =>
        this.professoresService.desativarProfessor(id).pipe(
          map(() => ProfessoresActions.deactivateProfessorSuccess({ id })),
          catchError(error => of(ProfessoresActions.deactivateProfessorFailure({ 
            error: error.message || 'Erro ao desativar professor' 
          })))
        )
      )
    )
  );

  // Assign Discipline Effect
  assignDiscipline$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProfessoresActions.assignDiscipline),
      switchMap(({ professorId, request }) =>
        this.professoresService.atribuirDisciplina(professorId, request).pipe(
          map(() => ProfessoresActions.assignDisciplineSuccess()),
          catchError(error => of(ProfessoresActions.assignDisciplineFailure({ 
            error: error.message || 'Erro ao atribuir disciplina' 
          })))
        )
      )
    )
  );

  // Remove Discipline Effect
  removeDiscipline$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProfessoresActions.removeDiscipline),
      switchMap(({ professorId, disciplinaId }) =>
        this.professoresService.removerDisciplina(professorId, disciplinaId).pipe(
          map(() => ProfessoresActions.removeDisciplineSuccess()),
          catchError(error => of(ProfessoresActions.removeDisciplineFailure({ 
            error: error.message || 'Erro ao remover disciplina' 
          })))
        )
      )
    )
  );

  // Search Professores Effect
  searchProfessores$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProfessoresActions.searchProfessores),
      switchMap(({ nome, escolaId, filtros }) =>
        this.professoresService.buscarProfessores(nome, escolaId, filtros).pipe(
          map(result => ProfessoresActions.searchProfessoresSuccess({ result })),
          catchError(error => of(ProfessoresActions.searchProfessoresFailure({ 
            error: error.message || 'Erro ao buscar professores' 
          })))
        )
      )
    )
  );

  // Load Statistics Effect
  loadStatistics$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProfessoresActions.loadStatistics),
      switchMap(({ escolaId }) =>
        this.professoresService.obterEstatisticas(escolaId).pipe(
          map(statistics => ProfessoresActions.loadStatisticsSuccess({ statistics })),
          catchError(error => of(ProfessoresActions.loadStatisticsFailure({ 
            error: error.message || 'Erro ao carregar estatísticas' 
          })))
        )
      )
    )
  );

  // Load Options Effect
  loadOptions$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProfessoresActions.loadOptions),
      switchMap(() =>
        this.professoresService.obterOpcoes().pipe(
          map(options => ProfessoresActions.loadOptionsSuccess({ options })),
          catchError(error => of(ProfessoresActions.loadOptionsFailure({ 
            error: error.message || 'Erro ao carregar opções' 
          })))
        )
      )
    )
  );
}