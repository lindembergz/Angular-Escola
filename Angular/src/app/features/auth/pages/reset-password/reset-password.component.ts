import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DividerModule } from 'primeng/divider';

// Services
import { NotificationService } from '../../../../core/services/notification.service';
import { AuthService } from '../../../../core/services/auth.service';

// Models
import { ResetPasswordRequest } from '../../models/auth.models';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    CardModule,
    InputTextModule,
    PasswordModule,
    ButtonModule,
    MessageModule,
    MessagesModule,
    ProgressSpinnerModule,
    DividerModule
  ],
  template: `
    <div class="reset-password-container">
      <div class="reset-password-wrapper fade-in">
        <p-card styleClass="reset-password-card modern-card">
          <ng-template pTemplate="header">
            <div class="reset-password-header">
              <div class="logo-container">
                <i class="pi pi-shield logo-icon"></i>
              </div>
              <h2>Redefinir Senha</h2>
              <p>Digite sua nova senha</p>
            </div>
          </ng-template>

          <form [formGroup]="resetPasswordForm" (ngSubmit)="onSubmit()" class="reset-password-form">
            <!-- Email Field (readonly) -->
            <div class="field">
              <label for="email" class="field-label">Email</label>
              <span class="p-input-icon-left w-full">
                <i class="pi pi-envelope"></i>
                <input
                  pInputText
                  id="email"
                  formControlName="email"
                  class="w-full"
                  readonly
                />
              </span>
            </div>

            <!-- New Password Field -->
            <div class="field">
              <label for="newPassword" class="field-label">Nova Senha</label>
              <span class="p-input-icon-left w-full">
                <i class="pi pi-lock"></i>
                <p-password
                  formControlName="newPassword"
                  placeholder="Digite sua nova senha"
                  [toggleMask]="true"
                  [feedback]="true"
                  styleClass="w-full"
                  inputStyleClass="w-full"
                  [class.ng-invalid]="isFieldInvalid('newPassword')"
                  autocomplete="new-password"
                  [promptLabel]="'Digite uma senha'"
                  [weakLabel]="'Fraca'"
                  [mediumLabel]="'Média'"
                  [strongLabel]="'Forte'"
                ></p-password>
              </span>
              <small 
                class="p-error" 
                *ngIf="isFieldInvalid('newPassword')"
              >
                {{ getFieldError('newPassword') }}
              </small>
            </div>

            <!-- Confirm Password Field -->
            <div class="field">
              <label for="confirmPassword" class="field-label">Confirmar Nova Senha</label>
              <span class="p-input-icon-left w-full">
                <i class="pi pi-lock"></i>
                <p-password
                  formControlName="confirmPassword"
                  placeholder="Confirme sua nova senha"
                  [toggleMask]="true"
                  [feedback]="false"
                  styleClass="w-full"
                  inputStyleClass="w-full"
                  [class.ng-invalid]="isFieldInvalid('confirmPassword')"
                  autocomplete="new-password"
                ></p-password>
              </span>
              <small 
                class="p-error" 
                *ngIf="isFieldInvalid('confirmPassword')"
              >
                {{ getFieldError('confirmPassword') }}
              </small>
            </div>

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

            <!-- Success Message -->
            <p-messages 
              *ngIf="showSuccessMessage" 
              severity="success" 
              styleClass="w-full"
            >
              <ng-template pTemplate>
                <div class="flex align-items-center">
                  <i class="pi pi-check-circle mr-2"></i>
                  <span>Senha redefinida com sucesso! Redirecionando para o login...</span>
                </div>
              </ng-template>
            </p-messages>

            <!-- Submit Button -->
            <p-button
              type="submit"
              label="Redefinir Senha"
              icon="pi pi-check"
              styleClass="w-full reset-password-button"
              [loading]="isLoading"
              [disabled]="resetPasswordForm.invalid || isLoading || showSuccessMessage"
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
        <div class="reset-password-footer">
          <p class="text-sm text-color-secondary text-center">
            © 2024 Sistema de Gestão Escolar. Todos os direitos reservados.
          </p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .reset-password-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, var(--p-green-500) 0%, var(--p-green-700) 100%);
      padding: 1rem;
      position: relative;
      overflow: hidden;
    }

    .reset-password-container::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><defs><pattern id="grain" width="100" height="100" patternUnits="userSpaceOnUse"><circle cx="25" cy="25" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="75" cy="75" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="50" cy="10" r="0.5" fill="rgba(255,255,255,0.05)"/></pattern></defs><rect width="100" height="100" fill="url(%23grain)"/></svg>');
      opacity: 0.3;
    }

    .reset-password-wrapper {
      width: 100%;
      max-width: 420px;
      position: relative;
      z-index: 1;
    }

    .reset-password-card {
      backdrop-filter: blur(20px);
      background: rgba(255, 255, 255, 0.95);
      border: 1px solid rgba(255, 255, 255, 0.2);
      box-shadow: 0 25px 50px rgba(0, 0, 0, 0.15);
      border-radius: 20px;
      overflow: hidden;
    }

    .reset-password-header {
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
      color: var(--p-green-500);
      background: linear-gradient(135deg, var(--p-green-500), var(--p-green-700));
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
      filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.1));
    }

    .reset-password-header h2 {
      color: var(--p-surface-800);
      margin: 0 0 0.5rem 0;
      font-weight: 700;
      font-size: 1.75rem;
      letter-spacing: -0.025em;
    }

    .reset-password-header p {
      color: var(--p-surface-600);
      margin: 0;
      font-size: 1rem;
      font-weight: 400;
    }

    .reset-password-form {
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

    .reset-password-button {
      height: 3.5rem;
      font-weight: 600;
      font-size: 1rem;
      margin-bottom: 1.5rem;
      background: linear-gradient(135deg, var(--p-green-500), var(--p-green-600));
      border: none;
      border-radius: 12px;
      transition: all 0.3s ease;
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }

    .reset-password-button:hover:not(:disabled) {
      background: linear-gradient(135deg, var(--p-green-600), var(--p-green-700));
      transform: translateY(-2px);
      box-shadow: 0 10px 25px rgba(76, 175, 80, 0.3);
    }

    .reset-password-button:active {
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

    .reset-password-footer {
      margin-top: 2rem;
      text-align: center;
    }

    .reset-password-footer p {
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

    .p-input-icon-left > input,
    .p-input-icon-left > .p-password > input {
      padding-left: 3rem;
      height: 3rem;
      border-radius: 12px;
      border: 2px solid var(--p-surface-300);
      background: var(--p-surface-0);
      transition: all 0.3s ease;
      font-size: 1rem;
    }

    .p-input-icon-left > input:focus,
    .p-input-icon-left > .p-password > input:focus {
      border-color: var(--p-green-500);
      box-shadow: 0 0 0 4px rgba(76, 175, 80, 0.1);
      transform: translateY(-1px);
    }

    .p-input-icon-left > input[readonly] {
      background: var(--p-surface-100);
      color: var(--p-surface-600);
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
      .reset-password-container {
        padding: 1rem;
      }
      
      .reset-password-wrapper {
        max-width: 100%;
      }
      
      .reset-password-header {
        padding: 2rem 1.5rem 1.5rem;
      }
      
      .logo-icon {
        font-size: 3rem;
      }
      
      .reset-password-header h2 {
        font-size: 1.5rem;
      }
      
      .reset-password-form {
        padding: 1.5rem;
      }
    }

    @media (max-width: 480px) {
      .reset-password-container {
        padding: 0.5rem;
      }
      
      .reset-password-header {
        padding: 1.5rem 1rem 1rem;
      }
      
      .reset-password-form {
        padding: 1rem;
      }
      
      .field {
        margin-bottom: 1.5rem;
      }
    }
  `]
})
export class ResetPasswordComponent implements OnInit, OnDestroy {
  resetPasswordForm: FormGroup;
  isLoading = false;
  showSuccessMessage = false;
  errorMessage: string | null = null;
  private destroy$ = new Subject<void>();

  private email: string = '';
  private token: string = '';

  constructor(
    private fb: FormBuilder,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService
  ) {
    this.resetPasswordForm = this.createForm();
  }

  ngOnInit(): void {
    // Get email and token from query parameters
    this.route.queryParams
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        this.email = params['email'] || '';
        this.token = params['token'] || '';
        
        if (!this.email || !this.token) {
          this.notificationService.error('Link de recuperação inválido');
          this.router.navigate(['/auth/forgot-password']);
          return;
        }
        
        // Set email in form
        this.resetPasswordForm.patchValue({ email: this.email });
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      newPassword: ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(128),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]).{8,}$/)
      ]],
      confirmPassword: ['', [Validators.required]]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  private passwordMatchValidator(form: FormGroup) {
    const newPassword = form.get('newPassword');
    const confirmPassword = form.get('confirmPassword');
    
    if (newPassword && confirmPassword && newPassword.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    
    return null;
  }

  async onSubmit(): Promise<void> {
    if (this.resetPasswordForm.valid && !this.isLoading) {
      this.isLoading = true;
      this.errorMessage = null;
      
      const request: ResetPasswordRequest = {
        email: this.email,
        token: this.token,
        novaSenha: this.resetPasswordForm.value.newPassword,
        confirmarSenha: this.resetPasswordForm.value.confirmPassword
      };

      try {
        // Call the API directly since we don't have NgRx actions for this yet
        await this.authService.resetPassword(request);
        
        this.showSuccessMessage = true;
        this.notificationService.success('Senha redefinida com sucesso!');
        
        // Redirect to login after 3 seconds
        setTimeout(() => {
          this.router.navigate(['/auth/login']);
        }, 3000);
        
      } catch (error: any) {
        this.errorMessage = error?.error?.detail || error?.message || 'Erro ao redefinir senha';
        this.notificationService.error(this.errorMessage || 'Erro desconhecido');
      } finally {
        this.isLoading = false;
      }
    } else {
      this.markFormGroupTouched();
    }
  }

  

  isFieldInvalid(fieldName: string): boolean {
    const field = this.resetPasswordForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.resetPasswordForm.get(fieldName);
    
    if (field && field.errors && (field.dirty || field.touched)) {
      const errors = field.errors;
      
      if (errors['required']) {
        return fieldName === 'email' ? 'Email é obrigatório' : 
               fieldName === 'newPassword' ? 'Nova senha é obrigatória' : 
               'Confirmação da senha é obrigatória';
      }
      
      if (errors['email']) {
        return 'Formato de email inválido';
      }
      
      if (errors['minlength']) {
        return `Mínimo de ${errors['minlength'].requiredLength} caracteres`;
      }
      
      if (errors['maxlength']) {
        return `Máximo de ${errors['maxlength'].requiredLength} caracteres`;
      }
      
      if (errors['pattern']) {
        return 'A senha deve conter pelo menos: 1 letra minúscula, 1 maiúscula, 1 número e 1 caractere especial';
      }
      
      if (errors['passwordMismatch']) {
        return 'As senhas não conferem';
      }
    }
    
    return '';
  }

  private markFormGroupTouched(): void {
    Object.keys(this.resetPasswordForm.controls).forEach(key => {
      const control = this.resetPasswordForm.get(key);
      control?.markAsTouched();
    });
  }
}