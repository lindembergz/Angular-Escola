import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Card } from 'primeng/card';
import { Button } from 'primeng/button';

@Component({
  selector: 'app-lazy-loading-error',
  standalone: true,
  imports: [CommonModule, Card, Button],
  template: `
    <div class="error-container">
      <p-card>
        <div class="text-center p-4">
          <i class="pi pi-exclamation-triangle text-6xl text-red-500 mb-3"></i>
          <h3 class="text-xl font-semibold mb-2">Erro ao Carregar Módulo</h3>
          <p class="text-600 mb-4">
            {{ errorMessage || 'Ocorreu um erro ao carregar esta funcionalidade. Verifique sua conexão e tente novamente.' }}
          </p>
          <div class="flex gap-2 justify-content-center">
            <p-button 
              label="Tentar Novamente" 
              icon="pi pi-refresh" 
              (onClick)="onRetry()"
              severity="secondary">
            </p-button>
            <p-button 
              label="Voltar ao Dashboard" 
              icon="pi pi-home" 
              (onClick)="onGoHome()"
              [text]="true">
            </p-button>
          </div>
        </div>
      </p-card>
    </div>
  `,
  styles: [`
    .error-container {
      max-width: 600px;
      margin: 2rem auto;
      padding: 0 1rem;
    }
    
    .p-card {
      border: 1px solid var(--red-200);
      background: var(--red-50);
    }
  `]
})
export class LazyLoadingErrorComponent {
  @Input() errorMessage?: string;
  @Output() retry = new EventEmitter<void>();
  @Output() goHome = new EventEmitter<void>();

  onRetry() {
    this.retry.emit();
  }

  onGoHome() {
    this.goHome.emit();
  }
}