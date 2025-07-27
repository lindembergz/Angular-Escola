import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ApiService } from '../../../core/services/api.service';
import {
  AlunoResumo,
  CriarAlunoRequest,
  CriarAlunoResponse,
  AtualizarAlunoRequest,
  AtualizarAlunoResponse,
  MatricularAlunoRequest,
  MatricularAlunoResponse,
  ObterAlunosResponse,
  ObterAlunoDetalheResponse,
  OpcoesAluno,
  OpcaoGenero,
  OpcaoDeficiencia,
  TipoGenero,
  TipoDeficiencia
} from '../models/aluno.model';

@Injectable({
  providedIn: 'root'
})
export class AlunosService {

  private readonly endpoint = 'alunos';

  constructor(private apiService: ApiService) { }

  // Queries (Read operations)
  obterAlunos(params: {
    page?: number;
    pageSize?: number;
    searchTerm?: string;
    escolaId?: string;
    turmaId?: string;
    ativo?: boolean;
  } = {}): Observable<ObterAlunosResponse> {
    return this.apiService.get<ObterAlunosResponse>(this.endpoint, params)
      .pipe(
        map(response => response.data || response as any)
      );
  }

  obterAlunoDetalhe(id: string): Observable<ObterAlunoDetalheResponse> {
    return this.apiService.getById<ObterAlunoDetalheResponse>(this.endpoint, id)
      .pipe(
        map(response => {
          // A API pode retornar os dados diretamente ou dentro de response.data
          const data = response.data || response as any;
          return {
            aluno: data.aluno,
            sucesso: data.sucesso,
            erros: data.erros || []
          };
        })
      );
  }

  obterAlunosPorTurma(turmaId: string): Observable<AlunoResumo[]> {
    return this.apiService.get<AlunoResumo[]>(`${this.endpoint}/turma/${turmaId}`)
      .pipe(
        map(response => response.data)
      );
  }

  obterAlunosPorResponsavel(responsavelId: string): Observable<AlunoResumo[]> {
    return this.apiService.get<AlunoResumo[]>(`${this.endpoint}/responsavel/${responsavelId}`)
      .pipe(
        map(response => response.data)
      );
  }

  // Commands (Write operations)
  criarAluno(request: CriarAlunoRequest): Observable<CriarAlunoResponse> {
    return this.apiService.post<CriarAlunoResponse>(this.endpoint, request)
      .pipe(
        map(response => {
          // A API pode retornar os dados diretamente ou dentro de response.data
          return response.data || response as any;
        })
      );
  }

  atualizarAluno(request: AtualizarAlunoRequest): Observable<AtualizarAlunoResponse> {
    return this.apiService.put<AtualizarAlunoResponse>(this.endpoint, request.id, request)
      .pipe(
        map(response => {
          // A API pode retornar os dados diretamente ou dentro de response.data
          return response.data || response as any;
        })
      );
  }

  excluirAluno(id: string): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id)
      .pipe(
        map(() => void 0)
      );
  }

  transferirAluno(alunoId: string, novaTurmaId: string, observacoes?: string): Observable<void> {
    return this.apiService.post<void>(`${this.endpoint}/${alunoId}/transferir`, {
      novaTurmaId,
      observacoes
    }).pipe(
      map(() => void 0)
    );
  }

  matricularAluno(alunoId: string, request: MatricularAlunoRequest): Observable<MatricularAlunoResponse> {
    return this.apiService.post<MatricularAlunoResponse>(`${this.endpoint}/${alunoId}/matricular`, request)
      .pipe(
        map(response => {
          // Handle both response formats
          return response.data || response as any;
        })
      );
  }

  // Options and utilities
  obterOpcoes(): Observable<OpcoesAluno> {
    // This could be a static method or come from API
    return new Observable(observer => {
      const opcoes: OpcoesAluno = {
        generos: this.getOpcoesGenero(),
        tiposDeficiencia: this.getOpcoesTiposDeficiencia()
      };
      observer.next(opcoes);
      observer.complete();
    });
  }

  private getOpcoesGenero(): OpcaoGenero[] {
    return [
      { valor: TipoGenero.NaoInformado, descricao: 'Não Informado' },
      { valor: TipoGenero.Masculino, descricao: 'Masculino' },
      { valor: TipoGenero.Feminino, descricao: 'Feminino' },
      { valor: TipoGenero.NaoBinario, descricao: 'Não Binário' }
    ];
  }

  private getOpcoesTiposDeficiencia(): OpcaoDeficiencia[] {
    return [
      { valor: TipoDeficiencia.Visual, descricao: 'Visual' },
      { valor: TipoDeficiencia.Auditiva, descricao: 'Auditiva' },
      { valor: TipoDeficiencia.Fisica, descricao: 'Física' },
      { valor: TipoDeficiencia.Intelectual, descricao: 'Intelectual' },
      { valor: TipoDeficiencia.Multipla, descricao: 'Múltipla' },
      { valor: TipoDeficiencia.Autismo, descricao: 'Autismo' },
      { valor: TipoDeficiencia.Surdocegueira, descricao: 'Surdocegueira' }
    ];
  }

  // Validation utilities
  validarCPF(cpf: string): boolean {
    // Remove non-numeric characters
    cpf = cpf.replace(/[^\d]/g, '');

    // Check if has 11 digits
    if (cpf.length !== 11) return false;

    // Check if all digits are the same
    if (/^(\d)\1{10}$/.test(cpf)) return false;

    // Validate first check digit
    let sum = 0;
    for (let i = 0; i < 9; i++) {
      sum += parseInt(cpf.charAt(i)) * (10 - i);
    }
    let remainder = (sum * 10) % 11;
    if (remainder === 10 || remainder === 11) remainder = 0;
    if (remainder !== parseInt(cpf.charAt(9))) return false;

    // Validate second check digit
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

  determinarFaixaEtariaEscolar(idade: number): string {
    if (idade <= 5) return 'Educação Infantil';
    if (idade <= 14) return 'Ensino Fundamental';
    if (idade <= 17) return 'Ensino Médio';
    return 'Educação de Jovens e Adultos';
  }

  // Search and filter utilities
  buscarAlunos(termo: string): Observable<AlunoResumo[]> {
    return this.obterAlunos({ searchTerm: termo })
      .pipe(
        map(response => response.alunos)
      );
  }

  filtrarAlunosAtivos(): Observable<AlunoResumo[]> {
    return this.obterAlunos({ ativo: true })
      .pipe(
        map(response => response.alunos)
      );
  }

  filtrarAlunosInativos(): Observable<AlunoResumo[]> {
    return this.obterAlunos({ ativo: false })
      .pipe(
        map(response => response.alunos)
      );
  }
}