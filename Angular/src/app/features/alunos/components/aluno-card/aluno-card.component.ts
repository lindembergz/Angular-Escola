import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { AvatarModule } from 'primeng/avatar';
import { BadgeModule } from 'primeng/badge';

import { AlunoResumo } from '../../models/aluno.model';

@Component({
  selector: 'app-aluno-card',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    CardModule,
    ButtonModule,
    TagModule,
    TooltipModule,
    AvatarModule,
    BadgeModule
  ],
  template: `
    <p-card [styleClass]="'aluno-card h-full ' + (aluno.ativo ? '' : 'inactive-card')">
      <ng-template pTemplate="header">
        <div class="card-header p-3" [ngClass]="aluno.ativo ? 'bg-primary' : 'bg-gray-400'">
          <div class="flex align-items-center">
            <p-avatar 
              [label]="getInitials(aluno.nome)" 
              styleClass="mr-3"
              size="large"
              [style]="{'background-color': aluno.ativo ? 'rgba(255,255,255,0.2)' : 'rgba(255,255,255,0.1)'}">
            </p-avatar>
            <div class="flex-1">
              <h3 class="m-0 text-white font-medium">{{ aluno.nome }}</h3>
              <p class="m-0 text-100 text-sm">{{ aluno.idade }} anos • {{ aluno.generoDescricao }}</p>
            </div>
            <div class="flex flex-column align-items-end gap-1">
              <p-badge 
                *ngIf="aluno.possuiDeficiencia"
                value="PcD"
                severity="warn"
                [pTooltip]="aluno.deficienciaDescricao">
              </p-badge>
            </div>
          </div>
        </div>
      </ng-template>

      <div class="card-content">
        <!-- Status Information -->
        <div class="flex justify-content-between align-items-center mb-3">
          <div class="flex gap-2">
            <p-tag 
              [value]="aluno.ativo ? 'Ativo' : 'Inativo'"
              [severity]="aluno.ativo ? 'success' : 'danger'"
              styleClass="text-xs">
            </p-tag>
            <p-tag 
              *ngIf="aluno.possuiMatriculaAtiva"
              value="Matriculado"
              severity="info"
              styleClass="text-xs">
            </p-tag>
          </div>
        </div>

        <!-- Student Information -->
        <div class="student-info mb-3">
          <div class="info-row" *ngIf="aluno.cpf">
            <span class="info-label">CPF:</span>
            <span class="info-value">{{ aluno.cpf }}</span>
          </div>
          
          <div class="info-row" *ngIf="aluno.nomeTurmaAtual">
            <span class="info-label">Turma:</span>
            <span class="info-value">{{ aluno.nomeTurmaAtual }}</span>
          </div>
          
          <div class="info-row" *ngIf="aluno.telefone">
            <span class="info-label">Telefone:</span>
            <span class="info-value">{{ aluno.telefone }}</span>
          </div>
          
          <div class="info-row" *ngIf="aluno.email">
            <span class="info-label">E-mail:</span>
            <span class="info-value text-sm">{{ aluno.email }}</span>
          </div>
          
          <div class="info-row">
            <span class="info-label">Responsáveis:</span>
            <span class="info-value">{{ aluno.quantidadeResponsaveis }}</span>
          </div>
          
          <div class="info-row">
            <span class="info-label">Cadastro:</span>
            <span class="info-value">{{ aluno.dataCadastro | date:'dd/MM/yyyy' }}</span>
          </div>
        </div>

        <!-- Actions -->
        <div class="flex gap-2 mt-3">
          <p-button 
            label="Ver Detalhes" 
            icon="pi pi-eye" 
            styleClass="p-button-sm flex-1"
            [routerLink]="['/alunos', aluno.id]">
          </p-button>
          
          <p-button 
            icon="pi pi-pencil" 
            styleClass="p-button-sm p-button-outlined"
            pTooltip="Editar"
            [routerLink]="['/alunos', aluno.id, 'editar']">
          </p-button>
          
          <p-button 
            icon="pi pi-trash" 
            styleClass="p-button-sm p-button-outlined p-button-danger"
            pTooltip="Excluir"
            (onClick)="onDelete()">
          </p-button>
        </div>
      </div>
    </p-card>
  `,
  styles: [`
    .aluno-card {
      transition: all 0.3s ease;
      border: 1px solid var(--surface-border);
    }

    .aluno-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    }

    .inactive-card {
      opacity: 0.7;
    }

    .card-header {
      border-radius: 6px 6px 0 0;
    }

    .card-content {
      padding: 1rem;
    }

    .student-info {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .info-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.25rem 0;
      border-bottom: 1px solid var(--surface-border);
    }

    .info-row:last-child {
      border-bottom: none;
    }

    .info-label {
      font-weight: 500;
      color: var(--text-color-secondary);
      font-size: 0.875rem;
    }

    .info-value {
      color: var(--text-color);
      font-size: 0.875rem;
      text-align: right;
      max-width: 60%;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    :host ::ng-deep .p-card .p-card-body {
      padding: 0;
    }

    :host ::ng-deep .p-avatar {
      color: white;
      font-weight: 600;
    }

    :host ::ng-deep .p-tag {
      font-size: 0.75rem;
    }

    :host ::ng-deep .p-button-sm {
      padding: 0.375rem 0.75rem;
      font-size: 0.875rem;
    }
  `]
})
export class AlunoCardComponent {
  @Input() aluno!: AlunoResumo;
  @Input() showActions = true;
  @Input() compact = false;

  @Output() deleteClick = new EventEmitter<AlunoResumo>();
  @Output() editClick = new EventEmitter<AlunoResumo>();
  @Output() viewClick = new EventEmitter<AlunoResumo>();

  getInitials(nome: string): string {
    return nome.split(' ')
      .map(n => n[0])
      .join('')
      .substring(0, 2)
      .toUpperCase();
  }

  onDelete(): void {
    this.deleteClick.emit(this.aluno);
  }

  onEdit(): void {
    this.editClick.emit(this.aluno);
  }

  onView(): void {
    this.viewClick.emit(this.aluno);
  }
}