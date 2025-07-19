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
import { CheckboxModule } from 'primeng/checkbox';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DividerModule } from 'primeng/divider';

// Store
import { Store } from '@ngrx/store';
import { AuthState } from '../../../../store/auth/auth.reducer';
import * as AuthActions from '../../../../store/auth/auth.actions';
import * as AuthSelectors from '../../../../store/auth/auth.selectors';

// Models
import { LoginRequest } from '../../models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CardModule,
    InputTextModule,
    PasswordModule,
    ButtonModule,
    CheckboxModule,
    MessageModule,
    MessagesModule,
    ProgressSpinnerModule,
    DividerModule
  ],
  template: `
    <div class="login-container">
      <div class="login-wrapper fade-in">
        <p-card styleClass="login-card modern-card">
          <ng-template pTemplate="header">
            <div class="login-header">
              <div class="logo-container">
                <i class="pi pi-graduation-cap logo-icon"></i>
              </div>
              <h2>Sistema de Gestão Escolar</h2>
              <p>Faça login para acessar o sistema</p>
            </div>
          </ng-template>

          <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="login-form">
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

            <!-- Password Field -->
            <div class="field">
              <label for="password" class="field-label">Senha</label>
              <span class="p-input-icon-left w-full">
                <i class="pi pi-lock"></i>
                <p-password
                  formControlName="password"
                  placeholder="Digite sua senha"
                  [toggleMask]="true"
                  [feedback]="false"
                  styleClass="w-full"
                  inputStyleClass="w-full"
                  [class.ng-invalid]="isFieldInvalid('password')"
                  autocomplete="current-password"
                ></p-password>
              </span>
              <small 
                class="p-error" 
                *ngIf="isFieldInvalid('password')"
              >
                {{ getFieldError('password') }}
              </small>
            </div>

            <!-- Remember Me -->
            <div class="field-checkbox">
              <p-checkbox
                formControlName="rememberMe"
                inputId="rememberMe"
                binary="true"
              ></p-checkbox>
              <label for="rememberMe" class="checkbox-label">Lembrar de mim</label>
            </div>

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
              label="Entrar"
              icon="pi pi-sign-in"
              styleClass="w-full login-button"
              [loading]="isLoading$ | async"
              [disabled]="loginForm.invalid || (isLoading$ | async)"
            ></p-button>

            <p-divider align="center">
              <span class="text-sm text-color-secondary">ou</span>
            </p-divider>

            <!-- Forgot Password Link -->
            <div class="text-center">
              <a 
                routerLink="/auth/forgot-password" 
                class="forgot-password-link"
              >
                Esqueceu sua senha?
              </a>
            </div>
          </form>
        </p-card>

        <!-- Footer -->
        <div class="login-footer">
          <p class="text-sm text-color-secondary text-center">
            © 2024 Sistema de Gestão Escolar. Todos os direitos reservados.
          </p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, var(--p-primary-500) 0%, var(--p-primary-700) 100%);
      padding: 1rem;
      position: relative;
      overflow: hidden;
    }

    .login-container::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><defs><pattern id="grain" width="100" height="100" patternUnits="userSpaceOnUse"><circle cx="25" cy="25" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="75" cy="75" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="50" cy="10" r="0.5" fill="rgba(255,255,255,0.05)"/></pattern></defs><rect width="100" height="100" fill="url(%23grain)"/></svg>');
      opacity: 0.3;
    }

    .login-wrapper {
      width: 100%;
      max-width: 420px;
      position: relative;
      z-index: 1;
    }

    .login-card {
      backdrop-filter: blur(20px);
      background: rgba(255, 255, 255, 0.95);
      border: 1px solid rgba(255, 255, 255, 0.2);
      box-shadow: 0 25px 50px rgba(0, 0, 0, 0.15);
      border-radius: 20px;
      overflow: hidden;
    }

    .login-header {
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
      color: var(--p-primary-500);
      background: linear-gradient(135deg, var(--p-primary-500), var(--p-primary-700));
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
      filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.1));
    }

    .login-header h2 {
      color: var(--p-surface-800);
      margin: 0 0 0.5rem 0;
      font-weight: 700;
      font-size: 1.75rem;
      letter-spacing: -0.025em;
    }

    .login-header p {
      color: var(--p-surface-600);
      margin: 0;
      font-size: 1rem;
      font-weight: 400;
    }

    .login-form {
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

    .field-checkbox {
      display: flex;
      align-items: center;
      margin-bottom: 2rem;
      padding: 0.5rem 0;
    }

    .checkbox-label {
      margin-left: 0.75rem;
      font-size: 0.95rem;
      color: var(--p-surface-600);
      font-weight: 500;
    }

    .login-button {
      height: 3.5rem;
      font-weight: 600;
      font-size: 1rem;
      margin-bottom: 1.5rem;
      background: linear-gradient(135deg, var(--p-primary-500), var(--p-primary-600));
      border: none;
      border-radius: 12px;
      transition: all 0.3s ease;
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }

    .login-button:hover:not(:disabled) {
      background: linear-gradient(135deg, var(--p-primary-600), var(--p-primary-700));
      transform: translateY(-2px);
      box-shadow: 0 10px 25px rgba(33, 150, 243, 0.3);
    }

    .login-button:active {
      transform: translateY(0);
    }

    .forgot-password-link {
      color: var(--p-primary-600);
      text-decoration: none;
      font-size: 0.95rem;
      font-weight: 600;
      transition: all 0.3s ease;
      position: relative;
      display: inline-block;
    }

    .forgot-password-link::after {
      content: '';
      position: absolute;
      bottom: -2px;
      left: 0;
      width: 0;
      height: 2px;
      background: var(--p-primary-600);
      transition: width 0.3s ease;
    }

    .forgot-password-link:hover {
      color: var(--p-primary-700);
      transform: translateY(-1px);
    }

    .forgot-password-link:hover::after {
      width: 100%;
    }

    .login-footer {
      margin-top: 2rem;
      text-align: center;
    }

    .login-footer p {
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
      border-color: var(--p-primary-500);
      box-shadow: 0 0 0 4px rgba(33, 150, 243, 0.1);
      transform: translateY(-1px);
    }

    .p-input-icon-left > i {
      color: var(--p-surface-500);
      font-size: 1.1rem;
      left: 1rem;
    }

    .p-checkbox {
      width: 1.25rem;
      height: 1.25rem;
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
      .login-container {
        padding: 1rem;
      }
      
      .login-wrapper {
        max-width: 100%;
      }
      
      .login-header {
        padding: 2rem 1.5rem 1.5rem;
      }
      
      .logo-icon {
        font-size: 3rem;
      }
      
      .login-header h2 {
        font-size: 1.5rem;
      }
      
      .login-form {
        padding: 1.5rem;
      }
    }

    @media (max-width: 480px) {
      .login-container {
        padding: 0.5rem;
      }
      
      .login-header {
        padding: 1.5rem 1rem 1rem;
      }
      
      .login-form {
        padding: 1rem;
      }
      
      .field {
        margin-bottom: 1.5rem;
      }
    }
  `]
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm: FormGroup;
  private destroy$ = new Subject<void>();

  // Store selectors
  isLoading$;
  error$;
  isAuthenticated$;

  private returnUrl: string = '/dashboard';

  constructor(
    private fb: FormBuilder,
    private store: Store<AuthState>,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.loginForm = this.createForm();
    
    // Initialize selectors in constructor
    this.isLoading$ = this.store.select(AuthSelectors.selectAuthLoading);
    this.error$ = this.store.select(AuthSelectors.selectAuthError);
    this.isAuthenticated$ = this.store.select(AuthSelectors.selectIsAuthenticated);
  }

  ngOnInit(): void {
    // Get return URL from route parameters or default to dashboard
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';

    // Redirect if already authenticated
    this.isAuthenticated$
      .pipe(takeUntil(this.destroy$))
      .subscribe(isAuthenticated => {
        if (isAuthenticated) {
          this.router.navigate([this.returnUrl]);
        }
      });

    // Clear any previous auth errors
    this.store.dispatch(AuthActions.clearAuthError());
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
      ]],
      password: ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(128)
      ]],
      rememberMe: [false]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      const request: LoginRequest = {
        email: this.loginForm.value.email.trim().toLowerCase(),
        senha: this.loginForm.value.password,
        lembrarMe: this.loginForm.value.rememberMe,
        agenteUsuario: navigator.userAgent,
        enderecoIp: '0.0.0.0' // Placeholder, conforme discutido
      };

      this.store.dispatch(AuthActions.login({ request }));
    } else {
      this.markFormGroupTouched();
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.loginForm.get(fieldName);
    
    if (field && field.errors && (field.dirty || field.touched)) {
      const errors = field.errors;
      
      if (errors['required']) {
        return fieldName === 'email' ? 'Email é obrigatório' : 'Senha é obrigatória';
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
    }
    
    return '';
  }

  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });
  }
}