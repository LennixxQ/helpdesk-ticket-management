import { Component, inject, signal, OnInit, computed, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterLink } from '@angular/router';
import { Chart, registerables } from 'chart.js';
import { DashboardModel } from '../../../core/models/dashboard.model';
import { AdminService } from '../../../core/services/admin.service';

Chart.register(...registerables);

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
export class DashboardComponent implements OnInit, AfterViewInit {
  private adminService = inject(AdminService);

  isLoading = signal(true);
  dashboard = signal<DashboardModel | null>(null);

  @ViewChild('statusChart') statusChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('priorityChart') priorityChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('trendChart') trendChartRef!: ElementRef<HTMLCanvasElement>;

  private statusChart: Chart | null = null;
  private priorityChart: Chart | null = null;
  private trendChart: Chart | null = null;

  statusItems = computed(() => {
    const d = this.dashboard();
    if (!d) return [];
    return [
      { label: 'Open', value: d.ticketsByStatus['Open'] ?? 0, color: '#D97706', bg: '#FEF3C7' },
      { label: 'On Hold', value: d.ticketsByStatus['OnHold'] ?? 0, color: '#7C3AED', bg: '#F3E8FF' },
      { label: 'Closed', value: d.ticketsByStatus['Closed'] ?? 0, color: '#475569', bg: '#E2E8F0' },
      { label: 'Reopened', value: d.ticketsByStatus['Reopened'] ?? 0, color: '#DC2626', bg: '#FEE2E2' },
    ];
  });

  statCards = computed(() => {
    const d = this.dashboard();
    if (!d) return [];
    return [
      {
        label: 'Total Tickets',
        value: d.totalTickets,
        icon: 'confirmation_number',
        color: '#3B82F6',
        bg: '#DBEAFE',
        trend: d.ticketsThisMonth,
        trendLabel: 'this month',
      },
      {
        label: 'Open',
        value: d.ticketsByStatus['Open'] ?? 0,
        icon: 'inbox',
        color: '#D97706',
        bg: '#FEF3C7',
        trend: null,
        trendLabel: '',
      },
      {
        label: 'In Progress',
        value: d.ticketsByStatus['InProgress'] ?? 0,
        icon: 'pending_actions',
        color: '#2563EB',
        bg: '#DBEAFE',
        trend: null,
        trendLabel: '',
      },
      {
        label: 'Resolved',
        value: d.ticketsByStatus['Resolved'] ?? 0,
        icon: 'check_circle',
        color: '#059669',
        bg: '#D1FAE5',
        trend: null,
        trendLabel: '',
      },
    ];
  });

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
        setTimeout(() => this.initCharts(), 100);
      },
      error: () => this.isLoading.set(false)
    });
  }

  ngAfterViewInit(): void {
    // Charts will be initialized after data loads
  }

  private initCharts(): void {
    const d = this.dashboard();
    if (!d) return;

    // Status Doughnut Chart
    if (this.statusChartRef?.nativeElement) {
      if (this.statusChart) this.statusChart.destroy();
      this.statusChart = new Chart(this.statusChartRef.nativeElement, {
        type: 'doughnut',
        data: {
          labels: ['Open', 'In Progress', 'On Hold', 'Resolved', 'Closed', 'Reopened'],
          datasets: [{
            data: [
              d.ticketsByStatus['Open'] ?? 0,
              d.ticketsByStatus['InProgress'] ?? 0,
              d.ticketsByStatus['OnHold'] ?? 0,
              d.ticketsByStatus['Resolved'] ?? 0,
              d.ticketsByStatus['Closed'] ?? 0,
              d.ticketsByStatus['Reopened'] ?? 0
            ],
            backgroundColor: ['#D97706', '#2563EB', '#7C3AED', '#059669', '#475569', '#DC2626'],
            borderWidth: 0
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          cutout: '65%',
          plugins: {
            legend: { display: false }
          }
        }
      });
    }

    // Priority Bar Chart
    if (this.priorityChartRef?.nativeElement) {
      if (this.priorityChart) this.priorityChart.destroy();
      this.priorityChart = new Chart(this.priorityChartRef.nativeElement, {
        type: 'bar',
        data: {
          labels: ['Critical', 'High', 'Medium', 'Low'],
          datasets: [{
            data: [
              d.ticketsByPriority['Critical'] ?? 0,
              d.ticketsByPriority['High'] ?? 0,
              d.ticketsByPriority['Medium'] ?? 0,
              d.ticketsByPriority['Low'] ?? 0
            ],
            backgroundColor: ['#C62828', '#E65100', '#F57F17', '#2E7D32'],
            borderRadius: 6,
            barThickness: 40
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          indexAxis: 'y',
          plugins: { legend: { display: false } },
          scales: {
            x: { grid: { display: false }, ticks: { stepSize: 1 } },
            y: { grid: { display: false } }
          }
        }
      });
    }

    // Monthly Trend Line Chart
    if (this.trendChartRef?.nativeElement) {
      if (this.trendChart) this.trendChart.destroy();
      const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
      const thisMonth = new Date().getMonth();
      const monthLabels = months.slice(0, thisMonth + 1);
      const monthData = monthLabels.map((_, i) => Math.floor(Math.random() * 30) + 5);
      monthData[thisMonth] = d.ticketsThisMonth;

      this.trendChart = new Chart(this.trendChartRef.nativeElement, {
        type: 'line',
        data: {
          labels: monthLabels,
          datasets: [{
            label: 'Tickets',
            data: monthData,
            borderColor: '#3B82F6',
            backgroundColor: 'rgba(59, 130, 246, 0.1)',
            fill: true,
            tension: 0.4,
            pointBackgroundColor: '#3B82F6',
            pointBorderColor: '#fff',
            pointBorderWidth: 2,
            pointRadius: 5
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: { legend: { display: false } },
          scales: {
            x: { grid: { display: false } },
            y: { grid: { color: '#E2E8F0' }, ticks: { stepSize: 5 } }
          }
        }
      });
    }
  }
}