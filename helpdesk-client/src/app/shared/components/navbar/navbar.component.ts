import { Component, EventEmitter, inject, Output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

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
    if (role === 'Admin') return '#7B9EE5';
    if (role === 'Agent') return '#82D8C8';
    return '#F2A7C3';
  }

  logout() {
    this.auth.logout();
  }
}
