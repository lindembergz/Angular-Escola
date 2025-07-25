export interface Horario {
  id: string;
  turmaId: string;
  disciplinaId: string;
  professorId: string;
  diaSemana: string;
  horaInicio: string;
  horaFim: string;
  sala?: string;
  anoLetivo: number;
  semestre: number;
  ativo: boolean;
  dataCriacao: Date;
}

export interface CriarHorarioRequest {
  turmaId: string;
  disciplinaId: string;
  professorId: string;
  diaSemana: string;
  horaInicio: string;
  horaFim: string;
  anoLetivo: number;
  semestre: number;
  sala?: string;
}

export interface AtualizarHorarioRequest {
  id: string;
  professorId: string;
  sala?: string;
}

export interface GradeHoraria {
  turmaId: string;
  turmaNome: string;
  anoLetivo: number;
  semestre: number;
  horarios: HorarioGrade[];
}

export interface HorarioGrade {
  id: string;
  diaSemana: string;
  horaInicio: string;
  horaFim: string;
  disciplinaNome: string;
  professorNome: string;
  sala?: string;
  ativo: boolean;
}

export interface HorarioProfessor {
  professorId: string;
  professorNome: string;
  anoLetivo: number;
  semestre: number;
  horarios: HorarioDetalhe[];
}

export interface HorarioDetalhe {
  id: string;
  diaSemana: string;
  horaInicio: string;
  horaFim: string;
  turmaNome: string;
  disciplinaNome: string;
  sala?: string;
  ativo: boolean;
}

export interface ConflitosHorario {
  temConflito: boolean;
  conflitos: ConflitosDetalhes[];
}

export interface ConflitosDetalhes {
  tipo: 'PROFESSOR' | 'SALA' | 'TURMA';
  descricao: string;
  horarioConflitante: HorarioDetalhe;
}

export interface HorarioEstatisticas {
  totalHorarios: number;
  horariosAtivos: number;
  horariosCancelados: number;
  utilizacaoSalas: number;
  conflitosDetectados: number;
}

export interface OpcoesHorario {
  diasSemana: DiaSemana[];
  horariosDisponiveis: string[];
  semestres: number[];
  salas: string[];
}

export interface DiaSemana {
  valor: string;
  nome: string;
  abreviacao: string;
}

export interface VerificarConflitosRequest {
  turmaId?: string;
  professorId?: string;
  sala?: string;
  diaSemana: string;
  horaInicio: string;
  horaFim: string;
  anoLetivo: number;
  semestre: number;
  horarioExcluirId?: string;
}