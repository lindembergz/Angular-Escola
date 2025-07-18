import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

// Services
import { AuthService } from '../../../../core/services/auth.service';
import { NotificationService } from '../../../../core/services/notification.service';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [
    CommonModule,
    CardModule,
    ButtonModule,
    MessageModule,
    MessagesModule,
    ProgressSpinnerModule
  ],
  template: `
    <div class="confirm-email-container">
      <div class="confirm-email-wrapper">
        <p-card styleClass="confirm-email-card">
          <ng-template pTemplate="header">
            <div class="confirm-email-header">
              <img src="assets/images/logo.png" alt="Logo" class="logo" />
              <h2>Confirmação de Email</h2>
            </div>
          </ng-template>

          <!-- Loading State -->
          <div *ngIf="isLoading" class="text-center p-4">
            <p-progressSpinner styleClass="w-4rem h-4rem"></p-progressSpinner>
            <p class="mt-3 text-color-secondary">Confirmando seu email...</p>
          </div>

          <!-- Success State -->
          <div *ngIf="!isLoading && isSuccess" class="text-center p-4">
            <i class="pi pi-check-circle text-6xl text-green-500 mb-3"></i>
            <h3 class="text-green-600 mb-2">Email Confirmado!</h3>
            <p class="text-color-secondary mb-4">
              Seu email foi confirmado com sucesso. Agora você pode fazer login no sistema.
            </p>
            <p-button
              label="Fazer Login"
              icon="pi pi-sign-in"
              routerLink="/auth/login"
              styleClass="w-full"
            ></p-button>
          </div>

          <!-- Error State -->
          <div *ngIf="!isLoading && !isSuccess" class="text-center p-4">
            <i class="pi pi-times-circle text-6xl text-red-500 mb-3"></i>
            <h3 class="text-red-600 mb-2">Erro na Confirmação</h3>
            <p class="text-color-secondary mb-4">
              {{ errorMessage || 'Não foi possível confirmar seu email. O link pode estar expirado ou inválido.' }}
            </p>
            
            <div class="flex flex-column gap-2">
              <p-button
                label="Reenviar Email de Confirmação"
                icon="pi pi-send"
                [loading]="isResending"
                [disabled]="isResending || !email"
                (onClick)="resendConfirmation()"
                styleClass="w-full"
                severity="secondary"
              ></p-button>
              
              <p-button
                label="Voltar ao Login"
                icon="pi pi-arrow-left"
                routerLink="/auth/login"
                styleClass="w-full"
                severity="secondary"
                [outlined]="true"
              ></p-button>
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
      background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-color-text) 100%);
      padding: 1rem;
    }

    .confirm-email-wrapper {
      width: 100%;
      max-width: 400px;
    }

    .confirm-email-card {
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
      border: none;
      border-radius: 1rem;
    }

    .confirm-email-header {
      text-align: center;
      padding: 2rem 1rem 1rem;
    }

    .logo {
      height: 60px;
      margin-bottom: 1rem;
    }

    .confirm-email-header h2 {
      color: var(--primary-color);
      margin: 0;
      font-weight: 600;
    }

    .confirm-email-footer {
      margin-top: 2rem;
    }

    @media (max-width: 768px) {
      .confirm-email-container {
        padding: 0.5rem;
      }
      
      .confirm-email-header {
        padding: 1.5rem 1rem 0.5rem;
      }
      
      .logo {
        height: 50px;
      }
    }
  `]
})
export class ConfirmEmailComponent implements OnInit, OnDestroy {
  isLoading = true;
  isSuccess = false;
  isResending = false;
  errorMessage = '';
  private destroy$ = new Subject<void>();
  email = '';
  private token = '';

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    // Get email and token from query parameters
    this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe(params => {
      this.email = params['email'] || '';
      this.token = params['token'] || '';
      
      if (!this.email || !this.token) {
        this.isLoading = false;
        this.errorMessage = 'Link de confirmação inválido. Parâmetros obrigatórios não encontrados.';
        return;
      }
      
      this.confirmEmail();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private confirmEmail(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.authService.confirmEmail(this.email, this.token)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.isSuccess = true;
          this.notificationService.success('Email confirmado com sucesso!');
        },
        error: (error) => {
          this.isLoading = false;
          this.isSuccess = false;
          this.errorMessage = error || 'Erro ao confirmar email';
        }
      });
  }

  resendConfirmation(): void {
    if (!this.email) {
      this.notificationService.error('Email não encontrado para reenvio');
      return;
    }

    this.isResending = true;

    this.authService.resendEmailConfirmation(this.email)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.isResending = false;
          this.notificationService.success(
            'Email de confirmação reenviado! Verifique sua caixa de entrada.'
          );
        },
        error: (error) => {
          this.isResending = false;
          this.notificationService.error(
            error || 'Erro ao reenviar email de confirmação'
          );
        }
      });
  }
}