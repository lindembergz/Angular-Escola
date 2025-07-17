export interface Escola {
  id: string;
  nome: string;
  cnpj: string;
  endereco: Endereco;
  tipo: string;
  redeEscolarId?: string;
  nomeRedeEscolar?: string;
  ativa: boolean;
  dataCriacao: Date;
  unidades?: UnidadeEscolar[];
}

export interface RedeEscolar {
  id: string;
  nome: string;
  cnpjMantenedora: string;
  enderecoSede: Endereco;
  escolas: Escola[];
  ativa: boolean;
  dataCriacao: Date;
  totalEscolas: number;
  escolasAtivas: number;
  escolasInativas: number;
}

export interface UnidadeEscolar {
  id: string;
  nome: string;
  endereco: Endereco;
  tipo: string;
  capacidadeMaximaAlunos: number;
  alunosMatriculados: number;
  ativa: boolean;
  dataCriacao: Date;
  idadeMinima?: number;
  idadeMaxima?: number;
  temBerçario?: boolean;
  temPreEscola?: boolean;
  seriesAtendidas: string[];
  vagasDisponiveis: number;
  percentualOcupacao: number;
  podeReceberNovasMatriculas: boolean;
}

export interface Endereco {
  logradouro: string;
  numero: string;
  complemento?: string;
  bairro: string;
  cidade: string;
  estado: string;
  cep: string;
}

export enum TipoEscola {
  INFANTIL = 'Infantil',
  FUNDAMENTAL = 'Fundamental',
  MEDIO = 'Médio',
  FUNDAMENTAL_E_MEDIO = 'Fundamental e Médio',
  TECNICO = 'Técnico',
  EJA = 'EJA',
  SUPERIOR = 'Superior',
  POS_GRADUACAO = 'Pós-Graduação'
}

export interface CreateEscolaRequest {
  nome: string;
  cnpj: string;
  endereco: Endereco;
  tipo: string;
  redeEscolarId?: string;
}

export interface UpdateEscolaRequest {
  id: string;
  nome?: string;
  endereco?: Endereco;
  tipo?: string;
  ativa?: boolean;
}

export interface CreateRedeEscolarRequest {
  nome: string;
  cnpjMantenedora: string;
  enderecoSede: Endereco;
}

export interface CreateUnidadeEscolarRequest {
  escolaId: string;
  nome: string;
  endereco: Endereco;
  tipo: string;
  capacidadeMaximaAlunos: number;
  idadeMinima?: number;
  idadeMaxima?: number;
  temBerçario?: boolean;
  temPreEscola?: boolean;
  seriesAtendidas: string[];
}

export interface EscolasFilter {
  nome?: string;
  tipoEscola?: string;
  redeEscolarId?: string;
  ativa?: boolean;
  page?: number;
  pageSize?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}