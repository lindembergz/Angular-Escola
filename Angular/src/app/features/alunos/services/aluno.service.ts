import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ApiService } from '../../../core/services/api.service';
import { 
  Aluno,
  CriarAlunoRequest,
  CriarAlunoResponse,
  AtualizarAlunoRequest,
  AtualizarAlunoResponse
} from '../models/aluno.model';

@Injectable({
  providedIn: 'root'
})
export class AlunoService {

  private readonly endpoint = 'alunos';

  constructor(private apiService: ApiService) {}

  // Individual student operations
  obterAluno(id: string): Observable<Aluno> {
    return this.apiService.getById<Aluno>(this.endpoint, id)
      .pipe(
        map(response => response.data)
      );
  }

  criarAluno(request: CriarAlunoRequest): Observable<CriarAlunoResponse> {
    return this.apiService.post<CriarAlunoResponse>(this.endpoint, request)
      .pipe(
        map(response => response.data)
      );
  }

  atualizarAluno(request: AtualizarAlunoRequest): Observable<AtualizarAlunoResponse> {
    return this.apiService.put<AtualizarAlunoResponse>(this.endpoint, request.id, request)
      .pipe(
        map(response => response.data)
      );
  }

  excluirAluno(id: string): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id)
      .pipe(
        map(() => void 0)
      );
  }

  // Utility methods
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
}