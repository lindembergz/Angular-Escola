import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, catchError, switchMap, tap } from 'rxjs/operators';
import { EscolasService } from '../services/escolas.service';
import * as EscolasActions from './escolas.actions';

@Injectable()
export class EscolasEffects {
  private actions$ = inject(Actions);
  private escolasService = inject(EscolasService);

  constructor() {}

  // Load Escolas Effect
  loadEscolas$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EscolasActions.loadEscolas),
      switchMap(({ filter }) =>
        this.escolasService.getEscolas(filter).pipe(
          map(escolas => EscolasActions.loadEscolasSuccess({ escolas })),
          catchError(error => of(EscolasActions.loadEscolasFailure({ 
            error: error.message || 'Erro ao carregar escolas' 
          })))
        )
      )
    )
  );

  // Load Escola by ID Effect
  loadEscolaById$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EscolasActions.loadEscolaById),
      switchMap(({ id }) =>
        this.escolasService.getEscolaById(id).pipe(
          map(escola => EscolasActions.loadEscolaByIdSuccess({ escola })),
          catchError(error => of(EscolasActions.loadEscolaByIdFailure({ 
            error: error.message || 'Erro ao carregar escola' 
          })))
        )
      )
    )
  );

  // Create Escola Effect
  createEscola$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EscolasActions.createEscola),
      switchMap(({ escola }) =>
        this.escolasService.createEscola(escola).pipe(
          map(createdEscola => EscolasActions.createEscolaSuccess({ escola: createdEscola })),
          catchError(error => of(EscolasActions.createEscolaFailure({ 
            error: error.message || 'Erro ao criar escola' 
          })))
        )
      )
    )
  );

  // Update Escola Effect
  updateEscola$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EscolasActions.updateEscola),
      switchMap(({ escola }) =>
        this.escolasService.updateEscola(escola).pipe(
          map(updatedEscola => EscolasActions.updateEscolaSuccess({ escola: updatedEscola })),
          catchError(error => of(EscolasActions.updateEscolaFailure({ 
            error: error.message || 'Erro ao atualizar escola' 
          })))
        )
      )
    )
  );

  // Delete Escola Effect
  deleteEscola$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EscolasActions.deleteEscola),
      switchMap(({ id }) =>
        this.escolasService.deleteEscola(id).pipe(
          map(() => EscolasActions.deleteEscolaSuccess({ id })),
          catchError(error => of(EscolasActions.deleteEscolaFailure({ 
            error: error.message || 'Erro ao excluir escola' 
          })))
        )
      )
    )
  );

  // Load Redes Escolares Effect
  loadRedesEscolares$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EscolasActions.loadRedesEscolares),
      switchMap(() =>
        this.escolasService.getRedesEscolares().pipe(
          map(redes => EscolasActions.loadRedesEscolaresSuccess({ redes })),
          catchError(error => of(EscolasActions.loadRedesEscolaresFailure({ 
            error: error.message || 'Erro ao carregar redes escolares' 
          })))
        )
      )
    )
  );

  // Create Rede Escolar Effect
  createRedeEscolar$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EscolasActions.createRedeEscolar),
      switchMap(({ rede }) =>
        this.escolasService.createRedeEscolar(rede).pipe(
          map(createdRede => EscolasActions.createRedeEscolarSuccess({ rede: createdRede })),
          catchError(error => of(EscolasActions.createRedeEscolarFailure({ 
            error: error.message || 'Erro ao criar rede escolar' 
          })))
        )
      )
    )
  );

  // Success Effects (for notifications, navigation, etc.)
  createEscolaSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EscolasActions.createEscolaSuccess),
      tap(({ escola }) => {
        console.log('Escola criada com sucesso:', escola.nome);
        // Aqui você pode adicionar notificações toast, navegação, etc.
      })
    ),
    { dispatch: false }
  );

  updateEscolaSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EscolasActions.updateEscolaSuccess),
      tap(({ escola }) => {
        console.log('Escola atualizada com sucesso:', escola.nome);
      })
    ),
    { dispatch: false }
  );

  deleteEscolaSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EscolasActions.deleteEscolaSuccess),
      tap(({ id }) => {
        console.log('Escola excluída com sucesso:', id);
      })
    ),
    { dispatch: false }
  );

  createRedeEscolarSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EscolasActions.createRedeEscolarSuccess),
      tap(({ rede }) => {
        console.log('Rede escolar criada com sucesso:', rede.nome);
      })
    ),
    { dispatch: false }
  );

  // Error Effects (for error handling, notifications, etc.)
  handleErrors$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        EscolasActions.loadEscolasFailure,
        EscolasActions.loadEscolaByIdFailure,
        EscolasActions.createEscolaFailure,
        EscolasActions.updateEscolaFailure,
        EscolasActions.deleteEscolaFailure,
        EscolasActions.loadRedesEscolaresFailure,
        EscolasActions.createRedeEscolarFailure
      ),
      tap(({ error }) => {
        console.error('Erro na operação:', error);
        // Aqui você pode adicionar notificações de erro, logging, etc.
      })
    ),
    { dispatch: false }
  );
}