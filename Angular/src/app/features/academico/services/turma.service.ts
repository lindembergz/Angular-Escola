import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  Turma,
  CriarTurmaRequest,
  AtualizarTurmaRequest,
  MatricularAlunoRequest,
  ObterTurmasRequest,
  ObterTurmasResponse,
  TurmaEstatisticas,
  TurmaComVagas,
  OpcoesTurma
} from '../models/turma.model';

@Injectable({
  providedIn: 'root'
})
export class TurmaService {
  private readonly apiUrl = `${environment.apiUrl}/api/turmas`;

  constructor(private http: HttpClient) {}

  obterTurmas(request: ObterTurmasRequest = {}): Observable<ObterTurmasResponse> {
    let params = new HttpParams();
    
    if (request.page) params = params.set('page', request.page.toString());
    if (request.pageSize) params = params.set('pageSize', request.pageSize.toString());
    if (request.escolaId) params = params.set('escolaId', request.escolaId);
    if (request.anoLetivo) params = params.set('anoLetivo', request.anoLetivo.toString());
    if (request.serie) params = params.set('serie', request.serie);
    if (request.turno) params = params.set('turno', request.turno);
    if (request.ativa !== undefined) params = params.set('ativa', request.ativa.toString());

  alert("this.http.get");
    return this.http.get<ObterTurmasResponse>(this.apiUrl, { params });
  }

  obterTurmaPorId(id: string): Observable<Turma> {
    return this.http.get<Turma>(`${this.apiUrl}/${id}`);
  }

  criarTurma(request: CriarTurmaRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, request);
  }

  atualizarTurma(request: AtualizarTurmaRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${request.id}`, request);
  }

  matricularAluno(request: MatricularAlunoRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${request.turmaId}/matricular`, {
      alunoId: request.alunoId
    });
  }

  obterTurmasPorAnoLetivo(anoLetivo: number, unidadeEscolarId?: string): Observable<Turma[]> {
    let params = new HttpParams();
    if (unidadeEscolarId) params = params.set('unidadeEscolarId', unidadeEscolarId);

    return this.http.get<Turma[]>(`${this.apiUrl}/ano-letivo/${anoLetivo}`, { params });
  }

  obterTurmasComVagas(unidadeEscolarId: string): Observable<TurmaComVagas[]> {
    const params = new HttpParams().set('unidadeEscolarId', unidadeEscolarId);
    return this.http.get<TurmaComVagas[]>(`${this.apiUrl}/vagas-disponiveis`, { params });
  }

  inativarTurma(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/inativar`, {});
  }

  reativarTurma(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/reativar`, {});
  }

  obterEstatisticas(unidadeEscolarId: string): Observable<TurmaEstatisticas> {
    const params = new HttpParams().set('unidadeEscolarId', unidadeEscolarId);
    return this.http.get<TurmaEstatisticas>(`${this.apiUrl}/estatisticas`, { params });
  }

  obterOpcoes(): Observable<OpcoesTurma> {
    // Mock data - em um cenário real, isso viria do backend
    return new Observable(observer => {
      observer.next({
        series: ['1º Ano', '2º Ano', '3º Ano', '4º Ano', '5º Ano', '6º Ano', '7º Ano', '8º Ano', '9º Ano', '1º EM', '2º EM', '3º EM'],
        turnos: ['Matutino', 'Vespertino', 'Noturno', 'Integral'],
        anosLetivos: [2023, 2024, 2025, 2026]
      });
      observer.complete();
    });
  }
}