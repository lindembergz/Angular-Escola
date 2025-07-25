export interface Turma {
  id: string;
  nome: string;
  serie: string;
  turno: string;
  capacidadeMaxima: number;
  anoLetivo: number;
  escolaId: string;
  ativa: boolean;
  alunosMatriculados: number;
  vagasDisponiveis: number;
  dataCriacao: Date;
}

export interface CriarTurmaRequest {
  nome: string;
  serie: string;
  turno: string;
  capacidadeMaxima: number;
  anoLetivo: number;
  escolaId: string;
}

export interface AtualizarTurmaRequest {
  id: string;
  nome: string;
  capacidadeMaxima: number;
}

export interface MatricularAlunoRequest {
  turmaId: string;
  alunoId: string;
}

export interface ObterTurmasRequest {
  page?: number;
  pageSize?: number;
  escolaId?: string;
  anoLetivo?: number;
  serie?: string;
  turno?: string;
  ativa?: boolean;
}

export interface ObterTurmasResponse {
  turmas: Turma[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface TurmaEstatisticas {
  totalTurmas: number;
  turmasAtivas: number;
  turmasInativas: number;
  totalAlunos: number;
  mediaAlunosPorTurma: number;
  ocupacaoMedia: number;
}

export interface TurmaComVagas {
  id: string;
  nome: string;
  serie: string;
  turno: string;
  vagasDisponiveis: number;
  capacidadeMaxima: number;
}

export interface OpcoesTurma {
  series: string[];
  turnos: string[];
  anosLetivos: number[];
}