import { Component, HostListener, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { inject } from '@angular/core/primitives/di';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet, MatSidenavModule, NavbarComponent, SidebarComponent],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss',
  standalone: true
})
export class MainLayoutComponent {
  private auth = inject(AuthService);

  isSidebarCollapsed = signal(false);
  isMobile = signal(window.innerWidth < 768);

  @HostListener('window:resize')
  onResize() {
    const mobile = window.innerWidth < 768;
    this.isMobile.set(mobile);
    if (mobile) this.isSidebarCollapsed.set(true);
  }

  toggleSidebar() {
    this.isSidebarCollapsed.update(v => !v);
  }

  get sidebarWidth() {
    return this.isSidebarCollapsed() ? '72px' : '260px';
  }
}
