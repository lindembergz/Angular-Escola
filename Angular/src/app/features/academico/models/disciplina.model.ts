export interface Disciplina {
  id: string;
  nome: string;
  codigo: string;
  cargaHoraria: number;
  serie: string;
  obrigatoria: boolean;
  descricao?: string;
  escolaId: string;
  ativa: boolean;
  preRequisitos: string[];
  dataCriacao: Date;
}

export interface CriarDisciplinaRequest {
  nome: string;
  codigo: string;
  cargaHoraria: number;
  serie: string;
  obrigatoria: boolean;
  escolaId: string;
  descricao?: string;
}

export interface AtualizarDisciplinaRequest {
  id: string;
  nome: string;
  cargaHoraria: number;
  obrigatoria: boolean;
  descricao?: string;
}

export interface ObterDisciplinasRequest {
  page?: number;
  pageSize?: number;
  escolaId?: string;
  serie?: string;
  obrigatoria?: boolean;
  ativa?: boolean;
  searchTerm?: string;
}

export interface ObterDisciplinasResponse {
  disciplinas: Disciplina[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface DisciplinaEstatisticas {
  totalDisciplinas: number;
  disciplinasAtivas: number;
  disciplinasInativas: number;
  disciplinasObrigatorias: number;
  disciplinasOptativas: number;
  mediaCargaHoraria: number;
}

export interface PreRequisito {
  disciplinaId: string;
  preRequisitoId: string;
}

export interface OpcoesDisciplina {
  series: string[];
  codigosSugeridos: string[];
}