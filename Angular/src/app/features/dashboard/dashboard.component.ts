import { Component } from '@angular/core';
import { Card } from 'primeng/card';
import { Button } from 'primeng/button';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [Card, Button, RouterModule],
  template: `
    <div class="dashboard-container">
      <div class="grid">
        <div class="col-12">
          <p-card>
            <ng-template pTemplate="header">
              <div class="flex align-items-center justify-content-center p-4">
                <i class="pi pi-graduation-cap text-6xl text-primary"></i>
              </div>
            </ng-template>
            <div class="text-center">
              <h1 class="text-4xl font-bold text-primary mb-3">
                Dashboard - Sistema de Gestão Escolar
              </h1>
              <p class="text-xl text-600 mb-4">
                Visão geral do sistema educacional
              </p>
              <p class="text-lg text-700 line-height-3">
                Acompanhe as principais métricas e acesse rapidamente as funcionalidades do sistema.
              </p>
            </div>
          </p-card>
        </div>
      </div>

      <!-- Cards de funcionalidades principais -->
      <div class="grid mt-4">
        <div class="col-12 md:col-6 lg:col-4">
          <p-card class="h-full">
            <ng-template pTemplate="header">
              <div class="flex align-items-center justify-content-center p-3 bg-blue-50">
                <i class="pi pi-users text-4xl text-blue-500"></i>
              </div>
            </ng-template>
            <div class="text-center">
              <h3 class="text-xl font-semibold mb-2">Gestão de Alunos</h3>
              <p class="text-600 mb-3">
                Cadastre e gerencie informações completas dos estudantes
              </p>
              <p-button 
                label="Acessar" 
                icon="pi pi-arrow-right" 
                routerLink="/alunos"
                [text]="true">
              </p-button>
            </div>
          </p-card>
        </div>

        <div class="col-12 md:col-6 lg:col-4">
          <p-card class="h-full">
            <ng-template pTemplate="header">
              <div class="flex align-items-center justify-content-center p-3 bg-green-50">
                <i class="pi pi-user text-4xl text-green-500"></i>
              </div>
            </ng-template>
            <div class="text-center">
              <h3 class="text-xl font-semibold mb-2">Professores</h3>
              <p class="text-600 mb-3">
                Organize o corpo docente e suas disciplinas
              </p>
              <p-button 
                label="Acessar" 
                icon="pi pi-arrow-right" 
                routerLink="/professores"
                [text]="true">
              </p-button>
            </div>
          </p-card>
        </div>

        <div class="col-12 md:col-6 lg:col-4">
          <p-card class="h-full">
            <ng-template pTemplate="header">
              <div class="flex align-items-center justify-content-center p-3 bg-orange-50">
                <i class="pi pi-book text-4xl text-orange-500"></i>
              </div>
            </ng-template>
            <div class="text-center">
              <h3 class="text-xl font-semibold mb-2">Acadêmico</h3>
              <p class="text-600 mb-3">
                Turmas, disciplinas e grade horária
              </p>
              <p-button 
                label="Acessar" 
                icon="pi pi-arrow-right" 
                routerLink="/academico"
                [text]="true">
              </p-button>
            </div>
          </p-card>
        </div>

        <div class="col-12 md:col-6 lg:col-4">
          <p-card class="h-full">
            <ng-template pTemplate="header">
              <div class="flex align-items-center justify-content-center p-3 bg-purple-50">
                <i class="pi pi-chart-bar text-4xl text-purple-500"></i>
              </div>
            </ng-template>
            <div class="text-center">
              <h3 class="text-xl font-semibold mb-2">Avaliações</h3>
              <p class="text-600 mb-3">
                Notas, frequência e desempenho acadêmico
              </p>
              <p-button 
                label="Acessar" 
                icon="pi pi-arrow-right" 
                routerLink="/avaliacoes"
                [text]="true">
              </p-button>
            </div>
          </p-card>
        </div>

        <div class="col-12 md:col-6 lg:col-4">
          <p-card class="h-full">
            <ng-template pTemplate="header">
              <div class="flex align-items-center justify-content-center p-3 bg-teal-50">
                <i class="pi pi-dollar text-4xl text-teal-500"></i>
              </div>
            </ng-template>
            <div class="text-center">
              <h3 class="text-xl font-semibold mb-2">Financeiro</h3>
              <p class="text-600 mb-3">
                Mensalidades, pagamentos e controle financeiro
              </p>
              <p-button 
                label="Acessar" 
                icon="pi pi-arrow-right" 
                routerLink="/financeiro"
                [text]="true">
              </p-button>
            </div>
          </p-card>
        </div>

        <div class="col-12 md:col-6 lg:col-4">
          <p-card class="h-full">
            <ng-template pTemplate="header">
              <div class="flex align-items-center justify-content-center p-3 bg-indigo-50">
                <i class="pi pi-file text-4xl text-indigo-500"></i>
              </div>
            </ng-template>
            <div class="text-center">
              <h3 class="text-xl font-semibold mb-2">Relatórios</h3>
              <p class="text-600 mb-3">
                Análises e relatórios detalhados
              </p>
              <p-button 
                label="Acessar" 
                icon="pi pi-arrow-right" 
                routerLink="/relatorios"
                [text]="true">
              </p-button>
            </div>
          </p-card>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 1rem;
    }

    .p-card {
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
      border: none;
      transition: all 0.3s ease;
      
      &:hover {
        transform: translateY(-2px);
        box-shadow: 0 8px 16px rgba(0, 0, 0, 0.15);
      }
      
      .p-card-header {
        border-radius: 6px 6px 0 0;
      }
      
      .p-card-body {
        padding: 1.5rem;
        height: 100%;
        display: flex;
        flex-direction: column;
      }
      
      .p-card-content {
        flex: 1;
        display: flex;
        flex-direction: column;
        justify-content: center;
      }
    }

    @media screen and (max-width: 768px) {
      .dashboard-container {
        padding: 0.5rem;
      }
      
      h1 {
        font-size: 2rem !important;
      }
      
      .text-xl {
        font-size: 1.1rem !important;
      }
    }
  `]
})
export class DashboardComponent {
  
}