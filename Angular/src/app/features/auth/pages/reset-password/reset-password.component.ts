import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

// Services
import { AuthService } from '../../../../core/services/auth.service';
import { NotificationService } from '../../../../core/services/notification.service';

// Models
import { ResetPasswordRequest } from '../../models/auth.models';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CardModule,
    InputTextModule,
    PasswordModule,
    ButtonModule,
    MessageModule,
    MessagesModule,
    ProgressSpinnerModule
  ],
  template: `
    <div class="reset-password-container">
      <div class="reset-password-wrapper">
        <p-card styleClass="reset-password-card">
          <ng-template pTemplate="header">
            <div class="reset-password-header">
              <img src="assets/images/logo.png" alt="Logo" class="logo" />
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

            <!-- Submit Button -->
            <p-button
              type="submit"
              label="Redefinir Senha"
              icon="pi pi-check"
              styleClass="w-full submit-button"
              [loading]="isLoading"
              [disabled]="resetPasswordForm.invalid || isLoading"
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
      background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-color-text) 100%);
      padding: 1rem;
    }

    .reset-password-wrapper {
      width: 100%;
      max-width: 400px;
    }

    .reset-password-card {
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
      border: none;
      border-radius: 1rem;
    }

    .reset-password-header {
      text-align: center;
      padding: 2rem 1rem 1rem;
    }

    .logo {
      height: 60px;
      margin-bottom: 1rem;
    }

    .reset-password-header h2 {
      color: var(--primary-color);
      margin: 0 0 0.5rem 0;
      font-weight: 600;
    }

    .reset-password-header p {
      color: var(--text-color-secondary);
      margin: 0;
      font-size: 0.9rem;
    }

    .reset-password-form {
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

    .reset-password-footer {
      margin-top: 2rem;
    }

    .p-error {
      display: block;
      margin-top: 0.25rem;
    }

    .p-input-icon-left > input {
      padding-left: 2.5rem;
    }

    .p-input-icon-left > .p-password > input {
      padding-left: 2.5rem;
    }

    @media (max-width: 768px) {
      .reset-password-container {
        padding: 0.5rem;
      }
      
      .reset-password-header {
        padding: 1.5rem 1rem 0.5rem;
      }
      
      .logo {
        height: 50px;
      }
    }
  `]
})
export class ResetPasswordComponent implements OnInit, OnDestroy {
  resetPasswordForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  private destroy$ = new Subject<void>();
  private email = '';
  private token = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.resetPasswordForm = this.createForm();
  }

  ngOnInit(): void {
    // Get email and token from query parameters
    this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe(params => {
      this.email = params['email'] || '';
      this.token = params['token'] || '';
      
      if (!this.email || !this.token) {
        this.errorMessage = 'Link de recuperação inválido ou expirado';
        return;
      }
      
      this.resetPasswordForm.patchValue({ email: this.email });
    });

    // Clear any previous messages
    this.errorMessage = '';
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
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/)
      ]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  private passwordMatchValidator(form: FormGroup) {
    const password = form.get('newPassword');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    
    return null;
  }

  onSubmit(): void {
    if (this.resetPasswordForm.valid && !this.isLoading) {
      this.isLoading = true;
      this.errorMessage = '';

      const request: ResetPasswordRequest = {
        email: this.email,
        token: this.token,
        newPassword: this.resetPasswordForm.value.newPassword,
        confirmPassword: this.resetPasswordForm.value.confirmPassword
      };

      this.authService.resetPassword(request)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.isLoading = false;
            this.notificationService.success('Senha redefinida com sucesso!');
            this.router.navigate(['/auth/login']);
          },
          error: (error) => {
            this.isLoading = false;
            this.errorMessage = error || 'Erro ao redefinir senha. Tente novamente.';
          }
        });
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
        switch (fieldName) {
          case 'newPassword': return 'Nova senha é obrigatória';
          case 'confirmPassword': return 'Confirmação de senha é obrigatória';
          default: return 'Campo obrigatório';
        }
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
        return 'As senhas não coincidem';
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