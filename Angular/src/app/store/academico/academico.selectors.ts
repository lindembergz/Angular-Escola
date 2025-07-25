import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AcademicoState } from './academico.reducer';

export const selectAcademicoState = createFeatureSelector<AcademicoState>('academico');

// Turmas Selectors
export const selectTurmas = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.turmas
);

export const selectSelectedTurma = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.selectedTurma
);

export const selectTurmasComVagas = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.turmasComVagas
);

export const selectTurmaEstatisticas = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.turmaEstatisticas
);

export const selectOpcoesTurma = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.opcoesTurma
);

export const selectTurmaFilters = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.turmaFilters
);

// Disciplinas Selectors
export const selectDisciplinas = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.disciplinas
);

export const selectSelectedDisciplina = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.selectedDisciplina
);

export const selectDisciplinaEstatisticas = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.disciplinaEstatisticas
);

export const selectOpcoesDisciplina = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.opcoesDisciplina
);

export const selectDisciplinaFilters = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.disciplinaFilters
);

// Horários Selectors
export const selectGradeHoraria = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.gradeHoraria
);

export const selectSelectedGradeHoraria = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.selectedGradeHoraria
);

export const selectHorariosProfessor = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.horariosProfessor
);

export const selectConflitosHorario = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.conflitosHorario
);

export const selectHorarioEstatisticas = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.horarioEstatisticas
);

export const selectOpcoesHorario = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.opcoesHorario
);

// Pagination Selectors
export const selectPagination = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => ({
    totalCount: state.totalCount,
    page: state.page,
    pageSize: state.pageSize,
    totalPages: state.totalPages
  })
);

// UI State Selectors
export const selectLoading = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.loading
);

export const selectError = createSelector(
  selectAcademicoState,
  (state: AcademicoState) => state.error
);

// Combined Selectors
export const selectTurmasWithPagination = createSelector(
  selectTurmas,
  selectPagination,
  (turmas, pagination) => ({
    turmas,
    ...pagination
  })
);

export const selectDisciplinasWithPagination = createSelector(
  selectDisciplinas,
  selectPagination,
  (disciplinas, pagination) => ({
    disciplinas,
    ...pagination
  })
);

export const selectTurmasAtivas = createSelector(
  selectTurmas,
  (turmas) => turmas.filter(turma => turma.ativa)
);

export const selectDisciplinasAtivas = createSelector(
  selectDisciplinas,
  (disciplinas) => disciplinas.filter(disciplina => disciplina.ativa)
);

export const selectTurmasPorSerie = createSelector(
  selectTurmas,
  (turmas) => {
    const turmasPorSerie: { [key: string]: any[] } = {};
    turmas.forEach(turma => {
      if (!turmasPorSerie[turma.serie]) {
        turmasPorSerie[turma.serie] = [];
      }
      turmasPorSerie[turma.serie].push(turma);
    });
    return turmasPorSerie;
  }
);

export const selectDisciplinasPorSerie = createSelector(
  selectDisciplinas,
  (disciplinas) => {
    const disciplinasPorSerie: { [key: string]: any[] } = {};
    disciplinas.forEach(disciplina => {
      if (!disciplinasPorSerie[disciplina.serie]) {
        disciplinasPorSerie[disciplina.serie] = [];
      }
      disciplinasPorSerie[disciplina.serie].push(disciplina);
    });
    return disciplinasPorSerie;
  }
);

export const selectGradeHorariaFormatada = createSelector(
  selectGradeHoraria,
  (gradeHoraria) => {
    if (!gradeHoraria) return null;
    
    const diasSemana = ['SEGUNDA', 'TERCA', 'QUARTA', 'QUINTA', 'SEXTA', 'SABADO'];
    const horarios: { [key: string]: any[] } = {};
    
    diasSemana.forEach(dia => {
      horarios[dia] = gradeHoraria.horarios
        .filter(h => h.diaSemana === dia)
        .sort((a, b) => a.horaInicio.localeCompare(b.horaInicio));
    });
    
    return {
      ...gradeHoraria,
      horariosPorDia: horarios
    };
  }
);

export const selectHorariosConflitantes = createSelector(
  selectConflitosHorario,
  (conflitos) => {
    if (!conflitos || !conflitos.temConflito) return [];
    return conflitos.conflitos;
  }
);

export const selectTemConflitosHorario = createSelector(
  selectConflitosHorario,
  (conflitos) => conflitos?.temConflito || false
);