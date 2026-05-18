import { Component, Input, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AuthService } from '../../../core/services/auth.service';
import { UserRole } from '../../../core/models/user.model';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles: UserRole[];
}

interface NavSection {
  title: string;
  items: NavItem[];
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

  auth = inject(AuthService);

  readonly navSections: NavSection[] = [
    {
      title: 'Main',
      items: [
        { label: 'Dashboard', icon: 'dashboard', route: '/dashboard', roles: [UserRole.Admin] },
        { label: 'All Tickets', icon: 'confirmation_number', route: '/tickets', roles: [UserRole.Admin, UserRole.User] },
        { label: 'My Queue', icon: 'inbox', route: '/agent/tickets', roles: [UserRole.Agent] },
      ]
    },
    {
      title: 'Management',
      items: [
        { label: 'Users', icon: 'group', route: '/admin/users', roles: [UserRole.Admin] },
        { label: 'Categories', icon: 'category', route: '/admin/categories', roles: [UserRole.Admin] },
        { label: 'Departments', icon: 'apartment', route: '/admin/departments', roles: [UserRole.Admin] },
      ]
    },
    {
      title: 'Analytics',
      items: [
        { label: 'Reports', icon: 'analytics', route: '/admin/reports', roles: [UserRole.Admin] },
        { label: 'Audit Log', icon: 'history', route: '/admin/audit', roles: [UserRole.Admin] },
      ]
    },
    {
      title: 'Resources',
      items: [
        { label: 'Knowledge Base', icon: 'menu_book', route: '/kb', roles: [UserRole.Admin, UserRole.Agent, UserRole.User] },
        { label: 'Templates', icon: 'auto_awesome', route: '/admin/recurring-templates', roles: [UserRole.Admin] },
      ]
    },
    {
      title: 'Settings',
      items: [
        { label: 'System Settings', icon: 'settings', route: '/admin/settings', roles: [UserRole.Admin] },
        { label: 'Profile', icon: 'person', route: '/profile', roles: [UserRole.Admin, UserRole.Agent, UserRole.User] },
        { label: 'Notifications', icon: 'notifications', route: '/notifications', roles: [UserRole.Admin, UserRole.Agent, UserRole.User] },
      ]
    }
  ];

  visibleSections = computed(() => {
    const role = this.auth.currentRole();
    if (!role) {
      // Try to force a token read
      const token = this.auth.getToken();
      if (token) {
        // Force re-evaluate by accessing currentUser
        this.auth.currentUser();
      }
      return [];
    }

    return this.navSections
      .map(section => ({
        ...section,
        items: section.items.filter(item => item.roles.includes(role))
      }))
      .filter(section => section.items.length > 0);
  });

  get userRole(): string {
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

  get currentUserName(): string {
    const user = this.auth.currentUser() as any;
    return user?.['unique_name'] || user?.['given_name'] || user?.['name'] || 'User';
  }

  get currentUserEmail(): string {
    const user = this.auth.currentUser() as any;
    return user?.['email'] || '';
  }

  get roleBadgeColor(): string {
    const role = this.auth.currentRole();
    switch (role) {
      case UserRole.Admin: return '#3B82F6';
      case UserRole.Agent: return '#0D9488';
      case UserRole.User: return '#8B5CF6';
      default: return '#64748B';
    }
  }
}