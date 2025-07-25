import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  Disciplina,
  CriarDisciplinaRequest,
  AtualizarDisciplinaRequest,
  ObterDisciplinasRequest,
  ObterDisciplinasResponse,
  DisciplinaEstatisticas,
  PreRequisito,
  OpcoesDisciplina
} from '../models/disciplina.model';

@Injectable({
  providedIn: 'root'
})
export class DisciplinaService {
  private readonly apiUrl = `${environment.apiUrl}/api/disciplinas`;

  constructor(private http: HttpClient) {}

  obterDisciplinas(request: ObterDisciplinasRequest = {}): Observable<ObterDisciplinasResponse> {
    let params = new HttpParams();
    
    if (request.page) params = params.set('page', request.page.toString());
    if (request.pageSize) params = params.set('pageSize', request.pageSize.toString());
    if (request.escolaId) params = params.set('escolaId', request.escolaId);
    if (request.serie) params = params.set('serie', request.serie);
    if (request.obrigatoria !== undefined) params = params.set('obrigatoria', request.obrigatoria.toString());
    if (request.ativa !== undefined) params = params.set('ativa', request.ativa.toString());
    if (request.searchTerm) params = params.set('searchTerm', request.searchTerm);

    return this.http.get<ObterDisciplinasResponse>(this.apiUrl, { params });
  }

  obterDisciplinaPorId(id: string): Observable<Disciplina> {
    return this.http.get<Disciplina>(`${this.apiUrl}/${id}`);
  }

  criarDisciplina(request: CriarDisciplinaRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, request);
  }

  atualizarDisciplina(request: AtualizarDisciplinaRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${request.id}`, request);
  }

  obterDisciplinasPorSerie(serie: string, escolaId: string): Observable<Disciplina[]> {
    const params = new HttpParams()
      .set('serie', serie)
      .set('escolaId', escolaId);
    
    return this.http.get<Disciplina[]>(`${this.apiUrl}/serie`, { params });
  }

  obterDisciplinasObrigatorias(escolaId: string): Observable<Disciplina[]> {
    const params = new HttpParams().set('escolaId', escolaId);
    return this.http.get<Disciplina[]>(`${this.apiUrl}/obrigatorias`, { params });
  }

  pesquisarDisciplinas(nome: string, escolaId: string): Observable<Disciplina[]> {
    const params = new HttpParams()
      .set('nome', nome)
      .set('escolaId', escolaId);
    
    return this.http.get<Disciplina[]>(`${this.apiUrl}/pesquisar`, { params });
  }

  adicionarPreRequisito(disciplinaId: string, preRequisitoId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${disciplinaId}/pre-requisitos`, {
      preRequisitoId
    });
  }

  removerPreRequisito(disciplinaId: string, preRequisitoId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${disciplinaId}/pre-requisitos/${preRequisitoId}`);
  }

  inativarDisciplina(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/inativar`, {});
  }

  reativarDisciplina(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/reativar`, {});
  }

  obterEstatisticas(escolaId: string): Observable<DisciplinaEstatisticas> {
    const params = new HttpParams().set('escolaId', escolaId);
    return this.http.get<DisciplinaEstatisticas>(`${this.apiUrl}/estatisticas`, { params });
  }

  obterOpcoes(): Observable<OpcoesDisciplina> {
    // Mock data - em um cenário real, isso viria do backend
    return new Observable(observer => {
      observer.next({
        series: ['1º Ano', '2º Ano', '3º Ano', '4º Ano', '5º Ano', '6º Ano', '7º Ano', '8º Ano', '9º Ano', '1º EM', '2º EM', '3º EM'],
        codigosSugeridos: ['MAT', 'POR', 'HIS', 'GEO', 'CIE', 'EDF', 'ART', 'ING', 'FIS', 'QUI', 'BIO', 'FIL', 'SOC']
      });
      observer.complete();
    });
  }
}