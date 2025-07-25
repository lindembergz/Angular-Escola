import { createAction, props } from '@ngrx/store';
import {
  Turma,
  CriarTurmaRequest,
  AtualizarTurmaRequest,
  MatricularAlunoRequest,
  ObterTurmasResponse,
  TurmaEstatisticas,
  TurmaComVagas,
  OpcoesTurma
} from '../../features/academico/models/turma.model';
import {
  Disciplina,
  CriarDisciplinaRequest,
  AtualizarDisciplinaRequest,
  ObterDisciplinasResponse,
  DisciplinaEstatisticas,
  OpcoesDisciplina
} from '../../features/academico/models/disciplina.model';
import {
  CriarHorarioRequest,
  AtualizarHorarioRequest,
  GradeHoraria,
  HorarioProfessor,
  ConflitosHorario,
  HorarioEstatisticas,
  OpcoesHorario,
  VerificarConflitosRequest
} from '../../features/academico/models/horario.model';

// Turmas Actions
export const loadTurmas = createAction(
  '[Academico] Load Turmas',
  props<{ 
    page?: number; 
    pageSize?: number; 
    escolaId?: string;
    anoLetivo?: number;
    serie?: string;
    turno?: string;
    ativa?: boolean;
  }>()
);

export const loadTurmasSuccess = createAction(
  '[Academico] Load Turmas Success',
  props<{ response: ObterTurmasResponse }>()
);

export const loadTurmasFailure = createAction(
  '[Academico] Load Turmas Failure',
  props<{ error: string }>()
);

export const loadTurma = createAction(
  '[Academico] Load Turma',
  props<{ id: string }>()
);

export const loadTurmaSuccess = createAction(
  '[Academico] Load Turma Success',
  props<{ turma: Turma }>()
);

export const loadTurmaFailure = createAction(
  '[Academico] Load Turma Failure',
  props<{ error: string }>()
);

export const createTurma = createAction(
  '[Academico] Create Turma',
  props<{ request: CriarTurmaRequest }>()
);

export const createTurmaSuccess = createAction(
  '[Academico] Create Turma Success',
  props<{ id: string }>()
);

export const createTurmaFailure = createAction(
  '[Academico] Create Turma Failure',
  props<{ error: string }>()
);

export const updateTurma = createAction(
  '[Academico] Update Turma',
  props<{ request: AtualizarTurmaRequest }>()
);

export const updateTurmaSuccess = createAction(
  '[Academico] Update Turma Success'
);

export const updateTurmaFailure = createAction(
  '[Academico] Update Turma Failure',
  props<{ error: string }>()
);

export const matricularAluno = createAction(
  '[Academico] Matricular Aluno',
  props<{ request: MatricularAlunoRequest }>()
);

export const matricularAlunoSuccess = createAction(
  '[Academico] Matricular Aluno Success'
);

export const matricularAlunoFailure = createAction(
  '[Academico] Matricular Aluno Failure',
  props<{ error: string }>()
);

export const inativarTurma = createAction(
  '[Academico] Inativar Turma',
  props<{ id: string }>()
);

export const inativarTurmaSuccess = createAction(
  '[Academico] Inativar Turma Success',
  props<{ id: string }>()
);

export const inativarTurmaFailure = createAction(
  '[Academico] Inativar Turma Failure',
  props<{ error: string }>()
);

export const reativarTurma = createAction(
  '[Academico] Reativar Turma',
  props<{ id: string }>()
);

export const reativarTurmaSuccess = createAction(
  '[Academico] Reativar Turma Success',
  props<{ id: string }>()
);

export const reativarTurmaFailure = createAction(
  '[Academico] Reativar Turma Failure',
  props<{ error: string }>()
);

export const loadTurmasComVagas = createAction(
  '[Academico] Load Turmas Com Vagas',
  props<{ unidadeEscolarId: string }>()
);

export const loadTurmasComVagasSuccess = createAction(
  '[Academico] Load Turmas Com Vagas Success',
  props<{ turmas: TurmaComVagas[] }>()
);

export const loadTurmasComVagasFailure = createAction(
  '[Academico] Load Turmas Com Vagas Failure',
  props<{ error: string }>()
);

export const loadTurmaEstatisticas = createAction(
  '[Academico] Load Turma Estatisticas',
  props<{ unidadeEscolarId: string }>()
);

export const loadTurmaEstatisticasSuccess = createAction(
  '[Academico] Load Turma Estatisticas Success',
  props<{ estatisticas: TurmaEstatisticas }>()
);

export const loadTurmaEstatisticasFailure = createAction(
  '[Academico] Load Turma Estatisticas Failure',
  props<{ error: string }>()
);

export const loadOpcoesTurma = createAction('[Academico] Load Opcoes Turma');

export const loadOpcoesTurmaSuccess = createAction(
  '[Academico] Load Opcoes Turma Success',
  props<{ opcoes: OpcoesTurma }>()
);

export const loadOpcoesTurmaFailure = createAction(
  '[Academico] Load Opcoes Turma Failure',
  props<{ error: string }>()
);

// Disciplinas Actions
export const loadDisciplinas = createAction(
  '[Academico] Load Disciplinas',
  props<{ 
    page?: number; 
    pageSize?: number; 
    escolaId?: string;
    serie?: string;
    obrigatoria?: boolean;
    ativa?: boolean;
    searchTerm?: string;
  }>()
);

export const loadDisciplinasSuccess = createAction(
  '[Academico] Load Disciplinas Success',
  props<{ response: ObterDisciplinasResponse }>()
);

export const loadDisciplinasFailure = createAction(
  '[Academico] Load Disciplinas Failure',
  props<{ error: string }>()
);

export const loadDisciplina = createAction(
  '[Academico] Load Disciplina',
  props<{ id: string }>()
);

export const loadDisciplinaSuccess = createAction(
  '[Academico] Load Disciplina Success',
  props<{ disciplina: Disciplina }>()
);

export const loadDisciplinaFailure = createAction(
  '[Academico] Load Disciplina Failure',
  props<{ error: string }>()
);

export const createDisciplina = createAction(
  '[Academico] Create Disciplina',
  props<{ request: CriarDisciplinaRequest }>()
);

export const createDisciplinaSuccess = createAction(
  '[Academico] Create Disciplina Success',
  props<{ id: string }>()
);

export const createDisciplinaFailure = createAction(
  '[Academico] Create Disciplina Failure',
  props<{ error: string }>()
);

export const updateDisciplina = createAction(
  '[Academico] Update Disciplina',
  props<{ request: AtualizarDisciplinaRequest }>()
);

export const updateDisciplinaSuccess = createAction(
  '[Academico] Update Disciplina Success'
);

export const updateDisciplinaFailure = createAction(
  '[Academico] Update Disciplina Failure',
  props<{ error: string }>()
);

export const inativarDisciplina = createAction(
  '[Academico] Inativar Disciplina',
  props<{ id: string }>()
);

export const inativarDisciplinaSuccess = createAction(
  '[Academico] Inativar Disciplina Success',
  props<{ id: string }>()
);

export const inativarDisciplinaFailure = createAction(
  '[Academico] Inativar Disciplina Failure',
  props<{ error: string }>()
);

export const reativarDisciplina = createAction(
  '[Academico] Reativar Disciplina',
  props<{ id: string }>()
);

export const reativarDisciplinaSuccess = createAction(
  '[Academico] Reativar Disciplina Success',
  props<{ id: string }>()
);

export const reativarDisciplinaFailure = createAction(
  '[Academico] Reativar Disciplina Failure',
  props<{ error: string }>()
);

export const loadDisciplinaEstatisticas = createAction(
  '[Academico] Load Disciplina Estatisticas',
  props<{ escolaId: string }>()
);

export const loadDisciplinaEstatisticasSuccess = createAction(
  '[Academico] Load Disciplina Estatisticas Success',
  props<{ estatisticas: DisciplinaEstatisticas }>()
);

export const loadDisciplinaEstatisticasFailure = createAction(
  '[Academico] Load Disciplina Estatisticas Failure',
  props<{ error: string }>()
);

export const loadOpcoesDisciplina = createAction('[Academico] Load Opcoes Disciplina');

export const loadOpcoesDisciplinaSuccess = createAction(
  '[Academico] Load Opcoes Disciplina Success',
  props<{ opcoes: OpcoesDisciplina }>()
);

export const loadOpcoesDisciplinaFailure = createAction(
  '[Academico] Load Opcoes Disciplina Failure',
  props<{ error: string }>()
);

// Horários Actions
export const loadGradeHoraria = createAction(
  '[Academico] Load Grade Horaria',
  props<{ turmaId: string; anoLetivo: number; semestre: number }>()
);

export const loadGradeHorariaSuccess = createAction(
  '[Academico] Load Grade Horaria Success',
  props<{ gradeHoraria: GradeHoraria }>()
);

export const loadGradeHorariaFailure = createAction(
  '[Academico] Load Grade Horaria Failure',
  props<{ error: string }>()
);

export const loadHorariosProfessor = createAction(
  '[Academico] Load Horarios Professor',
  props<{ professorId: string; anoLetivo: number; semestre: number }>()
);

export const loadHorariosProfessorSuccess = createAction(
  '[Academico] Load Horarios Professor Success',
  props<{ horarios: HorarioProfessor }>()
);

export const loadHorariosProfessorFailure = createAction(
  '[Academico] Load Horarios Professor Failure',
  props<{ error: string }>()
);

export const createHorario = createAction(
  '[Academico] Create Horario',
  props<{ request: CriarHorarioRequest }>()
);

export const createHorarioSuccess = createAction(
  '[Academico] Create Horario Success',
  props<{ id: string }>()
);

export const createHorarioFailure = createAction(
  '[Academico] Create Horario Failure',
  props<{ error: string }>()
);

export const updateHorario = createAction(
  '[Academico] Update Horario',
  props<{ request: AtualizarHorarioRequest }>()
);

export const updateHorarioSuccess = createAction(
  '[Academico] Update Horario Success'
);

export const updateHorarioFailure = createAction(
  '[Academico] Update Horario Failure',
  props<{ error: string }>()
);

export const verificarConflitos = createAction(
  '[Academico] Verificar Conflitos',
  props<{ request: VerificarConflitosRequest }>()
);

export const verificarConflitosSuccess = createAction(
  '[Academico] Verificar Conflitos Success',
  props<{ conflitos: ConflitosHorario }>()
);

export const verificarConflitosFailure = createAction(
  '[Academico] Verificar Conflitos Failure',
  props<{ error: string }>()
);

export const cancelarHorario = createAction(
  '[Academico] Cancelar Horario',
  props<{ id: string }>()
);

export const cancelarHorarioSuccess = createAction(
  '[Academico] Cancelar Horario Success',
  props<{ id: string }>()
);

export const cancelarHorarioFailure = createAction(
  '[Academico] Cancelar Horario Failure',
  props<{ error: string }>()
);

export const reativarHorario = createAction(
  '[Academico] Reativar Horario',
  props<{ id: string }>()
);

export const reativarHorarioSuccess = createAction(
  '[Academico] Reativar Horario Success',
  props<{ id: string }>()
);

export const reativarHorarioFailure = createAction(
  '[Academico] Reativar Horario Failure',
  props<{ error: string }>()
);

export const loadHorarioEstatisticas = createAction(
  '[Academico] Load Horario Estatisticas',
  props<{ escolaId: string; anoLetivo: number; semestre: number }>()
);

export const loadHorarioEstatisticasSuccess = createAction(
  '[Academico] Load Horario Estatisticas Success',
  props<{ estatisticas: HorarioEstatisticas }>()
);

export const loadHorarioEstatisticasFailure = createAction(
  '[Academico] Load Horario Estatisticas Failure',
  props<{ error: string }>()
);

export const loadOpcoesHorario = createAction('[Academico] Load Opcoes Horario');

export const loadOpcoesHorarioSuccess = createAction(
  '[Academico] Load Opcoes Horario Success',
  props<{ opcoes: OpcoesHorario }>()
);

export const loadOpcoesHorarioFailure = createAction(
  '[Academico] Load Opcoes Horario Failure',
  props<{ error: string }>()
);

// UI Actions
export const setSelectedTurma = createAction(
  '[Academico] Set Selected Turma',
  props<{ turma: Turma | null }>()
);

export const setSelectedDisciplina = createAction(
  '[Academico] Set Selected Disciplina',
  props<{ disciplina: Disciplina | null }>()
);

export const setSelectedGradeHoraria = createAction(
  '[Academico] Set Selected Grade Horaria',
  props<{ gradeHoraria: GradeHoraria | null }>()
);

export const clearSelectedItems = createAction('[Academico] Clear Selected Items');

export const setLoading = createAction(
  '[Academico] Set Loading',
  props<{ loading: boolean }>()
);

export const clearErrors = createAction('[Academico] Clear Errors');

// Filter Actions
export const setTurmaFilters = createAction(
  '[Academico] Set Turma Filters',
  props<{ 
    escolaId?: string;
    anoLetivo?: number;
    serie?: string;
    turno?: string;
    ativa?: boolean;
  }>()
);

export const setDisciplinaFilters = createAction(
  '[Academico] Set Disciplina Filters',
  props<{ 
    escolaId?: string;
    serie?: string;
    obrigatoria?: boolean;
    ativa?: boolean;
    searchTerm?: string;
  }>()
);

export const clearFilters = createAction('[Academico] Clear Filters');

// Pagination Actions
export const setPagination = createAction(
  '[Academico] Set Pagination',
  props<{ page: number; pageSize: number }>()
);