import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { 
  Escola, 
  RedeEscolar, 
  CreateEscolaRequest, 
  UpdateEscolaRequest, 
  CreateRedeEscolarRequest,
  CreateUnidadeEscolarRequest,
  EscolasFilter,
  PagedResult
} from '../models/escola.model';

@Injectable({
  providedIn: 'root'
})
export class EscolasService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Escolas
  getEscolas(filter?: EscolasFilter): Observable<PagedResult<Escola>> {
    let params = new HttpParams();
    
    if (filter) {
      if (filter.nome) params = params.set('filtroNome', filter.nome);
      if (filter.tipoEscola) params = params.set('filtroTipo', filter.tipoEscola);
      if (filter.ativa !== undefined) params = params.set('filtroAtiva', filter.ativa.toString());
      if (filter.page) params = params.set('pagina', filter.page.toString());
      if (filter.pageSize) params = params.set('tamanhoPagina', filter.pageSize.toString());
    }

    return this.http.get<{escolas: Escola[], total: number}>(`${this.apiUrl}/escolas/paginado`, { params })
      .pipe(
        map(response => ({
          items: response.escolas,
          totalCount: response.total,
          page: filter?.page || 1,
          pageSize: filter?.pageSize || 10,
          totalPages: Math.ceil(response.total / (filter?.pageSize || 10))
        }))
      );
  }

  getAllEscolas(): Observable<Escola[]> {
    return this.http.get<Escola[]>(`${this.apiUrl}/escolas`);
  }

  getEscolasAtivas(): Observable<Escola[]> {
    return this.http.get<Escola[]>(`${this.apiUrl}/escolas/ativas`);
  }

  getEscolaById(id: string): Observable<Escola> {
    return this.http.get<Escola>(`${this.apiUrl}/escolas/${id}`);
  }

  getEscolasPorTipo(tipo: string): Observable<Escola[]> {
    return this.http.get<Escola[]>(`${this.apiUrl}/escolas/tipo/${tipo}`);
  }

  pesquisarEscolasPorNome(nome: string): Observable<Escola[]> {
    return this.http.get<Escola[]>(`${this.apiUrl}/escolas/pesquisar/${nome}`);
  }

  createEscola(escola: CreateEscolaRequest): Observable<Escola> {
    return this.http.post<Escola>(`${this.apiUrl}/escolas`, escola);
  }

  updateEscola(escola: UpdateEscolaRequest): Observable<Escola> {
    return this.http.put<Escola>(`${this.apiUrl}/escolas/${escola.id}`, escola);
  }

  deleteEscola(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/escolas/${id}`);
  }

  ativarEscola(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/escolas/${id}/ativar`, {});
  }

  desativarEscola(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/escolas/${id}/desativar`, {});
  }

  // Redes Escolares
  getRedesEscolares(): Observable<RedeEscolar[]> {
    return this.http.get<RedeEscolar[]>(`${this.apiUrl}/redes-escolares`);
  }

  getRedesEscolaresAtivas(): Observable<RedeEscolar[]> {
    return this.http.get<RedeEscolar[]>(`${this.apiUrl}/redes-escolares/ativas`);
  }

  getRedeEscolarById(id: string): Observable<RedeEscolar> {
    return this.http.get<RedeEscolar>(`${this.apiUrl}/redes-escolares/${id}`);
  }

  getRedesEscolaresPaginado(pagina: number = 1, tamanhoPagina: number = 10, filtroNome?: string, filtroAtiva?: boolean): Observable<PagedResult<RedeEscolar>> {
    let params = new HttpParams()
      .set('pagina', pagina.toString())
      .set('tamanhoPagina', tamanhoPagina.toString());
    
    if (filtroNome) params = params.set('filtroNome', filtroNome);
    if (filtroAtiva !== undefined) params = params.set('filtroAtiva', filtroAtiva.toString());

    return this.http.get<{redes: RedeEscolar[], total: number}>(`${this.apiUrl}/redes-escolares/paginado`, { params })
      .pipe(
        map(response => ({
          items: response.redes,
          totalCount: response.total,
          page: pagina,
          pageSize: tamanhoPagina,
          totalPages: Math.ceil(response.total / tamanhoPagina)
        }))
      );
  }

  createRedeEscolar(rede: CreateRedeEscolarRequest): Observable<RedeEscolar> {
    return this.http.post<RedeEscolar>(`${this.apiUrl}/redes-escolares`, rede);
  }

  updateRedeEscolar(rede: Partial<RedeEscolar> & { id: string }): Observable<RedeEscolar> {
    return this.http.put<RedeEscolar>(`${this.apiUrl}/redes-escolares/${rede.id}`, rede);
  }

  deleteRedeEscolar(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/redes-escolares/${id}`);
  }

  ativarRedeEscolar(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/redes-escolares/${id}/ativar`, {});
  }

  desativarRedeEscolar(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/redes-escolares/${id}/desativar`, {});
  }

  // Relacionamentos
  getEscolasPorRede(redeEscolarId: string): Observable<Escola[]> {
    return this.http.get<Escola[]>(`${this.apiUrl}/redes-escolares/${redeEscolarId}/escolas`);
  }

  associarEscolaRede(escolaId: string, redeEscolarId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/escolas/${escolaId}/associar-rede/${redeEscolarId}`, {});
  }

  desassociarEscolaRede(escolaId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/escolas/${escolaId}/desassociar-rede`, {});
  }

  // Unidades Escolares
  adicionarUnidadeEscolar(unidade: CreateUnidadeEscolarRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/escolas/${unidade.escolaId}/unidades`, unidade);
  }

  removerUnidadeEscolar(escolaId: string, unidadeId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/escolas/${escolaId}/unidades/${unidadeId}`);
  }

  // Estatísticas
  getEstatisticasPorTipo(): Observable<{[key: string]: number}> {
    return this.http.get<{[key: string]: number}>(`${this.apiUrl}/escolas/estatisticas/tipo`);
  }

  getEstatisticasPorEstado(): Observable<{[key: string]: number}> {
    return this.http.get<{[key: string]: number}>(`${this.apiUrl}/escolas/estatisticas/estado`);
  }

  getTotalEscolasAtivas(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/escolas/total-ativas`);
  }

  getTotalRedesEscolaresAtivas(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/redes-escolares/total-ativas`);
  }
}