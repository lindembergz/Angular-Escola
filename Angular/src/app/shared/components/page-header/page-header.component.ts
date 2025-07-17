import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Breadcrumb } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-page-header',
  standalone: true,
  imports: [CommonModule, RouterModule, Breadcrumb],
  template: `
    <div class="page-header">
      <div class="page-header-content">
        <div class="page-title-section">
          <div class="page-icon" *ngIf="icon">
            <i [class]="icon"></i>
          </div>
          <div class="page-title-content">
            <h1 class="page-title">{{ title }}</h1>
            <p class="page-subtitle" *ngIf="subtitle">{{ subtitle }}</p>
          </div>
        </div>
        
        <div class="page-actions" *ngIf="showActions">
          <ng-content select="[slot=actions]"></ng-content>
        </div>
      </div>
      
      <div class="page-breadcrumb" *ngIf="breadcrumbItems && breadcrumbItems.length > 0">
        <p-breadcrumb [model]="breadcrumbItems" [home]="homeItem"></p-breadcrumb>
      </div>
    </div>
  `,
  styles: [`
    .page-header {
      background: white;
      border-bottom: 1px solid var(--surface-border);
      padding: 1.5rem 2rem;
      margin-bottom: 1.5rem;
    }
    
    .page-header-content {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
    }
    
    .page-title-section {
      display: flex;
      align-items: center;
      gap: 1rem;
    }
    
    .page-icon {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 3rem;
      height: 3rem;
      border-radius: 50%;
      background: var(--primary-color);
      color: white;
      
      i {
        font-size: 1.5rem;
      }
    }
    
    .page-title-content {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }
    
    .page-title {
      margin: 0;
      font-size: 1.75rem;
      font-weight: 600;
      color: var(--text-color);
    }
    
    .page-subtitle {
      margin: 0;
      font-size: 1rem;
      color: var(--text-color-secondary);
    }
    
    .page-actions {
      display: flex;
      gap: 0.5rem;
    }
    
    .page-breadcrumb {
      :host ::ng-deep .p-breadcrumb {
        background: transparent;
        border: none;
        padding: 0;
      }
    }
    
    @media screen and (max-width: 768px) {
      .page-header {
        padding: 1rem;
      }
      
      .page-header-content {
        flex-direction: column;
        align-items: flex-start;
        gap: 1rem;
      }
      
      .page-title {
        font-size: 1.5rem;
      }
      
      .page-actions {
        width: 100%;
        justify-content: flex-end;
      }
    }
  `]
})
export class PageHeaderComponent {
  @Input() title: string = '';
  @Input() subtitle: string = '';
  @Input() icon: string = '';
  @Input() showActions: boolean = true;
  @Input() breadcrumbItems: MenuItem[] = [];
  
  homeItem: MenuItem = {
    icon: 'pi pi-home',
    routerLink: '/'
  };
}