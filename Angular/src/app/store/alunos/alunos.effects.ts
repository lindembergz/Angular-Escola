import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, catchError, switchMap, tap } from 'rxjs/operators';
import { Router } from '@angular/router';

import * as AlunosActions from './alunos.actions';
import { AlunosService } from '../../features/alunos/services/alunos.service';
import { NotificationService } from '../../core/services/notification.service';

@Injectable()
export class AlunosEffects {
  private actions$ = inject(Actions);
  private alunosService = inject(AlunosService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);

  loadAlunos$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AlunosActions.loadAlunos),
      switchMap(({ page = 1, pageSize = 20, searchTerm, escolaId, turmaId, ativo }) => {
        console.log('🔄 Loading alunos with params:', { page, pageSize, searchTerm, escolaId, turmaId, ativo });
        return this.alunosService.obterAlunos({
          page,
          pageSize,
          searchTerm,
          escolaId,
          turmaId,
          ativo
        }).pipe(
          map(response => {
            console.log('✅ Alunos API response:', response);
            return AlunosActions.loadAlunosSuccess({ response });
          }),
          catchError(error => {
            console.error('❌ Error loading alunos:', error);
            this.notificationService.showError('Erro ao carregar alunos', error.message);
            return of(AlunosActions.loadAlunosFailure({ error: error.message }));
          })
        );
      })
    )
  );

  loadAluno$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AlunosActions.loadAluno),
      switchMap(({ id }) =>
        this.alunosService.obterAlunoDetalhe(id).pipe(
          map(response => {
            if (response.sucesso && response.aluno) {
              return AlunosActions.loadAlunoSuccess({ aluno: response.aluno });
            } else {
              console.log(response)
              throw new Error(response.erros?.join(', ') || 'Erro ao carregar aluno');
            }
          }),
          catchError(error => {
            console.log("Log 2:" + error.message)
            this.notificationService.showError('Erro ao carregar aluno', error.message);
            return of(AlunosActions.loadAlunoFailure({ error: error.message }));
          })
        )
      )
    )
  );

  createAluno$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AlunosActions.createAluno),
      switchMap(({ request }) =>
        this.alunosService.criarAluno(request).pipe(
          map(response => {
            if (response.sucesso) {
              this.notificationService.showSuccess(
                'Aluno criado com sucesso',
                `${response.nome} foi cadastrado com sucesso`
              );
              return AlunosActions.createAlunoSuccess({ response });
            } else {
              throw new Error(response.erros?.join(', ') || 'Erro ao criar aluno');
            }
          }),
          catchError(error => {
            this.notificationService.showError('Erro ao criar aluno', error.message);
            return of(AlunosActions.createAlunoFailure({ error: error.message }));
          })
        )
      )
    )
  );

  createAlunoSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AlunosActions.createAlunoSuccess),
      tap(({ response }) => {
        this.router.navigate(['/alunos', response.id]);
      })
    ),
    { dispatch: false }
  );

  updateAluno$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AlunosActions.updateAluno),
      switchMap(({ request }) =>
        this.alunosService.atualizarAluno(request).pipe(
          map(response => {
            if (response.sucesso) {
              this.notificationService.showSuccess(
                'Aluno atualizado com sucesso',
                `${response.nome} foi atualizado com sucesso`
              );
              return AlunosActions.updateAlunoSuccess({ response });
            } else {
              throw new Error(response.erros?.join(', ') || 'Erro ao atualizar aluno');
            }
          }),
          catchError(error => {
            this.notificationService.showError('Erro ao atualizar aluno', error.message);
            return of(AlunosActions.updateAlunoFailure({ error: error.message }));
          })
        )
      )
    )
  );

  updateAlunoSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AlunosActions.updateAlunoSuccess),
      tap(({ response }) => {
        this.router.navigate(['/alunos', response.id]);
      })
    ),
    { dispatch: false }
  );

  deleteAluno$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AlunosActions.deleteAluno),
      switchMap(({ id }) =>
        this.alunosService.excluirAluno(id).pipe(
          map(() => {
            this.notificationService.showSuccess(
              'Aluno excluído com sucesso',
              'O aluno foi removido do sistema'
            );
            return AlunosActions.deleteAlunoSuccess({ id });
          }),
          catchError(error => {
            this.notificationService.showError('Erro ao excluir aluno', error.message);
            return of(AlunosActions.deleteAlunoFailure({ error: error.message }));
          })
        )
      )
    )
  );

  loadOpcoesAluno$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AlunosActions.loadOpcoesAluno),
      switchMap(() =>
        this.alunosService.obterOpcoes().pipe(
          map(opcoes => AlunosActions.loadOpcoesAlunoSuccess({ opcoes })),
          catchError(error => {
            this.notificationService.showError('Erro ao carregar opções', error.message);
            return of(AlunosActions.loadOpcoesAlunoFailure({ error: error.message }));
          })
        )
      )
    )
  );

  matricularAluno$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AlunosActions.matricularAluno),
      switchMap(({ alunoId, request }) =>
        this.alunosService.matricularAluno(alunoId, request).pipe(
          map(response => {
            if (response.sucesso) {
              this.notificationService.showSuccess(
                'Matrícula realizada com sucesso',
                `${response.nomeAluno} foi matriculado com sucesso. Número da matrícula: ${response.numeroMatricula}`
              );
              return AlunosActions.matricularAlunoSuccess({ response });
            } else {
              throw new Error(response.erros?.join(', ') || 'Erro ao matricular aluno');
            }
          }),
          catchError(error => {
            this.notificationService.showError('Erro ao matricular aluno', error.message);
            return of(AlunosActions.matricularAlunoFailure({ error: error.message }));
          })
        )
      )
    )
  );

  matricularAlunoSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AlunosActions.matricularAlunoSuccess),
      switchMap(({ response }) => {
        // Reload aluno details after successful matriculation
        if (response.alunoId) {
          return of(AlunosActions.loadAluno({ id: response.alunoId }));
        }
        return of(); // Return empty observable if no alunoId
      })
    )
  );

  // Reload alunos after successful operations
  reloadAfterSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        AlunosActions.createAlunoSuccess,
        AlunosActions.updateAlunoSuccess,
        AlunosActions.deleteAlunoSuccess,
        AlunosActions.matricularAlunoSuccess
      ),
      map(() => AlunosActions.loadAlunos({}))
    )
  );
}