import { Component, EventEmitter, inject, Output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { UserRole } from '../../../core/models/user.model';

@Component({
  selector: 'app-navbar',
  imports: [MatIconModule, MatButtonModule, MatMenuModule, MatTooltipModule, MatDividerModule, CommonModule, RouterLink],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
  standalone: true
})
export class NavbarComponent {
  @Output() toggleSidebar = new EventEmitter<void>();

  auth = inject(AuthService);
  router = inject(Router);

  get userInitial(): string {
    const name = this.auth.currentUserName();
    const email = this.auth.currentUser()?.['email'] as string;
    const display = name ?? email ?? 'U';
    return display.charAt(0).toUpperCase();
  }
  get userDisplayName(): string {
    return this.auth.currentUserName()
      ?? (this.auth.currentUser()?.['email'] as string)
      ?? 'User';
  }

  get roleColor(): string {
    const role = this.auth.currentRole();
    if (role === UserRole.Admin) return '#3B82F6';
    if (role === UserRole.Agent) return '#0D9488';
    return '#8B5CF6';
  }

  logout() {
    this.auth.logout();
  }

  getRoleClass(): string {
    const role = this.auth.currentRole();
    if (role === UserRole.Admin) return 'admin';
    if (role === UserRole.Agent) return 'agent';
    return 'user';
  }

  get userRoleLabel(): string {
    const role = this.auth.currentRole();
    if (role === null || role === undefined) return '';
    switch (role) {
      case UserRole.Admin: return 'Admin';
      case UserRole.Agent: return 'Support Agent';
      case UserRole.User: return 'User';
      case UserRole.DepartmentHead: return 'Department Head';
      default: return '';
    }
  }

  get profilePicUrl(): string | null {
    return this.auth.profilePic();
  }

  get profilePicPosition(): string {
    return this.auth.profilePicPosition();
  }
}
