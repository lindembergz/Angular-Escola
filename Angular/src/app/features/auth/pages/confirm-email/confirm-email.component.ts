import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DividerModule } from 'primeng/divider';

// Services
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../../../core/services/notification.service';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    CardModule,
    ButtonModule,
    MessageModule,
    MessagesModule,
    ProgressSpinnerModule,
    DividerModule
  ],
  template: `
    <div class="confirm-email-container">
      <div class="confirm-email-wrapper fade-in">
        <p-card styleClass="confirm-email-card modern-card">
          <ng-template pTemplate="header">
            <div class="confirm-email-header">
              <div class="logo-container">
                <i class="pi" [class]="getIconClass()" [style.color]="getIconColor()"></i>
              </div>
              <h2>{{ getTitle() }}</h2>
              <p>{{ getSubtitle() }}</p>
            </div>
          </ng-template>

          <div class="confirm-email-content">
            <!-- Loading State -->
            <div *ngIf="isLoading" class="text-center">
              <p-progressSpinner
                styleClass="w-4rem h-4rem"
                strokeWidth="4"
                fill="transparent"
                animationDuration="1s"
              ></p-progressSpinner>
              <p class="mt-3 text-color-secondary">Confirmando seu email...</p>
            </div>

            <!-- Success State -->
            <div *ngIf="!isLoading && isSuccess" class="text-center">
              <p-messages severity="success" styleClass="w-full">
                <ng-template pTemplate>
                  <div class="flex align-items-center">
                    <i class="pi pi-check-circle mr-2"></i>
                    <span>Email confirmado com sucesso!</span>
                  </div>
                </ng-template>
              </p-messages>

              <div class="success-actions mt-4">
                <p-button
                  label="Fazer Login"
                  icon="pi pi-sign-in"
                  styleClass="w-full confirm-email-button success-button"
                  (click)="goToLogin()"
                ></p-button>
              </div>
            </div>

            <!-- Error State -->
            <div *ngIf="!isLoading && !isSuccess" class="text-center">
              <p-messages severity="error" styleClass="w-full">
                <ng-template pTemplate>
                  <div class="flex align-items-center">
                    <i class="pi pi-exclamation-triangle mr-2"></i>
                    <span>{{ errorMessage }}</span>
                  </div>
                </ng-template>
              </p-messages>

              <div class="error-actions mt-4">
                <p-button
                  label="Reenviar Confirmação"
                  icon="pi pi-send"
                  styleClass="w-full confirm-email-button error-button mb-3"
                  [loading]="isResending"
                  [disabled]="isResending"
                  (click)="resendConfirmation()"
                ></p-button>

                <p-divider align="center">
                  <span class="text-sm text-color-secondary">ou</span>
                </p-divider>

                <a
                  routerLink="/auth/login"
                  class="back-to-login-link"
                >
                  <i class="pi pi-arrow-left mr-2"></i>
                  Voltar ao Login
                </a>
              </div>
            </div>
          </div>
        </p-card>

        <!-- Footer -->
        <div class="confirm-email-footer">
          <p class="text-sm text-color-secondary text-center">
            © 2024 Sistema de Gestão Escolar. Todos os direitos reservados.
          </p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .confirm-email-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, var(--p-cyan-500) 0%, var(--p-cyan-700) 100%);
      padding: 1rem;
      position: relative;
      overflow: hidden;
    }

    .confirm-email-container::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><defs><pattern id="grain" width="100" height="100" patternUnits="userSpaceOnUse"><circle cx="25" cy="25" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="75" cy="75" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="50" cy="10" r="0.5" fill="rgba(255,255,255,0.05)"/></pattern></defs><rect width="100" height="100" fill="url(%23grain)"/></svg>');
      opacity: 0.3;
    }

    .confirm-email-wrapper {
      width: 100%;
      max-width: 420px;
      position: relative;
      z-index: 1;
    }

    .confirm-email-card {
      backdrop-filter: blur(20px);
      background: rgba(255, 255, 255, 0.95);
      border: 1px solid rgba(255, 255, 255, 0.2);
      box-shadow: 0 25px 50px rgba(0, 0, 0, 0.15);
      border-radius: 20px;
      overflow: hidden;
    }

    .confirm-email-header {
      text-align: center;
      padding: 3rem 2rem 2rem;
      background: linear-gradient(135deg, rgba(255, 255, 255, 0.1), rgba(255, 255, 255, 0.05));
      position: relative;
    }

    .logo-container {
      margin-bottom: 1.5rem;
    }

    .logo-container i {
      font-size: 4rem;
      filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.1));
    }

    .confirm-email-header h2 {
      color: var(--p-surface-800);
      margin: 0 0 0.5rem 0;
      font-weight: 700;
      font-size: 1.75rem;
      letter-spacing: -0.025em;
    }

    .confirm-email-header p {
      color: var(--p-surface-600);
      margin: 0;
      font-size: 1rem;
      font-weight: 400;
    }

    .confirm-email-content {
      padding: 2rem;
    }

    .confirm-email-button {
      height: 3.5rem;
      font-weight: 600;
      font-size: 1rem;
      border-radius: 12px;
      transition: all 0.3s ease;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      border: none;
    }

    .success-button {
      background: linear-gradient(135deg, var(--p-green-500), var(--p-green-600));
    }

    .success-button:hover:not(:disabled) {
      background: linear-gradient(135deg, var(--p-green-600), var(--p-green-700));
      transform: translateY(-2px);
      box-shadow: 0 10px 25px rgba(76, 175, 80, 0.3);
    }

    .error-button {
      background: linear-gradient(135deg, var(--p-orange-500), var(--p-orange-600));
    }

    .error-button:hover:not(:disabled) {
      background: linear-gradient(135deg, var(--p-orange-600), var(--p-orange-700));
      transform: translateY(-2px);
      box-shadow: 0 10px 25px rgba(255, 152, 0, 0.3);
    }

    .confirm-email-button:active {
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

    .confirm-email-footer {
      margin-top: 2rem;
      text-align: center;
    }

    .confirm-email-footer p {
      color: rgba(255, 255, 255, 0.8);
      font-size: 0.85rem;
      font-weight: 400;
    }

    .p-divider {
      margin: 1.5rem 0;
    }

    .p-divider .p-divider-content {
      background: transparent;
      color: var(--p-surface-500);
      font-size: 0.85rem;
      font-weight: 500;
    }

    @media (max-width: 768px) {
      .confirm-email-container {
        padding: 1rem;
      }

      .confirm-email-wrapper {
        max-width: 100%;
      }

      .confirm-email-header {
        padding: 2rem 1.5rem 1.5rem;
      }

      .logo-container i {
        font-size: 3rem;
      }

      .confirm-email-header h2 {
        font-size: 1.5rem;
      }

      .confirm-email-content {
        padding: 1.5rem;
      }
    }

    @media (max-width: 480px) {
      .confirm-email-container {
        padding: 0.5rem;
      }

      .confirm-email-header {
        padding: 1.5rem 1rem 1rem;
      }

      .confirm-email-content {
        padding: 1rem;
      }
    }
  `]
})
export class ConfirmEmailComponent implements OnInit, OnDestroy {
  isLoading = true;
  isSuccess = false;
  isResending = false;
  errorMessage: string = '';
  private destroy$ = new Subject<void>();

  private email: string = '';
  private token: string = '';

  constructor(
    private authService: AuthService,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    // Get email and token from query parameters
    this.route.queryParams
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        this.email = params['email'] || '';
        this.token = params['token'] || '';

        if (!this.email || !this.token) {
          this.handleError('Link de confirmação inválido');
          return;
        }

        // Automatically confirm email
        this.confirmEmail();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private async confirmEmail(): Promise<void> {
    try {
      await this.authService.confirmEmail(this.email, this.token).toPromise();

      this.isSuccess = true;
      this.notificationService.success('Email confirmado com sucesso!');

    } catch (error: any) {
      this.handleError(error?.error?.detail || error?.message || 'Erro ao confirmar email');
    } finally {
      this.isLoading = false;
    }
  }

  async resendConfirmation(): Promise<void> {
    if (this.isResending) return;

    this.isResending = true;

    try {
      await this.authService.resendEmailConfirmation(this.email).toPromise();

      this.notificationService.success('Nova confirmação enviada para seu email');

    } catch (error: any) {
      const errorMessage = error?.error?.detail || error?.message || 'Erro ao reenviar confirmação';
      this.notificationService.error(errorMessage);
    } finally {
      this.isResending = false;
    }
  }

  private handleError(message: string): void {
    this.errorMessage = message;
    this.isSuccess = false;
    this.isLoading = false;
    this.notificationService.error(message);
  }

  goToLogin(): void {
    this.router.navigate(['/auth/login']);
  }

  getIconClass(): string {
    if (this.isLoading) {
      return 'pi-spin pi-spinner';
    } else if (this.isSuccess) {
      return 'pi-check-circle';
    } else {
      return 'pi-exclamation-triangle';
    }
  }

  getIconColor(): string {
    if (this.isLoading) {
      return 'var(--p-cyan-500)';
    } else if (this.isSuccess) {
      return 'var(--p-green-500)';
    } else {
      return 'var(--p-orange-500)';
    }
  }

  getTitle(): string {
    if (this.isLoading) {
      return 'Confirmando Email';
    } else if (this.isSuccess) {
      return 'Email Confirmado!';
    } else {
      return 'Erro na Confirmação';
    }
  }

  getSubtitle(): string {
    if (this.isLoading) {
      return 'Aguarde enquanto confirmamos seu email';
    } else if (this.isSuccess) {
      return 'Seu email foi confirmado com sucesso';
    } else {
      return 'Não foi possível confirmar seu email';
    }
  }
}
