import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../../core/services/api.service';
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
  private readonly escolaEndpoint = 'Escola';
  private readonly redeEndpoint = 'RedeEscolar';

  constructor(private apiService: ApiService) {}

  // Escolas
  getEscolas(filter?: EscolasFilter): Observable<PagedResult<Escola>> {
    const params: any = {};
    
    if (filter) {
      if (filter.nome) params.filtroNome = filter.nome;
      if (filter.tipoEscola) params.filtroTipo = filter.tipoEscola;
      if (filter.ativa !== undefined) params.filtroAtiva = filter.ativa;
      if (filter.page) params.pagina = filter.page;
      if (filter.pageSize) params.tamanhoPagina = filter.pageSize;
    }

    return this.apiService.get<{escolas: Escola[], total: number}>(`${this.escolaEndpoint}/paginado`, params)
      .pipe(
        map(response => ({
          items: response.data.escolas,
          totalCount: response.data.total,
          page: filter?.page || 1,
          pageSize: filter?.pageSize || 10,
          totalPages: Math.ceil(response.data.total / (filter?.pageSize || 10))
        }))
      );
  }

  getAllEscolas(): Observable<Escola[]> {
    return this.apiService.get<Escola[]>(this.escolaEndpoint)
      .pipe(map(response => response.data));
  }

  getEscolasAtivas(): Observable<Escola[]> {
    return this.apiService.get<Escola[]>(`${this.escolaEndpoint}/ativas`)
      .pipe(map(response => response.data));
  }

  getEscolaById(id: string): Observable<Escola> {
    return this.apiService.getById<Escola>(this.escolaEndpoint, id)
      .pipe(map(response => response.data));
  }

  getEscolasPorTipo(tipo: string): Observable<Escola[]> {
    return this.apiService.get<Escola[]>(`${this.escolaEndpoint}/tipo/${tipo}`)
      .pipe(map(response => response.data));
  }

  pesquisarEscolasPorNome(nome: string): Observable<Escola[]> {
    return this.apiService.get<Escola[]>(`${this.escolaEndpoint}/pesquisar`, { nome })
      .pipe(map(response => response.data));
  }

  createEscola(escola: CreateEscolaRequest): Observable<Escola> {
    return this.apiService.post<Escola>(this.escolaEndpoint, escola)
      .pipe(map(response => response.data));
  }

  updateEscola(escola: UpdateEscolaRequest): Observable<Escola> {
    return this.apiService.put<Escola>(this.escolaEndpoint, escola.id, escola)
      .pipe(map(response => response.data));
  }

  deleteEscola(id: string): Observable<void> {
    return this.apiService.delete<void>(this.escolaEndpoint, id)
      .pipe(map(() => void 0));
  }

  ativarEscola(id: string): Observable<void> {
    return this.apiService.patch<void>(this.escolaEndpoint, id, { acao: 'ativar' })
      .pipe(map(() => void 0));
  }

  desativarEscola(id: string): Observable<void> {
    return this.apiService.patch<void>(this.escolaEndpoint, id, { acao: 'desativar' })
      .pipe(map(() => void 0));
  }

  // Redes Escolares
  getRedesEscolares(): Observable<RedeEscolar[]> {
    return this.apiService.get<RedeEscolar[]>(this.redeEndpoint)
      .pipe(map(response => response.data));
  }

  getRedesEscolaresAtivas(): Observable<RedeEscolar[]> {
    return this.apiService.get<RedeEscolar[]>(`${this.redeEndpoint}/ativas`)
      .pipe(map(response => response.data));
  }

  getRedeEscolarById(id: string): Observable<RedeEscolar> {
    return this.apiService.getById<RedeEscolar>(this.redeEndpoint, id)
      .pipe(map(response => response.data));
  }

  getRedesEscolaresPaginado(pagina: number = 1, tamanhoPagina: number = 10, filtroNome?: string, filtroAtiva?: boolean): Observable<PagedResult<RedeEscolar>> {
    const params: any = {
      pagina: pagina,
      tamanhoPagina: tamanhoPagina
    };
    
    if (filtroNome) params.filtroNome = filtroNome;
    if (filtroAtiva !== undefined) params.filtroAtiva = filtroAtiva;

    return this.apiService.get<{redes: RedeEscolar[], total: number}>(`${this.redeEndpoint}/paginado`, params)
      .pipe(
        map(response => ({
          items: response.data.redes,
          totalCount: response.data.total,
          page: pagina,
          pageSize: tamanhoPagina,
          totalPages: Math.ceil(response.data.total / tamanhoPagina)
        }))
      );
  }

  createRedeEscolar(rede: CreateRedeEscolarRequest): Observable<RedeEscolar> {
    return this.apiService.post<RedeEscolar>(this.redeEndpoint, rede)
      .pipe(map(response => response.data));
  }

  updateRedeEscolar(rede: Partial<RedeEscolar> & { id: string }): Observable<RedeEscolar> {
    return this.apiService.put<RedeEscolar>(this.redeEndpoint, rede.id, rede)
      .pipe(map(response => response.data));
  }

  deleteRedeEscolar(id: string): Observable<void> {
    return this.apiService.delete<void>(this.redeEndpoint, id)
      .pipe(map(() => void 0));
  }

  ativarRedeEscolar(id: string): Observable<void> {
    return this.apiService.patch<void>(this.redeEndpoint, id, { acao: 'ativar' })
      .pipe(map(() => void 0));
  }

  desativarRedeEscolar(id: string): Observable<void> {
    return this.apiService.patch<void>(this.redeEndpoint, id, { acao: 'desativar' })
      .pipe(map(() => void 0));
  }

  // Relacionamentos
  getEscolasPorRede(redeEscolarId: string): Observable<Escola[]> {
    return this.apiService.get<Escola[]>(`${this.redeEndpoint}/${redeEscolarId}/escolas`)
      .pipe(map(response => response.data));
  }

  associarEscolaRede(escolaId: string, redeEscolarId: string): Observable<void> {
    return this.apiService.post<void>(`${this.escolaEndpoint}/${escolaId}/associar-rede/${redeEscolarId}`, {})
      .pipe(map(() => void 0));
  }

  desassociarEscolaRede(escolaId: string): Observable<void> {
    return this.apiService.post<void>(`${this.escolaEndpoint}/${escolaId}/desassociar-rede`, {})
      .pipe(map(() => void 0));
  }

  // Unidades Escolares
  adicionarUnidadeEscolar(unidade: CreateUnidadeEscolarRequest): Observable<any> {
    return this.apiService.post(`${this.escolaEndpoint}/${unidade.escolaId}/unidades`, unidade)
      .pipe(map(response => response.data));
  }

  removerUnidadeEscolar(escolaId: string, unidadeId: string): Observable<void> {
    return this.apiService.delete<void>(`${this.escolaEndpoint}/${escolaId}/unidades/${unidadeId}`)
      .pipe(map(() => void 0));
  }

  // Estatísticas
  getEstatisticasPorTipo(): Observable<{[key: string]: number}> {
    return this.apiService.get<{[key: string]: number}>(`${this.escolaEndpoint}/estatisticas/tipo`)
      .pipe(map(response => response.data));
  }

  getEstatisticasPorEstado(): Observable<{[key: string]: number}> {
    return this.apiService.get<{[key: string]: number}>(`${this.escolaEndpoint}/estatisticas/estado`)
      .pipe(map(response => response.data));
  }

  getTotalEscolasAtivas(): Observable<number> {
    return this.apiService.get<number>(`${this.escolaEndpoint}/estatisticas/total-ativas`)
      .pipe(map(response => response.data));
  }

  getTotalRedesEscolaresAtivas(): Observable<number> {
    return this.apiService.get<number>(`${this.redeEndpoint}/estatisticas/total-ativas`)
      .pipe(map(response => response.data));
  }
}