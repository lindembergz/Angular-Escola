import { Component, EventEmitter, Output } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { AuthService, User } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  @Output() toggleSidebar = new EventEmitter<void>();

  currentUser: User | null = null;
  userMenuItems: MenuItem[] = [];

  constructor(
    private authService: AuthService,
    private notificationService: NotificationService,
    private router: Router
  ) {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.setupUserMenu();
    });
  }

  onToggleSidebar(): void {
    this.toggleSidebar.emit();
  }

  private setupUserMenu(): void {
    this.userMenuItems = [
      {
        label: 'Perfil',
        icon: 'pi pi-user',
        command: () => this.goToProfile()
      },
      {
        label: 'Configurações',
        icon: 'pi pi-cog',
        command: () => this.goToSettings()
      },
      {
        separator: true
      },
      {
        label: 'Sair',
        icon: 'pi pi-sign-out',
        command: () => this.logout()
      }
    ];
  }

  private goToProfile(): void {
    this.router.navigate(['/profile']);
  }

  private goToSettings(): void {
    this.router.navigate(['/settings']);
  }

  private logout(): void {
    this.authService.logout();
    this.notificationService.success('Logout realizado com sucesso');
    this.router.navigate(['/login']);
  }
}