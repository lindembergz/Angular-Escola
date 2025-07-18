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
import { LoginCredentials } from '../../models/auth.models';

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
      <div class="login-wrapper">
        <p-card styleClass="login-card">
          <ng-template pTemplate="header">
            <div class="login-header">
              <img src="assets/images/logo.png" alt="Logo" class="logo" />
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
      background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-color-text) 100%);
      padding: 1rem;
    }

    .login-wrapper {
      width: 100%;
      max-width: 400px;
    }

    .login-card {
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
      border: none;
      border-radius: 1rem;
    }

    .login-header {
      text-align: center;
      padding: 2rem 1rem 1rem;
    }

    .logo {
      height: 60px;
      margin-bottom: 1rem;
    }

    .login-header h2 {
      color: var(--primary-color);
      margin: 0 0 0.5rem 0;
      font-weight: 600;
    }

    .login-header p {
      color: var(--text-color-secondary);
      margin: 0;
      font-size: 0.9rem;
    }

    .login-form {
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

    .field-checkbox {
      display: flex;
      align-items: center;
      margin-bottom: 1.5rem;
    }

    .checkbox-label {
      margin-left: 0.5rem;
      font-size: 0.9rem;
      color: var(--text-color);
    }

    .login-button {
      height: 3rem;
      font-weight: 600;
      margin-bottom: 1rem;
    }

    .forgot-password-link {
      color: var(--primary-color);
      text-decoration: none;
      font-size: 0.9rem;
      font-weight: 500;
      transition: color 0.2s;
    }

    .forgot-password-link:hover {
      color: var(--primary-color-text);
      text-decoration: underline;
    }

    .login-footer {
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
      .login-container {
        padding: 0.5rem;
      }
      
      .login-header {
        padding: 1.5rem 1rem 0.5rem;
      }
      
      .logo {
        height: 50px;
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
      const credentials: LoginCredentials = {
        email: this.loginForm.value.email.trim().toLowerCase(),
        password: this.loginForm.value.password,
        rememberMe: this.loginForm.value.rememberMe
      };

      this.store.dispatch(AuthActions.login({ credentials }));
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