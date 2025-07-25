import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, catchError, switchMap, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';

import { TurmaService } from '../../features/academico/services/turma.service';
import { DisciplinaService } from '../../features/academico/services/disciplina.service';
import { HorarioService } from '../../features/academico/services/horario.service';
import * as AcademicoActions from './academico.actions';

@Injectable()
export class AcademicoEffects {

  constructor(
    private actions$: Actions,
    private turmaService: TurmaService,
    private disciplinaService: DisciplinaService,
    private horarioService: HorarioService,
    private router: Router,
    private messageService: MessageService
  ) {}

  // Turmas Effects
  loadTurmas$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.loadTurmas),
      switchMap((action) =>
        this.turmaService.obterTurmas({
          page: action.page,
          pageSize: action.pageSize,
          escolaId: action.escolaId,
          anoLetivo: action.anoLetivo,
          serie: action.serie,
          turno: action.turno,
          ativa: action.ativa
        }).pipe(
          map((response) => AcademicoActions.loadTurmasSuccess({ response })),
          catchError((error) => of(AcademicoActions.loadTurmasFailure({ 
            error: error.error?.message || 'Erro ao carregar turmas' 
          })))
        )
      )
    )
  );

  loadTurma$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.loadTurma),
      switchMap((action) =>
        this.turmaService.obterTurmaPorId(action.id).pipe(
          map((turma) => AcademicoActions.loadTurmaSuccess({ turma })),
          catchError((error) => of(AcademicoActions.loadTurmaFailure({ 
            error: error.error?.message || 'Erro ao carregar turma' 
          })))
        )
      )
    )
  );

  createTurma$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.createTurma),
      switchMap((action) =>
        this.turmaService.criarTurma(action.request).pipe(
          map((id) => AcademicoActions.createTurmaSuccess({ id })),
          catchError((error) => of(AcademicoActions.createTurmaFailure({ 
            error: error.error?.message || 'Erro ao criar turma' 
          })))
        )
      )
    )
  );

  createTurmaSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.createTurmaSuccess),
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Turma criada com sucesso!'
        });
        this.router.navigate(['/academico/turmas']);
      })
    ),
    { dispatch: false }
  );

  updateTurma$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.updateTurma),
      switchMap((action) =>
        this.turmaService.atualizarTurma(action.request).pipe(
          map(() => AcademicoActions.updateTurmaSuccess()),
          catchError((error) => of(AcademicoActions.updateTurmaFailure({ 
            error: error.error?.message || 'Erro ao atualizar turma' 
          })))
        )
      )
    )
  );

  updateTurmaSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.updateTurmaSuccess),
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Turma atualizada com sucesso!'
        });
      })
    ),
    { dispatch: false }
  );

  matricularAluno$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.matricularAluno),
      switchMap((action) =>
        this.turmaService.matricularAluno(action.request).pipe(
          map(() => AcademicoActions.matricularAlunoSuccess()),
          catchError((error) => of(AcademicoActions.matricularAlunoFailure({ 
            error: error.error?.message || 'Erro ao matricular aluno' 
          })))
        )
      )
    )
  );

  matricularAlunoSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.matricularAlunoSuccess),
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Aluno matriculado com sucesso!'
        });
      })
    ),
    { dispatch: false }
  );

  inativarTurma$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.inativarTurma),
      switchMap((action) =>
        this.turmaService.inativarTurma(action.id).pipe(
          map(() => AcademicoActions.inativarTurmaSuccess({ id: action.id })),
          catchError((error) => of(AcademicoActions.inativarTurmaFailure({ 
            error: error.error?.message || 'Erro ao inativar turma' 
          })))
        )
      )
    )
  );

  reativarTurma$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.reativarTurma),
      switchMap((action) =>
        this.turmaService.reativarTurma(action.id).pipe(
          map(() => AcademicoActions.reativarTurmaSuccess({ id: action.id })),
          catchError((error) => of(AcademicoActions.reativarTurmaFailure({ 
            error: error.error?.message || 'Erro ao reativar turma' 
          })))
        )
      )
    )
  );

  loadTurmasComVagas$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.loadTurmasComVagas),
      switchMap((action) =>
        this.turmaService.obterTurmasComVagas(action.unidadeEscolarId).pipe(
          map((turmas) => AcademicoActions.loadTurmasComVagasSuccess({ turmas })),
          catchError((error) => of(AcademicoActions.loadTurmasComVagasFailure({ 
            error: error.error?.message || 'Erro ao carregar turmas com vagas' 
          })))
        )
      )
    )
  );

  loadTurmaEstatisticas$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.loadTurmaEstatisticas),
      switchMap((action) =>
        this.turmaService.obterEstatisticas(action.unidadeEscolarId).pipe(
          map((estatisticas) => AcademicoActions.loadTurmaEstatisticasSuccess({ estatisticas })),
          catchError((error) => of(AcademicoActions.loadTurmaEstatisticasFailure({ 
            error: error.error?.message || 'Erro ao carregar estatísticas de turmas' 
          })))
        )
      )
    )
  );

  loadOpcoesTurma$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.loadOpcoesTurma),
      switchMap(() =>
        this.turmaService.obterOpcoes().pipe(
          map((opcoes) => AcademicoActions.loadOpcoesTurmaSuccess({ opcoes })),
          catchError((error) => of(AcademicoActions.loadOpcoesTurmaFailure({ 
            error: error.error?.message || 'Erro ao carregar opções de turma' 
          })))
        )
      )
    )
  );

  // Disciplinas Effects
  loadDisciplinas$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.loadDisciplinas),
      switchMap((action) =>
        this.disciplinaService.obterDisciplinas({
          page: action.page,
          pageSize: action.pageSize,
          escolaId: action.escolaId,
          serie: action.serie,
          obrigatoria: action.obrigatoria,
          ativa: action.ativa,
          searchTerm: action.searchTerm
        }).pipe(
          map((response) => AcademicoActions.loadDisciplinasSuccess({ response })),
          catchError((error) => of(AcademicoActions.loadDisciplinasFailure({ 
            error: error.error?.message || 'Erro ao carregar disciplinas' 
          })))
        )
      )
    )
  );

  loadDisciplina$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.loadDisciplina),
      switchMap((action) =>
        this.disciplinaService.obterDisciplinaPorId(action.id).pipe(
          map((disciplina) => AcademicoActions.loadDisciplinaSuccess({ disciplina })),
          catchError((error) => of(AcademicoActions.loadDisciplinaFailure({ 
            error: error.error?.message || 'Erro ao carregar disciplina' 
          })))
        )
      )
    )
  );

  createDisciplina$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.createDisciplina),
      switchMap((action) =>
        this.disciplinaService.criarDisciplina(action.request).pipe(
          map((id) => AcademicoActions.createDisciplinaSuccess({ id })),
          catchError((error) => of(AcademicoActions.createDisciplinaFailure({ 
            error: error.error?.message || 'Erro ao criar disciplina' 
          })))
        )
      )
    )
  );

  createDisciplinaSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.createDisciplinaSuccess),
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Disciplina criada com sucesso!'
        });
        this.router.navigate(['/academico/disciplinas']);
      })
    ),
    { dispatch: false }
  );

  updateDisciplina$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.updateDisciplina),
      switchMap((action) =>
        this.disciplinaService.atualizarDisciplina(action.request).pipe(
          map(() => AcademicoActions.updateDisciplinaSuccess()),
          catchError((error) => of(AcademicoActions.updateDisciplinaFailure({ 
            error: error.error?.message || 'Erro ao atualizar disciplina' 
          })))
        )
      )
    )
  );

  updateDisciplinaSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.updateDisciplinaSuccess),
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Disciplina atualizada com sucesso!'
        });
      })
    ),
    { dispatch: false }
  );

  inativarDisciplina$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.inativarDisciplina),
      switchMap((action) =>
        this.disciplinaService.inativarDisciplina(action.id).pipe(
          map(() => AcademicoActions.inativarDisciplinaSuccess({ id: action.id })),
          catchError((error) => of(AcademicoActions.inativarDisciplinaFailure({ 
            error: error.error?.message || 'Erro ao inativar disciplina' 
          })))
        )
      )
    )
  );

  reativarDisciplina$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.reativarDisciplina),
      switchMap((action) =>
        this.disciplinaService.reativarDisciplina(action.id).pipe(
          map(() => AcademicoActions.reativarDisciplinaSuccess({ id: action.id })),
          catchError((error) => of(AcademicoActions.reativarDisciplinaFailure({ 
            error: error.error?.message || 'Erro ao reativar disciplina' 
          })))
        )
      )
    )
  );

  loadDisciplinaEstatisticas$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.loadDisciplinaEstatisticas),
      switchMap((action) =>
        this.disciplinaService.obterEstatisticas(action.escolaId).pipe(
          map((estatisticas) => AcademicoActions.loadDisciplinaEstatisticasSuccess({ estatisticas })),
          catchError((error) => of(AcademicoActions.loadDisciplinaEstatisticasFailure({ 
            error: error.error?.message || 'Erro ao carregar estatísticas de disciplinas' 
          })))
        )
      )
    )
  );

  loadOpcoesDisciplina$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.loadOpcoesDisciplina),
      switchMap(() =>
        this.disciplinaService.obterOpcoes().pipe(
          map((opcoes) => AcademicoActions.loadOpcoesDisciplinaSuccess({ opcoes })),
          catchError((error) => of(AcademicoActions.loadOpcoesDisciplinaFailure({ 
            error: error.error?.message || 'Erro ao carregar opções de disciplina' 
          })))
        )
      )
    )
  );

  // Horários Effects
  loadGradeHoraria$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.loadGradeHoraria),
      switchMap((action) =>
        this.horarioService.obterGradeHorariaTurma(action.turmaId, action.anoLetivo, action.semestre).pipe(
          map((gradeHoraria) => AcademicoActions.loadGradeHorariaSuccess({ gradeHoraria })),
          catchError((error) => of(AcademicoActions.loadGradeHorariaFailure({ 
            error: error.error?.message || 'Erro ao carregar grade horária' 
          })))
        )
      )
    )
  );

  loadHorariosProfessor$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.loadHorariosProfessor),
      switchMap((action) =>
        this.horarioService.obterHorariosProfessor(action.professorId, action.anoLetivo, action.semestre).pipe(
          map((horarios) => AcademicoActions.loadHorariosProfessorSuccess({ horarios })),
          catchError((error) => of(AcademicoActions.loadHorariosProfessorFailure({ 
            error: error.error?.message || 'Erro ao carregar horários do professor' 
          })))
        )
      )
    )
  );

  createHorario$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.createHorario),
      switchMap((action) =>
        this.horarioService.criarHorario(action.request).pipe(
          map((id) => AcademicoActions.createHorarioSuccess({ id })),
          catchError((error) => of(AcademicoActions.createHorarioFailure({ 
            error: error.error?.message || 'Erro ao criar horário' 
          })))
        )
      )
    )
  );

  createHorarioSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.createHorarioSuccess),
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Horário criado com sucesso!'
        });
      })
    ),
    { dispatch: false }
  );

  updateHorario$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.updateHorario),
      switchMap((action) =>
        this.horarioService.atualizarHorario(action.request).pipe(
          map(() => AcademicoActions.updateHorarioSuccess()),
          catchError((error) => of(AcademicoActions.updateHorarioFailure({ 
            error: error.error?.message || 'Erro ao atualizar horário' 
          })))
        )
      )
    )
  );

  updateHorarioSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.updateHorarioSuccess),
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Horário atualizado com sucesso!'
        });
      })
    ),
    { dispatch: false }
  );

  verificarConflitos$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.verificarConflitos),
      switchMap((action) =>
        this.horarioService.verificarConflitos(action.request).pipe(
          map((conflitos) => AcademicoActions.verificarConflitosSuccess({ conflitos })),
          catchError((error) => of(AcademicoActions.verificarConflitosFailure({ 
            error: error.error?.message || 'Erro ao verificar conflitos' 
          })))
        )
      )
    )
  );

  cancelarHorario$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.cancelarHorario),
      switchMap((action) =>
        this.horarioService.cancelarHorario(action.id).pipe(
          map(() => AcademicoActions.cancelarHorarioSuccess({ id: action.id })),
          catchError((error) => of(AcademicoActions.cancelarHorarioFailure({ 
            error: error.error?.message || 'Erro ao cancelar horário' 
          })))
        )
      )
    )
  );

  reativarHorario$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.reativarHorario),
      switchMap((action) =>
        this.horarioService.reativarHorario(action.id).pipe(
          map(() => AcademicoActions.reativarHorarioSuccess({ id: action.id })),
          catchError((error) => of(AcademicoActions.reativarHorarioFailure({ 
            error: error.error?.message || 'Erro ao reativar horário' 
          })))
        )
      )
    )
  );

  loadHorarioEstatisticas$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.loadHorarioEstatisticas),
      switchMap((action) =>
        this.horarioService.obterEstatisticas(action.escolaId, action.anoLetivo, action.semestre).pipe(
          map((estatisticas) => AcademicoActions.loadHorarioEstatisticasSuccess({ estatisticas })),
          catchError((error) => of(AcademicoActions.loadHorarioEstatisticasFailure({ 
            error: error.error?.message || 'Erro ao carregar estatísticas de horários' 
          })))
        )
      )
    )
  );

  loadOpcoesHorario$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AcademicoActions.loadOpcoesHorario),
      switchMap(() =>
        this.horarioService.obterOpcoes().pipe(
          map((opcoes) => AcademicoActions.loadOpcoesHorarioSuccess({ opcoes })),
          catchError((error) => of(AcademicoActions.loadOpcoesHorarioFailure({ 
            error: error.error?.message || 'Erro ao carregar opções de horário' 
          })))
        )
      )
    )
  );

  // Error handling
  showErrorMessage$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        AcademicoActions.loadTurmasFailure,
        AcademicoActions.loadTurmaFailure,
        AcademicoActions.createTurmaFailure,
        AcademicoActions.updateTurmaFailure,
        AcademicoActions.matricularAlunoFailure,
        AcademicoActions.inativarTurmaFailure,
        AcademicoActions.reativarTurmaFailure,
        AcademicoActions.loadDisciplinasFailure,
        AcademicoActions.loadDisciplinaFailure,
        AcademicoActions.createDisciplinaFailure,
        AcademicoActions.updateDisciplinaFailure,
        AcademicoActions.inativarDisciplinaFailure,
        AcademicoActions.reativarDisciplinaFailure,
        AcademicoActions.loadGradeHorariaFailure,
        AcademicoActions.loadHorariosProfessorFailure,
        AcademicoActions.createHorarioFailure,
        AcademicoActions.updateHorarioFailure,
        AcademicoActions.verificarConflitosFailure,
        AcademicoActions.cancelarHorarioFailure,
        AcademicoActions.reativarHorarioFailure
      ),
      tap((action: any) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Erro',
          detail: action.error
        });
      })
    ),
    { dispatch: false }
  );
}