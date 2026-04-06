import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterLink } from '@angular/router';
import { DashboardModel } from '../../../core/models/dashboard.model';
import { AdminService } from '../../../core/services/admin.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class DashboardComponent implements OnInit {
  private adminService = inject(AdminService);

  isLoading = signal(true);
  dashboard = signal<DashboardModel | null>(null);

  statusItems = computed(() => {
    const d = this.dashboard();
    if (!d) return [];
    return [
      { label: 'Open', value: d.ticketsByStatus['Open'] ?? 0, color: '#D4860A', bg: '#FFF0D6' },
      { label: 'On Hold', value: d.ticketsByStatus['OnHold'] ?? 0, color: '#7B3FCC', bg: '#F3E8FF' },
      { label: 'Closed', value: d.ticketsByStatus['Closed'] ?? 0, color: '#4A5568', bg: '#E8EAF0' },
      { label: 'Reopened', value: d.ticketsByStatus['Reopened'] ?? 0, color: '#C53030', bg: '#FFE5E5' },
    ];
  });

  // Stat cards config
  statCards = computed(() => {
    const d = this.dashboard();
    if (!d) return [];
    return [
      {
        label: 'Total Tickets',
        value: d.totalTickets,
        icon: 'confirmation_number',
        color: '#7B9EE5',
        bg: '#EEF3FC',
        trend: d.ticketsThisMonth,
        trendLabel: 'this month',
      },
      {
        label: 'Open',
        value: d.ticketsByStatus['Open'] ?? 0,
        icon: 'inbox',
        color: '#D4860A',
        bg: '#FFF0D6',
        trend: null,
        trendLabel: '',
      },
      {
        label: 'In Progress',
        value: d.ticketsByStatus['InProgress'] ?? 0,
        icon: 'pending_actions',
        color: '#1A78C2',
        bg: '#D6EEFF',
        trend: null,
        trendLabel: '',
      },
      {
        label: 'Resolved',
        value: d.ticketsByStatus['Resolved'] ?? 0,
        icon: 'check_circle_outline',
        color: '#1A8C5A',
        bg: '#D6F5E8',
        trend: null,
        trendLabel: '',
      },
    ];
  });

  // Priority breakdown
  priorityData = computed(() => {
    const d = this.dashboard();
    if (!d) return [];
    const total = d.totalTickets || 1;
    return [
      { label: 'Critical', value: d.ticketsByPriority['Critical'] ?? 0, color: '#C62828', bg: '#FCE4EC', percent: Math.round(((d.ticketsByPriority['Critical'] ?? 0) / total) * 100) },
      { label: 'High', value: d.ticketsByPriority['High'] ?? 0, color: '#E65100', bg: '#FFF3E0', percent: Math.round(((d.ticketsByPriority['High'] ?? 0) / total) * 100) },
      { label: 'Medium', value: d.ticketsByPriority['Medium'] ?? 0, color: '#F57F17', bg: '#FFF8E1', percent: Math.round(((d.ticketsByPriority['Medium'] ?? 0) / total) * 100) },
      { label: 'Low', value: d.ticketsByPriority['Low'] ?? 0, color: '#2E7D32', bg: '#E8F5E9', percent: Math.round(((d.ticketsByPriority['Low'] ?? 0) / total) * 100) },
    ];
  });

  // Month comparison
  monthTrend = computed(() => {
    const d = this.dashboard();
    if (!d) return 0;
    if (!d.ticketsLastMonth) return 100;
    return Math.round(((d.ticketsThisMonth - d.ticketsLastMonth) / d.ticketsLastMonth) * 100);
  });

  ngOnInit(): void {
    this.adminService.getDashboard().subscribe({
      next: (res: any) => {
        if (res.success) this.dashboard.set(res.data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }
}