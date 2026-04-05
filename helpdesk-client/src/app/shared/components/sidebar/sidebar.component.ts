import { CommonModule } from '@angular/common';
import { Component, computed, inject, Input, input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AuthService } from '../../../core/services/auth.service';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles: string[];
}


@Component({
  selector: 'app-sidebar',
  imports: [CommonModule, RouterLink, RouterLinkActive, MatIconModule, MatTooltipModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
  standalone: true
})
export class SidebarComponent {
  @Input() collapsed = false;

  private auth = inject(AuthService);

  readonly navItems: NavItem[] = [
    { label: 'Dashboard', icon: 'dashboard', route: '/dashboard', roles: ['Admin'] },
    { label: 'All Tickets', icon: 'confirmation_number', route: '/tickets', roles: ['Admin', 'User'] },
    { label: 'My Tickets', icon: 'inbox', route: '/agent/tickets', roles: ['Agent'] },
    { label: 'Users', icon: 'group', route: '/admin/users', roles: ['Admin'] },
    { label: 'Categories', icon: 'category', route: '/admin/categories', roles: ['Admin'] },
  ];

  visibleItems = computed(() => {
    const role = this.auth.currentRole();
    return this.navItems.filter(item => role && item.roles.includes(role));
  });
}
