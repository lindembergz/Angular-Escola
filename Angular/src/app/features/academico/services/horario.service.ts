import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  Horario,
  CriarHorarioRequest,
  AtualizarHorarioRequest,
  GradeHoraria,
  HorarioProfessor,
  ConflitosHorario,
  HorarioEstatisticas,
  OpcoesHorario,
  VerificarConflitosRequest,
  HorarioDetalhe
} from '../models/horario.model';

@Injectable({
  providedIn: 'root'
})
export class HorarioService {
  private readonly apiUrl = `${environment.apiUrl}/api/horarios`;

  constructor(private http: HttpClient) {}

  obterHorarioPorId(id: string): Observable<Horario> {
    return this.http.get<Horario>(`${this.apiUrl}/${id}`);
  }

  criarHorario(request: CriarHorarioRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, request);
  }

  atualizarHorario(request: AtualizarHorarioRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${request.id}`, request);
  }

  obterGradeHorariaTurma(turmaId: string, anoLetivo: number, semestre: number): Observable<GradeHoraria> {
    const params = new HttpParams()
      .set('anoLetivo', anoLetivo.toString())
      .set('semestre', semestre.toString());
    
    return this.http.get<GradeHoraria>(`${this.apiUrl}/turma/${turmaId}`, { params });
  }

  obterHorariosProfessor(professorId: string, anoLetivo: number, semestre: number): Observable<HorarioProfessor> {
    const params = new HttpParams()
      .set('anoLetivo', anoLetivo.toString())
      .set('semestre', semestre.toString());
    
    return this.http.get<HorarioProfessor>(`${this.apiUrl}/professor/${professorId}`, { params });
  }

  obterHorariosDisciplina(disciplinaId: string, anoLetivo: number, semestre: number): Observable<HorarioDetalhe[]> {
    const params = new HttpParams()
      .set('anoLetivo', anoLetivo.toString())
      .set('semestre', semestre.toString());
    
    return this.http.get<HorarioDetalhe[]>(`${this.apiUrl}/disciplina/${disciplinaId}`, { params });
  }

  obterHorariosPorDia(diaSemana: string, escolaId?: string): Observable<HorarioDetalhe[]> {
    let params = new HttpParams();
    if (escolaId) params = params.set('escolaId', escolaId);
    
    return this.http.get<HorarioDetalhe[]>(`${this.apiUrl}/dia-semana/${diaSemana}`, { params });
  }

  verificarConflitos(request: VerificarConflitosRequest): Observable<ConflitosHorario> {
    return this.http.post<ConflitosHorario>(`${this.apiUrl}/verificar-conflitos`, request);
  }

  cancelarHorario(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/cancelar`, {});
  }

  reativarHorario(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/reativar`, {});
  }

  alterarProfessor(horarioId: string, professorId: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${horarioId}/alterar-professor`, {
      professorId
    });
  }

  alterarSala(horarioId: string, sala: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${horarioId}/alterar-sala`, {
      sala
    });
  }

  obterEstatisticas(escolaId: string, anoLetivo: number, semestre: number): Observable<HorarioEstatisticas> {
    const params = new HttpParams()
      .set('escolaId', escolaId)
      .set('anoLetivo', anoLetivo.toString())
      .set('semestre', semestre.toString());
    
    return this.http.get<HorarioEstatisticas>(`${this.apiUrl}/estatisticas`, { params });
  }

  obterOpcoes(): Observable<OpcoesHorario> {
    // Mock data - em um cenário real, isso viria do backend
    return new Observable(observer => {
      observer.next({
        diasSemana: [
          { valor: 'SEGUNDA', nome: 'Segunda-feira', abreviacao: 'SEG' },
          { valor: 'TERCA', nome: 'Terça-feira', abreviacao: 'TER' },
          { valor: 'QUARTA', nome: 'Quarta-feira', abreviacao: 'QUA' },
          { valor: 'QUINTA', nome: 'Quinta-feira', abreviacao: 'QUI' },
          { valor: 'SEXTA', nome: 'Sexta-feira', abreviacao: 'SEX' },
          { valor: 'SABADO', nome: 'Sábado', abreviacao: 'SAB' }
        ],
        horariosDisponiveis: [
          '07:00', '07:50', '08:40', '09:30', '10:20', '11:10',
          '13:00', '13:50', '14:40', '15:30', '16:20', '17:10',
          '19:00', '19:50', '20:40', '21:30'
        ],
        semestres: [1, 2],
        salas: ['Sala 01', 'Sala 02', 'Sala 03', 'Laboratório', 'Biblioteca', 'Auditório']
      });
      observer.complete();
    });
  }
}