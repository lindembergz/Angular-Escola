import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

import { AlunosFacade } from '../../../../store/alunos/alunos.facade';

@Component({
  selector: 'app-aluno-id-helper',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    CardModule,
    ButtonModule,
    TableModule,
    TagModule,
    TooltipModule,
    ProgressSpinnerModule
  ],
  template: `
    <p-card>
      <ng-template pTemplate="header">
        <div class="p-3">
          <h3 class="text-lg font-medium text-900 m-0 flex align-items-center">
            <i class="pi pi-info-circle text-blue-500 mr-2"></i>
            IDs de Alunos Disponíveis
          </h3>
        </div>
      </ng-template>
      
      <div class="mb-3">
        <p class="text-600 mb-2">
          Aqui estão os IDs válidos dos alunos cadastrados no sistema:
        </p>
        <small class="text-500">
          Clique em "Ver Detalhes" para acessar as informações completas do aluno.
        </small>
      </div>

      <!-- Loading -->
      <div *ngIf="loading$ | async" class="flex justify-content-center p-4">
        <p-progressSpinner></p-progressSpinner>
      </div>

      <!-- Table -->
      <p-table 
        [value]="(alunos$ | async) || []" 
        [paginator]="false"
        styleClass="p-datatable-gridlines p-datatable-sm"
        responsiveLayout="scroll"
        *ngIf="!(loading$ | async)">
        
        <ng-template pTemplate="header">
          <tr>
            <th>Nome</th>
            <th>ID (GUID)</th>
            <th>Status</th>
            <th>Ações</th>
          </tr>
        </ng-template>
        
        <ng-template pTemplate="body" let-aluno>
          <tr>
            <td>
              <div class="font-medium">{{ aluno.nome }}</div>
              <div class="text-sm text-600">{{ aluno.cpf }}</div>
            </td>
            <td>
              <code class="text-sm bg-gray-100 p-1 border-round">{{ aluno.id }}</code>
            </td>
            <td>
              <p-tag 
                [value]="aluno.ativo ? 'Ativo' : 'Inativo'"
                [severity]="aluno.ativo ? 'success' : 'danger'"
                styleClass="text-xs">
              </p-tag>
            </td>
            <td>
              <div class="flex gap-2">
                <p-button 
                  label="Ver Detalhes" 
                  icon="pi pi-eye" 
                  styleClass="p-button-sm p-button-outlined"
                  [routerLink]="['/alunos', aluno.id]">
                </p-button>
                <p-button 
                  icon="pi pi-copy" 
                  styleClass="p-button-sm p-button-text"
                  pTooltip="Copiar ID"
                  (onClick)="copyToClipboard(aluno.id)">
                </p-button>
              </div>
            </td>
          </tr>
        </ng-template>
        
        <ng-template pTemplate="emptymessage">
          <tr>
            <td colspan="4" class="text-center py-4">
              <div class="flex flex-column align-items-center">
                <i class="pi pi-users text-4xl text-400 mb-3"></i>
                <p class="text-600 mb-0">Nenhum aluno encontrado</p>
              </div>
            </td>
          </tr>
        </ng-template>
      </p-table>

      <ng-template pTemplate="footer">
        <div class="flex justify-content-between align-items-center">
          <small class="text-500">
            💡 Dica: Use estes IDs para acessar diretamente os detalhes dos alunos
          </small>
          <p-button 
            label="Ir para Lista Completa" 
            icon="pi pi-list" 
            styleClass="p-button-sm p-button-outlined"
            routerLink="/alunos">
          </p-button>
        </div>
      </ng-template>
    </p-card>
  `,
  styles: [`
    :host ::ng-deep .p-card .p-card-body {
      padding: 1.5rem;
    }

    :host ::ng-deep .p-datatable .p-datatable-tbody > tr > td {
      padding: 0.75rem;
    }

    :host ::ng-deep code {
      font-family: 'Courier New', monospace;
      font-size: 0.8rem;
      word-break: break-all;
    }

    :host ::ng-deep .p-tag {
      font-size: 0.75rem;
    }
  `]
})
export class AlunoIdHelperComponent implements OnInit {
  
  // Observables
  alunos$;
  loading$;

  constructor(private alunosFacade: AlunosFacade) {
    this.alunos$ = this.alunosFacade.alunos$;
    this.loading$ = this.alunosFacade.loading$;
  }

  ngOnInit(): void {
    // Load alunos to show available IDs
    this.alunosFacade.loadAlunos({ pageSize: 10 }); // Limit to first 10 for this helper
  }

  copyToClipboard(text: string): void {
    navigator.clipboard.writeText(text).then(() => {
      console.log('✅ ID copiado para a área de transferência:', text);
      // You could show a toast notification here
    }).catch(err => {
      console.error('❌ Erro ao copiar ID:', err);
    });
  }
}