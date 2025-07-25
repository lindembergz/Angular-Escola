import { createReducer, on } from '@ngrx/store';
import {
  Turma,
  TurmaEstatisticas,
  TurmaComVagas,
  OpcoesTurma
} from '../../features/academico/models/turma.model';
import {
  Disciplina,
  DisciplinaEstatisticas,
  OpcoesDisciplina
} from '../../features/academico/models/disciplina.model';
import {
  GradeHoraria,
  HorarioProfessor,
  ConflitosHorario,
  HorarioEstatisticas,
  OpcoesHorario
} from '../../features/academico/models/horario.model';
import * as AcademicoActions from './academico.actions';

export interface AcademicoState {
  // Turmas
  turmas: Turma[];
  selectedTurma: Turma | null;
  turmasComVagas: TurmaComVagas[];
  turmaEstatisticas: TurmaEstatisticas | null;
  opcoesTurma: OpcoesTurma | null;
  
  // Disciplinas
  disciplinas: Disciplina[];
  selectedDisciplina: Disciplina | null;
  disciplinaEstatisticas: DisciplinaEstatisticas | null;
  opcoesDisciplina: OpcoesDisciplina | null;
  
  // Horários
  gradeHoraria: GradeHoraria | null;
  selectedGradeHoraria: GradeHoraria | null;
  horariosProfessor: HorarioProfessor | null;
  conflitosHorario: ConflitosHorario | null;
  horarioEstatisticas: HorarioEstatisticas | null;
  opcoesHorario: OpcoesHorario | null;
  
  // Pagination
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  
  // Filters
  turmaFilters: {
    escolaId?: string;
    anoLetivo?: number;
    serie?: string;
    turno?: string;
    ativa?: boolean;
  };
  disciplinaFilters: {
    escolaId?: string;
    serie?: string;
    obrigatoria?: boolean;
    ativa?: boolean;
    searchTerm?: string;
  };
  
  // UI State
  loading: boolean;
  error: string | null;
}

export const initialState: AcademicoState = {
  // Turmas
  turmas: [],
  selectedTurma: null,
  turmasComVagas: [],
  turmaEstatisticas: null,
  opcoesTurma: null,
  
  // Disciplinas
  disciplinas: [],
  selectedDisciplina: null,
  disciplinaEstatisticas: null,
  opcoesDisciplina: null,
  
  // Horários
  gradeHoraria: null,
  selectedGradeHoraria: null,
  horariosProfessor: null,
  conflitosHorario: null,
  horarioEstatisticas: null,
  opcoesHorario: null,
  
  // Pagination
  totalCount: 0,
  page: 1,
  pageSize: 10,
  totalPages: 0,
  
  // Filters
  turmaFilters: {},
  disciplinaFilters: {},
  
  // UI State
  loading: false,
  error: null
};

export const academicoReducer = createReducer(
  initialState,
  
  // Turmas Reducers
  on(AcademicoActions.loadTurmas, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  
  on(AcademicoActions.loadTurmasSuccess, (state, { response }) => ({
    ...state,
    turmas: response.turmas,
    totalCount: response.totalCount,
    page: response.page,
    pageSize: response.pageSize,
    totalPages: response.totalPages,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.loadTurmasFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),
  
  on(AcademicoActions.loadTurmaSuccess, (state, { turma }) => ({
    ...state,
    selectedTurma: turma,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.createTurmaSuccess, (state) => ({
    ...state,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.updateTurmaSuccess, (state) => ({
    ...state,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.inativarTurmaSuccess, (state, { id }) => ({
    ...state,
    turmas: state.turmas.map(turma => 
      turma.id === id ? { ...turma, ativa: false } : turma
    ),
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.reativarTurmaSuccess, (state, { id }) => ({
    ...state,
    turmas: state.turmas.map(turma => 
      turma.id === id ? { ...turma, ativa: true } : turma
    ),
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.loadTurmasComVagasSuccess, (state, { turmas }) => ({
    ...state,
    turmasComVagas: turmas,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.loadTurmaEstatisticasSuccess, (state, { estatisticas }) => ({
    ...state,
    turmaEstatisticas: estatisticas,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.loadOpcoesTurmaSuccess, (state, { opcoes }) => ({
    ...state,
    opcoesTurma: opcoes,
    loading: false,
    error: null
  })),
  
  // Disciplinas Reducers
  on(AcademicoActions.loadDisciplinas, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  
  on(AcademicoActions.loadDisciplinasSuccess, (state, { response }) => ({
    ...state,
    disciplinas: response.disciplinas,
    totalCount: response.totalCount,
    page: response.page,
    pageSize: response.pageSize,
    totalPages: response.totalPages,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.loadDisciplinasFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),
  
  on(AcademicoActions.loadDisciplinaSuccess, (state, { disciplina }) => ({
    ...state,
    selectedDisciplina: disciplina,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.createDisciplinaSuccess, (state) => ({
    ...state,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.updateDisciplinaSuccess, (state) => ({
    ...state,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.inativarDisciplinaSuccess, (state, { id }) => ({
    ...state,
    disciplinas: state.disciplinas.map(disciplina => 
      disciplina.id === id ? { ...disciplina, ativa: false } : disciplina
    ),
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.reativarDisciplinaSuccess, (state, { id }) => ({
    ...state,
    disciplinas: state.disciplinas.map(disciplina => 
      disciplina.id === id ? { ...disciplina, ativa: true } : disciplina
    ),
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.loadDisciplinaEstatisticasSuccess, (state, { estatisticas }) => ({
    ...state,
    disciplinaEstatisticas: estatisticas,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.loadOpcoesDisciplinaSuccess, (state, { opcoes }) => ({
    ...state,
    opcoesDisciplina: opcoes,
    loading: false,
    error: null
  })),
  
  // Horários Reducers
  on(AcademicoActions.loadGradeHoraria, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  
  on(AcademicoActions.loadGradeHorariaSuccess, (state, { gradeHoraria }) => ({
    ...state,
    gradeHoraria,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.loadGradeHorariaFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),
  
  on(AcademicoActions.loadHorariosProfessorSuccess, (state, { horarios }) => ({
    ...state,
    horariosProfessor: horarios,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.createHorarioSuccess, (state) => ({
    ...state,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.updateHorarioSuccess, (state) => ({
    ...state,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.verificarConflitosSuccess, (state, { conflitos }) => ({
    ...state,
    conflitosHorario: conflitos,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.loadHorarioEstatisticasSuccess, (state, { estatisticas }) => ({
    ...state,
    horarioEstatisticas: estatisticas,
    loading: false,
    error: null
  })),
  
  on(AcademicoActions.loadOpcoesHorarioSuccess, (state, { opcoes }) => ({
    ...state,
    opcoesHorario: opcoes,
    loading: false,
    error: null
  })),
  
  // UI Actions
  on(AcademicoActions.setSelectedTurma, (state, { turma }) => ({
    ...state,
    selectedTurma: turma
  })),
  
  on(AcademicoActions.setSelectedDisciplina, (state, { disciplina }) => ({
    ...state,
    selectedDisciplina: disciplina
  })),
  
  on(AcademicoActions.setSelectedGradeHoraria, (state, { gradeHoraria }) => ({
    ...state,
    selectedGradeHoraria: gradeHoraria
  })),
  
  on(AcademicoActions.clearSelectedItems, (state) => ({
    ...state,
    selectedTurma: null,
    selectedDisciplina: null,
    selectedGradeHoraria: null
  })),
  
  on(AcademicoActions.setLoading, (state, { loading }) => ({
    ...state,
    loading
  })),
  
  on(AcademicoActions.clearErrors, (state) => ({
    ...state,
    error: null
  })),
  
  // Filter Actions
  on(AcademicoActions.setTurmaFilters, (state, filters) => ({
    ...state,
    turmaFilters: { ...state.turmaFilters, ...filters }
  })),
  
  on(AcademicoActions.setDisciplinaFilters, (state, filters) => ({
    ...state,
    disciplinaFilters: { ...state.disciplinaFilters, ...filters }
  })),
  
  on(AcademicoActions.clearFilters, (state) => ({
    ...state,
    turmaFilters: {},
    disciplinaFilters: {}
  })),
  
  // Pagination Actions
  on(AcademicoActions.setPagination, (state, { page, pageSize }) => ({
    ...state,
    page,
    pageSize
  })),
  
  // Error handling for all failure actions
  on(
    AcademicoActions.loadTurmaFailure,
    AcademicoActions.createTurmaFailure,
    AcademicoActions.updateTurmaFailure,
    AcademicoActions.matricularAlunoFailure,
    AcademicoActions.inativarTurmaFailure,
    AcademicoActions.reativarTurmaFailure,
    AcademicoActions.loadTurmasComVagasFailure,
    AcademicoActions.loadTurmaEstatisticasFailure,
    AcademicoActions.loadOpcoesTurmaFailure,
    AcademicoActions.loadDisciplinaFailure,
    AcademicoActions.createDisciplinaFailure,
    AcademicoActions.updateDisciplinaFailure,
    AcademicoActions.inativarDisciplinaFailure,
    AcademicoActions.reativarDisciplinaFailure,
    AcademicoActions.loadDisciplinaEstatisticasFailure,
    AcademicoActions.loadOpcoesDisciplinaFailure,
    AcademicoActions.loadHorariosProfessorFailure,
    AcademicoActions.createHorarioFailure,
    AcademicoActions.updateHorarioFailure,
    AcademicoActions.verificarConflitosFailure,
    AcademicoActions.cancelarHorarioFailure,
    AcademicoActions.reativarHorarioFailure,
    AcademicoActions.loadHorarioEstatisticasFailure,
    AcademicoActions.loadOpcoesHorarioFailure,
    (state, { error }) => ({
      ...state,
      loading: false,
      error
    })
  )
);