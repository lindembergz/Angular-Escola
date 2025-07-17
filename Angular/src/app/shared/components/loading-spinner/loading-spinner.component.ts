import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProgressSpinner } from 'primeng/progressspinner';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [CommonModule, ProgressSpinner],
  template: `
    <div class="loading-container" [class.overlay]="overlay">
      <div class="loading-content">
        <p-progressSpinner 
          [style]="{ width: size, height: size }" 
          strokeWidth="4"
          animationDuration="1s">
        </p-progressSpinner>
        <p *ngIf="message" class="loading-message">{{ message }}</p>
      </div>
    </div>
  `,
  styles: [`
    .loading-container {
      display: flex;
      justify-content: center;
      align-items: center;
      padding: 2rem;
      
      &.overlay {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(255, 255, 255, 0.8);
        z-index: 9999;
      }
    }
    
    .loading-content {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 1rem;
    }
    
    .loading-message {
      margin: 0;
      color: var(--text-color-secondary);
      font-size: 0.9rem;
      text-align: center;
    }
  `]
})
export class LoadingSpinnerComponent {
  @Input() message: string = '';
  @Input() size: string = '50px';
  @Input() overlay: boolean = false;
}