import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Card } from 'primeng/card';
import { Button } from 'primeng/button';

@Component({
  selector: 'app-em-desenvolvimento',
  standalone: true,
  imports: [CommonModule, RouterModule, Card, Button],
  template: `
    <div class="container">
      <p-card>
        <div class="text-center p-4">
          <i [class]="iconClass" class="text-6xl text-primary mb-3"></i>
          <h2>{{ titulo }}</h2>
          <p class="text-600 mb-4">{{ descricao }}</p>
          <p-button label="Voltar ao Dashboard" routerLink="/" icon="pi pi-home"></p-button>
        </div>
      </p-card>
    </div>
  `,
  styles: [`
    .container {
      max-width: 800px;
      margin: 2rem auto;
      padding: 0 1rem;
    }
  `]
})
export class EmDesenvolvimentoComponent {
  @Input() titulo = 'Módulo em Desenvolvimento';
  @Input() descricao = 'Esta funcionalidade está sendo desenvolvida e estará disponível em breve.';
  @Input() iconClass = 'pi pi-cog';
}