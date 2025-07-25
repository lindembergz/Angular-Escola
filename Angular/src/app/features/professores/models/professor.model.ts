// Interfaces baseadas nos DTOs do backend
export interface ProfessorResumo {
  id: string;
  nome: string;
  cpf: string;
  registro: string;
  email?: string;
  escolaId: string;
  ativo: boolean;
  cargaHorariaTotal: number;
  maiorTitulo: string;
  quantidadeDisciplinas: number;
}

export interface Professor {
  id: string;
  nome: string;
  cpf: string;
  registro: string;
  email?: string;
  telefone?: string;
  dataNascimento: Date;
  dataContratacao: Date;
  escolaId: string;
  ativo: boolean;
  dataCadastro: Date;
  observacoes?: string;
  idade: number;
  tempoServico: number;
  cargaHorariaTotal: number;
  titulos: TituloAcademico[];
  disciplinas: DisciplinaProfessor[];
}

export interface TituloAcademico {
  tipo: string;
  curso: string;
  instituicao: string;
  anoFormatura: number;
  descricao: string;
}

export interface DisciplinaProfessor {
  id: string;
  disciplinaId: string;
  cargaHorariaSemanal: number;
  dataAtribuicao: Date;
  ativa: boolean;
  observacoes?: string;
  // Dados da disciplina obtidos via integração
  disciplina?: DisciplinaInfo;
}

export interface DisciplinaInfo {
  id: string;
  nome: string;
  codigo: string;
  cargaHoraria: number;
  serie: string;
  obrigatoria: boolean;
}

export interface PaginatedResult<T> {
  items: T[];
  totalItems: number;
  pagina: number;
  tamanhoPagina: number;
  totalPaginas: number;
}

// Request/Response interfaces
export interface CriarProfessorRequest {
  nome: string;
  cpf: string;
  registro: string;
  email?: string;
  telefone?: string;
  dataNascimento: Date;
  dataContratacao: Date;
  escolaId: string;
  observacoes?: string;
  titulos: CriarTituloRequest[];
}

export interface CriarTituloRequest {
  tipo: number;
  curso: string;
  instituicao: string;
  anoFormatura: number;
  descricao?: string;
}

export interface AtualizarProfessorRequest {
  id: string;
  nome: string;
  cpf: string;
  registro: string;
  email?: string;
  telefone?: string;
  dataNascimento: Date;
  dataContratacao: Date;
  observacoes?: string;
  titulos: CriarTituloRequest[];
}

export interface AtribuirDisciplinaRequest {
  disciplinaId: string;
  cargaHorariaSemanal: number;
  observacoes?: string;
}

export interface ObterProfessoresResponse {
  professores: ProfessorResumo[];
  totalItems: number;
  pagina: number;
  tamanhoPagina: number;
  totalPaginas: number;
}

export interface ObterProfessorDetalheResponse {
  professor: Professor;
  sucesso: boolean;
  erros: string[];
}

export interface CriarProfessorResponse {
  professorId: string;
  sucesso: boolean;
  erros: string[];
}

export interface AtualizarProfessorResponse {
  sucesso: boolean;
  erros: string[];
}

// Options interfaces
export interface OpcoesProfessor {
  tiposTitulo: OpcaoTitulo[];
}

export interface OpcaoTitulo {
  valor: number;
  descricao: string;
}

export enum TipoTitulo {
  EnsinoMedio = 1,
  Tecnologo = 2,
  Graduacao = 3,
  PosGraduacao = 4,
  Mestrado = 5,
  Doutorado = 6,
  PosDoutorado = 7
}

// Statistics interface
export interface EstatisticasProfessor {
  totalProfessores: number;
  professoresAtivos: number;
  professoresInativos: number;
  cargaHorariaMedia: number;
  professoresComTituloSuperior: number;
}

// Filter interface
export interface FiltrosProfessor {
  escolaId?: string;
  ativo?: boolean;
  nome?: string;
  pagina: number;
  tamanhoPagina: number;
}