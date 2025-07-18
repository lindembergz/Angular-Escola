import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';

import { Subject, takeUntil } from 'rxjs';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

// Services
import { AuthService } from '../../../../core/services/auth.service';
import { NotificationService } from '../../../../core/services/notification.service';

// Models
import { ForgotPasswordRequest } from '../../models/auth.models';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CardModule,
    InputTextModule,
    ButtonModule,
    MessageModule,
    MessagesModule,
    ProgressSpinnerModule
  ],
  template: `
    <div class="forgot-password-container">
      <div class="forgot-password-wrapper">
        <p-card styleClass="forgot-password-card">
          <ng-template pTemplate="header">
            <div class="forgot-password-header">
              <img src="assets/images/logo.png" alt="Logo" class="logo" />
              <h2>Recuperar Senha</h2>
              <p>Digite seu email para receber as instruções de recuperação</p>
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
                  <span>
                    Se o email existir em nosso sistema, você receberá as instruções 
                    para recuperação da senha em alguns minutos.
                  </span>
                </div>
              </ng-template>
            </p-messages>

            <!-- Error Messages -->
            <p-messages 
              *ngIf="errorMessage" 
              severity="error" 
              styleClass="w-full"
            >
              <ng-template pTemplate>
                <div class="flex align-items-center">
                  <i class="pi pi-exclamation-triangle mr-2"></i>
                  <span>{{ errorMessage }}</span>
                </div>
              </ng-template>
            </p-messages>

            <!-- Submit Button -->
            <p-button
              type="submit"
              label="Enviar Instruções"
              icon="pi pi-send"
              styleClass="w-full submit-button"
              [loading]="isLoading"
              [disabled]="forgotPasswordForm.invalid || isLoading || showSuccessMessage"
            ></p-button>

            <!-- Back to Login -->
            <div class="text-center mt-3">
              <a 
                routerLink="/auth/login" 
                class="back-to-login-link"
              >
                <i class="pi pi-arrow-left mr-1"></i>
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
      background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-color-text) 100%);
      padding: 1rem;
    }

    .forgot-password-wrapper {
      width: 100%;
      max-width: 400px;
    }

    .forgot-password-card {
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
      border: none;
      border-radius: 1rem;
    }

    .forgot-password-header {
      text-align: center;
      padding: 2rem 1rem 1rem;
    }

    .logo {
      height: 60px;
      margin-bottom: 1rem;
    }

    .forgot-password-header h2 {
      color: var(--primary-color);
      margin: 0 0 0.5rem 0;
      font-weight: 600;
    }

    .forgot-password-header p {
      color: var(--text-color-secondary);
      margin: 0;
      font-size: 0.9rem;
    }

    .forgot-password-form {
      padding: 1rem;
    }

    .field {
      margin-bottom: 1.5rem;
    }

    .field-label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: var(--text-color);
    }

    .submit-button {
      height: 3rem;
      font-weight: 600;
      margin-bottom: 1rem;
    }

    .back-to-login-link {
      color: var(--primary-color);
      text-decoration: none;
      font-size: 0.9rem;
      font-weight: 500;
      transition: color 0.2s;
      display: inline-flex;
      align-items: center;
    }

    .back-to-login-link:hover {
      color: var(--primary-color-text);
      text-decoration: underline;
    }

    .forgot-password-footer {
      margin-top: 2rem;
    }

    .p-error {
      display: block;
      margin-top: 0.25rem;
    }

    .p-input-icon-left > input {
      padding-left: 2.5rem;
    }

    @media (max-width: 768px) {
      .forgot-password-container {
        padding: 0.5rem;
      }
      
      .forgot-password-header {
        padding: 1.5rem 1rem 0.5rem;
      }
      
      .logo {
        height: 50px;
      }
    }
  `]
})
export class ForgotPasswordComponent implements OnInit, OnDestroy {
  forgotPasswordForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  showSuccessMessage = false;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private notificationService: NotificationService
  ) {
    this.forgotPasswordForm = this.createForm();
  }

  ngOnInit(): void {
    // Clear any previous messages
    this.errorMessage = '';
    this.showSuccessMessage = false;
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

  onSubmit(): void {
    if (this.forgotPasswordForm.valid && !this.isLoading) {
      this.isLoading = true;
      this.errorMessage = '';
      this.showSuccessMessage = false;

      const request: ForgotPasswordRequest = {
        email: this.forgotPasswordForm.value.email.trim().toLowerCase(),
        callbackUrl: `${window.location.origin}/auth/reset-password`
      };

      this.authService.forgotPassword(request)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.isLoading = false;
            this.showSuccessMessage = true;
            this.notificationService.success(
              'Instruções enviadas! Verifique seu email.'
            );
          },
          error: (error) => {
            this.isLoading = false;
            this.errorMessage = error || 'Erro ao enviar instruções. Tente novamente.';
          }
        });
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