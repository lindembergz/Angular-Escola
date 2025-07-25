import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ApiService } from '../../../core/services/api.service';
import {
  ProfessorResumo,
  Professor,
  CriarProfessorRequest,
  CriarProfessorResponse,
  AtualizarProfessorRequest,
  AtualizarProfessorResponse,
  AtribuirDisciplinaRequest,
  DisciplinaInfo,
  OpcoesProfessor,
  EstatisticasProfessor,
  FiltrosProfessor,
  PaginatedResult,
  TipoTitulo
} from '../models/professor.model';

@Injectable({
  providedIn: 'root'
})
export class ProfessoresService {

  private readonly endpoint = 'professores';

  constructor(private apiService: ApiService) { }

  // Queries (Read operations)
  obterProfessores(filtros: FiltrosProfessor): Observable<PaginatedResult<ProfessorResumo>> {
    const params: any = {
      pagina: filtros.pagina,
      tamanhoPagina: filtros.tamanhoPagina
    };

    if (filtros.escolaId) params.escolaId = filtros.escolaId;
    if (filtros.ativo !== undefined) params.ativo = filtros.ativo;
    if (filtros.nome) params.nome = filtros.nome;

    return this.apiService.get<PaginatedResult<ProfessorResumo>>(this.endpoint, params)
      .pipe(
        map(response => response.data || response as any)
      );
  }

  obterProfessorDetalhe(id: string): Observable<Professor> {
    return this.apiService.getById<Professor>(this.endpoint, id)
      .pipe(
        map(response => response.data || response as any)
      );
  }

  obterProfessoresPorEscola(escolaId: string, filtros: Partial<FiltrosProfessor> = {}): Observable<PaginatedResult<ProfessorResumo>> {
    const params: any = {
      pagina: filtros.pagina || 1,
      tamanhoPagina: filtros.tamanhoPagina || 10
    };

    if (filtros.ativo !== undefined) params.ativo = filtros.ativo;
    if (filtros.nome) params.nome = filtros.nome;

    return this.apiService.get<PaginatedResult<ProfessorResumo>>(`${this.endpoint}/escola/${escolaId}`, params)
      .pipe(
        map(response => response.data || response as any)
      );
  }

  obterProfessoresAtivos(escolaId?: string, filtros: Partial<FiltrosProfessor> = {}): Observable<PaginatedResult<ProfessorResumo>> {
    const params: any = {
      pagina: filtros.pagina || 1,
      tamanhoPagina: filtros.tamanhoPagina || 10
    };

    if (escolaId) params.escolaId = escolaId;
    if (filtros.nome) params.nome = filtros.nome;

    return this.apiService.get<PaginatedResult<ProfessorResumo>>(`${this.endpoint}/ativos`, params)
      .pipe(
        map(response => response.data || response as any)
      );
  }

  buscarProfessores(nome: string, escolaId?: string, filtros: Partial<FiltrosProfessor> = {}): Observable<PaginatedResult<ProfessorResumo>> {
    const params: any = {
      nome,
      pagina: filtros.pagina || 1,
      tamanhoPagina: filtros.tamanhoPagina || 10
    };

    if (escolaId) params.escolaId = escolaId;

    return this.apiService.get<PaginatedResult<ProfessorResumo>>(`${this.endpoint}/buscar`, params)
      .pipe(
        map(response => response.data || response as any)
      );
  }

  obterEstatisticas(escolaId: string): Observable<EstatisticasProfessor> {
    return this.apiService.get<EstatisticasProfessor>(`${this.endpoint}/escola/${escolaId}/estatisticas`)
      .pipe(
        map(response => response.data || response as any)
      );
  }

  obterDisciplinasDisponiveis(escolaId: string): Observable<DisciplinaInfo[]> {
    return this.apiService.get<DisciplinaInfo[]>(`${this.endpoint}/disciplinas-disponiveis/${escolaId}`)
      .pipe(
        map(response => response.data || response as any)
      );
  }

  // Commands (Write operations)
  criarProfessor(request: CriarProfessorRequest): Observable<CriarProfessorResponse> {
    return this.apiService.post<CriarProfessorResponse>(this.endpoint, request)
      .pipe(
        map(response => response.data || response as any)
      );
  }

  atualizarProfessor(request: AtualizarProfessorRequest): Observable<AtualizarProfessorResponse> {
    return this.apiService.put<AtualizarProfessorResponse>(this.endpoint, request.id, request)
      .pipe(
        map(response => response.data || response as any)
      );
  }

  desativarProfessor(id: string): Observable<void> {
    return this.apiService.post<void>(`${this.endpoint}/${id}/desativar`, { id })
      .pipe(
        map(() => void 0)
      );
  }

  atribuirDisciplina(professorId: string, request: AtribuirDisciplinaRequest): Observable<void> {
    return this.apiService.post<void>(`${this.endpoint}/${professorId}/disciplinas`, request)
      .pipe(
        map(() => void 0)
      );
  }

  removerDisciplina(professorId: string, disciplinaId: string): Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${professorId}/disciplinas`, disciplinaId)
      .pipe(
        map(() => void 0)
      );
  }

  // Options and utilities
  obterOpcoes(): Observable<OpcoesProfessor> {
    return this.apiService.get<OpcoesProfessor>(`${this.endpoint}/opcoes`)
      .pipe(
        map(response => response.data || response as any)
      );
  }

  // Validation utilities
  validarCPF(cpf: string): boolean {
    cpf = cpf.replace(/[^\d]/g, '');
    if (cpf.length !== 11) return false;
    if (/^(\d)\1{10}$/.test(cpf)) return false;

    let sum = 0;
    for (let i = 0; i < 9; i++) {
      sum += parseInt(cpf.charAt(i)) * (10 - i);
    }
    let remainder = (sum * 10) % 11;
    if (remainder === 10 || remainder === 11) remainder = 0;
    if (remainder !== parseInt(cpf.charAt(9))) return false;

    sum = 0;
    for (let i = 0; i < 10; i++) {
      sum += parseInt(cpf.charAt(i)) * (11 - i);
    }
    remainder = (sum * 10) % 11;
    if (remainder === 10 || remainder === 11) remainder = 0;
    if (remainder !== parseInt(cpf.charAt(10))) return false;

    return true;
  }

  formatarCPF(cpf: string): string {
    cpf = cpf.replace(/[^\d]/g, '');
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
  }

  calcularIdade(dataNascimento: Date): number {
    const hoje = new Date();
    const nascimento = new Date(dataNascimento);
    let idade = hoje.getFullYear() - nascimento.getFullYear();
    const mesAtual = hoje.getMonth();
    const mesNascimento = nascimento.getMonth();

    if (mesAtual < mesNascimento || (mesAtual === mesNascimento && hoje.getDate() < nascimento.getDate())) {
      idade--;
    }

    return idade;
  }

  calcularTempoServico(dataContratacao: Date): number {
    const hoje = new Date();
    const contratacao = new Date(dataContratacao);
    let anos = hoje.getFullYear() - contratacao.getFullYear();
    const mesAtual = hoje.getMonth();
    const mesContratacao = contratacao.getMonth();

    if (mesAtual < mesContratacao || (mesAtual === mesContratacao && hoje.getDate() < contratacao.getDate())) {
      anos--;
    }

    return anos;
  }

  obterDescricaoTipoTitulo(tipo: number): string {
    switch (tipo) {
      case TipoTitulo.EnsinoMedio: return 'Ensino Médio';
      case TipoTitulo.Tecnologo: return 'Tecnólogo';
      case TipoTitulo.Graduacao: return 'Graduação';
      case TipoTitulo.PosGraduacao: return 'Pós-Graduação';
      case TipoTitulo.Mestrado: return 'Mestrado';
      case TipoTitulo.Doutorado: return 'Doutorado';
      case TipoTitulo.PosDoutorado: return 'Pós-Doutorado';
      default: return 'Não informado';
    }
  }
}