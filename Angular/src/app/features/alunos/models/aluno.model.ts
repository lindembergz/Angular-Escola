export interface Aluno {
  id: string;
  nome: string;
  cpf: string;
  dataNascimento: Date;
  idade: number;
  faixaEtariaEscolar: string;
  endereco: Endereco;
  genero: Genero;
  deficiencia: Deficiencia;
  telefone?: string;
  email?: string;
  observacoes?: string;
  escolaId: string;
  nomeEscola: string;
  dataCadastro: Date;
  ativo: boolean;
  responsaveis: Responsavel[];
  matriculas: Matricula[];
  matriculaAtiva?: Matricula;
  pendencias: string[];
}

export interface AlunoResumo {
  id: string;
  nome: string;
  cpf: string;
  dataNascimento: Date;
  idade: number;
  generoDescricao: string;
  possuiDeficiencia: boolean;
  deficienciaDescricao?: string;
  telefone: string;
  email: string;
  cidade: string;
  estado: string;
  ativo: boolean;
  possuiMatriculaAtiva: boolean;
  nomeTurmaAtual?: string;
  quantidadeResponsaveis: number;
  dataCadastro: Date;
}

export interface Endereco {
  logradouro: string;
  numero: string;
  complemento?: string;
  bairro: string;
  cidade: string;
  estado: string;
  cep: string;
  cepFormatado: string;
  enderecoCompleto: string;
}

export interface Genero {
  tipo: TipoGenero;
  descricao: string;
}

export interface Deficiencia {
  tipo?: TipoDeficiencia;
  tipoDescricao?: string;
  descricao: string;
  possuiDeficiencia: boolean;
}

export interface Responsavel {
  id: string;
  nome: string;
  cpf: string;
  telefone: string;
  email?: string;
  tipoDescricao: string;
  profissao?: string;
  localTrabalho?: string;
  telefoneTrabalho?: string;
  responsavelFinanceiro: boolean;
  responsavelAcademico: boolean;
  autorizadoBuscar: boolean;
  endereco?: Endereco;
}

export interface Matricula {
  id: string;
  turmaId: string;
  nomeTurma: string;
  anoLetivo: number;
  numeroMatricula: string;
  dataMatricula: Date;
  dataCancelamento?: Date;
  motivoCancelamento?: string;
  ativa: boolean;
  statusDescricao: string;
  observacoes?: string;
  diasMatriculado: number;
}

export enum TipoGenero {
  NaoInformado = 0,
  Masculino = 1,
  Feminino = 2,
  NaoBinario = 3
}

export enum TipoDeficiencia {
  Visual = 1,
  Auditiva = 2,
  Fisica = 3,
  Intelectual = 4,
  Multipla = 5,
  Autismo = 6,
  Surdocegueira = 7
}

export enum TipoResponsavel {
  Pai = 1,
  Mae = 2,
  Responsavel = 3,
  Tutor = 4
}

// DTOs para criação e atualização
export interface CriarAlunoRequest {
  nome: string;
  cpf: string;
  dataNascimento: Date;
  logradouro: string;
  numero: string;
  complemento?: string;
  bairro: string;
  cidade: string;
  estado: string;
  cep: string;
  genero?: number; // TipoGenero
  tipoDeficiencia?: number; // TipoDeficiencia
  descricaoDeficiencia?: string;
  telefone?: string;
  email?: string;
  observacoes?: string;
  escolaId: string;
  responsaveis: CriarResponsavelRequest[];
}

export interface CriarResponsavelRequest {
  nome: string;
  cpf: string;
  telefone: string;
  email?: string;
  tipo: number; // TipoResponsavel
  profissao?: string;
  localTrabalho?: string;
  telefoneTrabalho?: string;
  responsavelFinanceiro: boolean;
  responsavelAcademico: boolean;
  autorizadoBuscar: boolean;
  observacoes?: string;
  logradouro?: string;
  numero?: string;
  complemento?: string;
  bairro?: string;
  cidade?: string;
  estado?: string;
  cep?: string;
}

export interface AtualizarAlunoRequest {
  id: string;
  nome: string;
  logradouro: string;
  numero: string;
  complemento?: string;
  bairro: string;
  cidade: string;
  estado: string;
  cep: string;
  genero?: number; // TipoGenero
  tipoDeficiencia?: number; // TipoDeficiencia
  descricaoDeficiencia?: string;
  telefone?: string;
  email?: string;
  observacoes?: string;
  responsaveis: CriarResponsavelRequest[];
}

// Opções para formulários
export interface OpcaoGenero {
  valor: number;
  descricao: string;
}

export interface OpcaoDeficiencia {
  valor: number;
  descricao: string;
}

export interface OpcoesAluno {
  generos: OpcaoGenero[];
  tiposDeficiencia: OpcaoDeficiencia[];
}

// Responses da API
export interface CriarAlunoResponse {
  id: string;
  nome: string;
  cpf: string;
  numeroMatricula: string;
  dataCadastro: Date;
  responsaveis: string[];
  sucesso: boolean;
  erros: string[];
}

export interface AtualizarAlunoResponse {
  id: string;
  nome: string;
  dataAtualizacao: Date;
  sucesso: boolean;
  erros: string[];
}

export interface MatricularAlunoResponse {
  matriculaId: string;
  alunoId: string;
  nomeAluno: string;
  turmaId: string;
  anoLetivo: number;
  numeroMatricula: string;
  dataMatricula: Date;
  sucesso: boolean;
  erros: string[];
}

export interface ObterAlunosResponse {
  alunos: AlunoResumo[];
  totalRegistros: number;
  totalPaginas: number;
  paginaAtual: number;
  tamanhoPagina: number;
  sucesso: boolean;
  erros: string[];
}

export interface MatricularAlunoRequest {
  turmaId: string;
  anoLetivo: number;
  observacoes?: string;
}

export interface ObterAlunoDetalheResponse {
  aluno?: Aluno;
  sucesso: boolean;
  erros: string[];
}