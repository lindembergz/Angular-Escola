import { Component, EventEmitter, Output } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Store } from '@ngrx/store';
import { MenuItem } from 'primeng/api';
import { Menu } from 'primeng/menu';
import { Avatar } from 'primeng/avatar';
import { Badge } from 'primeng/badge';
import { Tooltip } from 'primeng/tooltip';
import { Ripple } from 'primeng/ripple';
import { AuthService } from '../../../features/auth/services/auth.service';
import { User } from '../../../features/auth/models/auth.models';
import * as AuthActions from '../../../store/auth/auth.actions';
import { AppState } from '../../../store/app.state';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, Menu, Avatar, Badge, Tooltip, Ripple],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  @Output() toggleSidebar = new EventEmitter<void>();

  currentUser: User | null = null;
  userMenuItems: MenuItem[] = [];

  constructor(
    private authService: AuthService,
    private router: Router,
    private store: Store<AppState>
  ) {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.setupUserMenu();
    });
  }

  onToggleSidebar(): void {
    this.toggleSidebar.emit();
  }

  navigateToHome(): void {
    this.router.navigate(['/']);
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
    // Dispatch logout action through NgRx
    this.store.dispatch(AuthActions.logout());
  }
}