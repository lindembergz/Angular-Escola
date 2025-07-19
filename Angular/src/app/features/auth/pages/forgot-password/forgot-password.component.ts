import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Subject } from 'rxjs';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DividerModule } from 'primeng/divider';

// Store
import { Store } from '@ngrx/store';
import { AuthState } from '../../../../store/auth/auth.reducer';
import * as AuthSelectors from '../../../../store/auth/auth.selectors';

// Services
import { NotificationService } from '../../../../core/services/notification.service';
import { AuthService } from '../../../../core/services/auth.service';

// Models
import { ForgotPasswordRequest } from '../../models/auth.models';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    CardModule,
    InputTextModule,
    ButtonModule,
    MessageModule,
    MessagesModule,
    ProgressSpinnerModule,
    DividerModule
  ],
  template: `
    <div class="forgot-password-container">
      <div class="forgot-password-wrapper fade-in">
        <p-card styleClass="forgot-password-card modern-card">
          <ng-template pTemplate="header">
            <div class="forgot-password-header">
              <div class="logo-container">
                <i class="pi pi-key logo-icon"></i>
              </div>
              <h2>Recuperar Senha</h2>
              <p>Digite seu email para receber instruções de recuperação</p>
            </div>
          </ng-template>

          <form [formGroup]="forgotPasswordForm" (ngSubmit)="onSubmit()" class="forgot-password-form">
            <!-- Email Field -->
            <div class="field">
              <label for="email" class="field-label">Email</label>
              <span class="p-input-icon-left w-full">
                <i class="pi pi-envelope"></i>
                <input
                  pInputText
                  id="email"
                  formControlName="email"
                  placeholder="Digite seu email"
                  class="w-full"
                  [class.ng-invalid]="isFieldInvalid('email')"
                  autocomplete="email"
                />
              </span>
              <small 
                class="p-error" 
                *ngIf="isFieldInvalid('email')"
              >
                {{ getFieldError('email') }}
              </small>
            </div>

            <!-- Success Message -->
            <p-messages 
              *ngIf="showSuccessMessage" 
              severity="success" 
              styleClass="w-full"
            >
              <ng-template pTemplate>
                <div class="flex align-items-center">
                  <i class="pi pi-check-circle mr-2"></i>
                  <span>Se o email existir, você receberá instruções para recuperação da senha.</span>
                </div>
              </ng-template>
            </p-messages>

            <!-- Error Messages -->
            <p-messages 
              *ngIf="error$ | async as error" 
              severity="error" 
              styleClass="w-full"
            >
              <ng-template pTemplate>
                <div class="flex align-items-center">
                  <i class="pi pi-exclamation-triangle mr-2"></i>
                  <span>{{ error }}</span>
                </div>
              </ng-template>
            </p-messages>

            <!-- Submit Button -->
            <p-button
              type="submit"
              label="Enviar Instruções"
              icon="pi pi-send"
              styleClass="w-full forgot-password-button"
              [loading]="isLoading"
              [disabled]="forgotPasswordForm.invalid || isLoading || showSuccessMessage"
            ></p-button>

            <p-divider align="center">
              <span class="text-sm text-color-secondary">ou</span>
            </p-divider>

            <!-- Back to Login Link -->
            <div class="text-center">
              <a 
                routerLink="/auth/login" 
                class="back-to-login-link"
              >
                <i class="pi pi-arrow-left mr-2"></i>
                Voltar ao Login
              </a>
            </div>
          </form>
        </p-card>

        <!-- Footer -->
        <div class="forgot-password-footer">
          <p class="text-sm text-color-secondary text-center">
            © 2024 Sistema de Gestão Escolar. Todos os direitos reservados.
          </p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .forgot-password-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, var(--p-primary-500) 0%, var(--p-primary-700) 100%);
      padding: 1rem;
      position: relative;
      overflow: hidden;
    }

    .forgot-password-container::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><defs><pattern id="grain" width="100" height="100" patternUnits="userSpaceOnUse"><circle cx="25" cy="25" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="75" cy="75" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="50" cy="10" r="0.5" fill="rgba(255,255,255,0.05)"/></pattern></defs><rect width="100" height="100" fill="url(%23grain)"/></svg>');
      opacity: 0.3;
    }

    .forgot-password-wrapper {
      width: 100%;
      max-width: 420px;
      position: relative;
      z-index: 1;
    }

    .forgot-password-card {
      backdrop-filter: blur(20px);
      background: rgba(255, 255, 255, 0.95);
      border: 1px solid rgba(255, 255, 255, 0.2);
      box-shadow: 0 25px 50px rgba(0, 0, 0, 0.15);
      border-radius: 20px;
      overflow: hidden;
    }

    .forgot-password-header {
      text-align: center;
      padding: 3rem 2rem 2rem;
      background: linear-gradient(135deg, rgba(255, 255, 255, 0.1), rgba(255, 255, 255, 0.05));
      position: relative;
    }

    .logo-container {
      margin-bottom: 1.5rem;
    }

    .logo-icon {
      font-size: 4rem;
      color: var(--p-orange-500);
      background: linear-gradient(135deg, var(--p-orange-500), var(--p-orange-700));
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
      filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.1));
    }

    .forgot-password-header h2 {
      color: var(--p-surface-800);
      margin: 0 0 0.5rem 0;
      font-weight: 700;
      font-size: 1.75rem;
      letter-spacing: -0.025em;
    }

    .forgot-password-header p {
      color: var(--p-surface-600);
      margin: 0;
      font-size: 1rem;
      font-weight: 400;
    }

    .forgot-password-form {
      padding: 2rem;
    }

    .field {
      margin-bottom: 1.75rem;
    }

    .field-label {
      display: block;
      margin-bottom: 0.75rem;
      font-weight: 600;
      color: var(--p-surface-700);
      font-size: 0.9rem;
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }

    .forgot-password-button {
      height: 3.5rem;
      font-weight: 600;
      font-size: 1rem;
      margin-bottom: 1.5rem;
      background: linear-gradient(135deg, var(--p-orange-500), var(--p-orange-600));
      border: none;
      border-radius: 12px;
      transition: all 0.3s ease;
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }

    .forgot-password-button:hover:not(:disabled) {
      background: linear-gradient(135deg, var(--p-orange-600), var(--p-orange-700));
      transform: translateY(-2px);
      box-shadow: 0 10px 25px rgba(255, 152, 0, 0.3);
    }

    .forgot-password-button:active {
      transform: translateY(0);
    }

    .back-to-login-link {
      color: var(--p-primary-600);
      text-decoration: none;
      font-size: 0.95rem;
      font-weight: 600;
      transition: all 0.3s ease;
      position: relative;
      display: inline-flex;
      align-items: center;
    }

    .back-to-login-link::after {
      content: '';
      position: absolute;
      bottom: -2px;
      left: 0;
      width: 0;
      height: 2px;
      background: var(--p-primary-600);
      transition: width 0.3s ease;
    }

    .back-to-login-link:hover {
      color: var(--p-primary-700);
      transform: translateY(-1px);
    }

    .back-to-login-link:hover::after {
      width: 100%;
    }

    .forgot-password-footer {
      margin-top: 2rem;
      text-align: center;
    }

    .forgot-password-footer p {
      color: rgba(255, 255, 255, 0.8);
      font-size: 0.85rem;
      font-weight: 400;
    }

    .p-error {
      display: block;
      margin-top: 0.5rem;
      font-size: 0.85rem;
      font-weight: 500;
    }

    .p-input-icon-left > input {
      padding-left: 3rem;
      height: 3rem;
      border-radius: 12px;
      border: 2px solid var(--p-surface-300);
      background: var(--p-surface-0);
      transition: all 0.3s ease;
      font-size: 1rem;
    }

    .p-input-icon-left > input:focus {
      border-color: var(--p-orange-500);
      box-shadow: 0 0 0 4px rgba(255, 152, 0, 0.1);
      transform: translateY(-1px);
    }

    .p-input-icon-left > i {
      color: var(--p-surface-500);
      font-size: 1.1rem;
      left: 1rem;
    }

    .p-divider {
      margin: 2rem 0;
    }

    .p-divider .p-divider-content {
      background: transparent;
      color: var(--p-surface-500);
      font-size: 0.85rem;
      font-weight: 500;
    }

    @media (max-width: 768px) {
      .forgot-password-container {
        padding: 1rem;
      }
      
      .forgot-password-wrapper {
        max-width: 100%;
      }
      
      .forgot-password-header {
        padding: 2rem 1.5rem 1.5rem;
      }
      
      .logo-icon {
        font-size: 3rem;
      }
      
      .forgot-password-header h2 {
        font-size: 1.5rem;
      }
      
      .forgot-password-form {
        padding: 1.5rem;
      }
    }

    @media (max-width: 480px) {
      .forgot-password-container {
        padding: 0.5rem;
      }
      
      .forgot-password-header {
        padding: 1.5rem 1rem 1rem;
      }
      
      .forgot-password-form {
        padding: 1rem;
      }
      
      .field {
        margin-bottom: 1.5rem;
      }
    }
  `]
})
export class ForgotPasswordComponent implements OnInit, OnDestroy {
  forgotPasswordForm: FormGroup;
  isLoading = false;
  showSuccessMessage = false;
  private destroy$ = new Subject<void>();

  // Store selectors
  error$;

  constructor(
    private fb: FormBuilder,
    private store: Store<AuthState>,
    private notificationService: NotificationService,
    private router: Router,
    private authService: AuthService
  ) {
    this.forgotPasswordForm = this.createForm();
    
    // Initialize selectors
    this.error$ = this.store.select(AuthSelectors.selectAuthError);
  }

  ngOnInit(): void {
    // Clear any previous auth errors
    // this.store.dispatch(AuthActions.clearAuthError());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      email: ['', [
        Validators.required,
        Validators.email,
        Validators.maxLength(254)
      ]]
    });
  }

  async onSubmit(): Promise<void> {
    if (this.forgotPasswordForm.valid && !this.isLoading) {
      this.isLoading = true;
      
      const request: ForgotPasswordRequest = {
        email: this.forgotPasswordForm.value.email.trim().toLowerCase(),
        urlCallback: `${window.location.origin}/auth/reset-password`
      };

      try {
        // Call the API directly since we don't have NgRx actions for this yet
        await this.authService.forgotPassword(request);
        
        this.showSuccessMessage = true;
        this.notificationService.success('Instruções enviadas para seu email');
        
        // Redirect to login after 5 seconds
        setTimeout(() => {
          this.router.navigate(['/auth/login']);
        }, 5000);
        
      } catch (error: any) {
        const errorMessage = error?.error?.detail || error?.message || 'Erro ao enviar instruções';
        this.notificationService.error(errorMessage);
      } finally {
        this.isLoading = false;
      }
    } else {
      this.markFormGroupTouched();
    }
  }

  

  isFieldInvalid(fieldName: string): boolean {
    const field = this.forgotPasswordForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.forgotPasswordForm.get(fieldName);
    
    if (field && field.errors && (field.dirty || field.touched)) {
      const errors = field.errors;
      
      if (errors['required']) {
        return 'Email é obrigatório';
      }
      
      if (errors['email']) {
        return 'Formato de email inválido';
      }
      
      if (errors['maxlength']) {
        return `Máximo de ${errors['maxlength'].requiredLength} caracteres`;
      }
    }
    
    return '';
  }

  private markFormGroupTouched(): void {
    Object.keys(this.forgotPasswordForm.controls).forEach(key => {
      const control = this.forgotPasswordForm.get(key);
      control?.markAsTouched();
    });
  }
}