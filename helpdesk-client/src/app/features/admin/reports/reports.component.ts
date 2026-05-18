import { Component, inject, signal, OnInit, ViewChild, ElementRef, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { Chart, registerables } from 'chart.js';
import { ReportsService, ReportFilter } from '../../../core/services/reports.service';

Chart.register(...registerables);

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatTableModule,
  ],
  templateUrl: './reports.html',
  styleUrl: './reports.scss'
})
export class ReportsComponent implements OnInit, AfterViewInit {
  private reportsService = inject(ReportsService);
  private snackBar = inject(MatSnackBar);
  private cdr = inject(ChangeDetectorRef);

  @ViewChild('volumeChart') volumeChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('agentChart') agentChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('slaChart') slaChartRef!: ElementRef<HTMLCanvasElement>;

  isLoading = signal(true);
  activeTab = signal<'volume' | 'agent' | 'sla'>('volume');

  startDate = new FormControl<Date | null>(null);
  endDate = new FormControl<Date | null>(null);

  volumeData = signal<any>(null);
  agentData = signal<any>(null);
  slaData = signal<any>(null);

  private volumeChart: Chart | null = null;
  private agentChart: Chart | null = null;
  private slaChart: Chart | null = null;

  totalTickets = signal(0);
  avgResolution = signal(0);
  slaCompliance = signal(0);
  topAgent = signal('');

  ngOnInit(): void {
    this.loadVolumeReport();
    this.loadTopAgent();
  }

  ngAfterViewInit(): void {}

  loadVolumeReport(): void {
    this.isLoading.set(true);
    this.activeTab.set('volume');
    const filter = this.buildFilter();
    this.reportsService.getTicketVolume(filter).subscribe({
      next: (res: any) => {
        const data = res.totalTickets !== undefined ? res : (res.data || res);
        if (data.totalTickets !== undefined) {
          this.volumeData.set(data);
          this.totalTickets.set(data.totalTickets || 0);
          this.isLoading.set(false);
          this.cdr.detectChanges();
          setTimeout(() => this.initVolumeChart(data), 500);
        } else {
          this.isLoading.set(false);
        }
      },
      error: () => this.isLoading.set(false)
    });
  }

  loadAgentReport(): void {
    this.isLoading.set(true);
    this.activeTab.set('agent');
    this.reportsService.getTopAgent().subscribe({
      next: (res: any) => {
        const agentId = res.AgentId || res.agentId;
        const agentName = res.AgentName || res.agentName;
        const avgRes = res.AvgResolutionHours || res.avgResolutionHours;

        if (agentId || agentName) {
          this.agentData.set(res);
          if (avgRes) this.avgResolution.set(Math.round(avgRes));
          if (agentName) this.topAgent.set(agentName);
          this.isLoading.set(false);
          this.cdr.detectChanges();
          setTimeout(() => this.initAgentChart(res), 300);
        } else {
          this.isLoading.set(false);
        }
      },
      error: () => this.isLoading.set(false)
    });
  }

  loadTopAgent(): void {
    this.reportsService.getTopAgent().subscribe({
      next: (res: any) => {
        const agentId = res.AgentId || res.agentId;
        const agentName = res.AgentName || res.agentName;
        const avgRes = res.AvgResolutionHours || res.avgResolutionHours;

        if (agentId || agentName) {
          if (avgRes) this.avgResolution.set(Math.round(avgRes));
          if (agentName) this.topAgent.set(agentName);
          this.cdr.detectChanges();
        }
      },
      error: () => {}
    });
  }

  loadSlaReport(): void {
    this.isLoading.set(true);
    this.activeTab.set('sla');
    const filter = this.buildFilter();
    this.reportsService.getSlaCompliance(filter).subscribe({
      next: (res: any) => {
        const data = res.totalTickets !== undefined ? res : (res.data || res);
        if (data.totalTickets !== undefined) {
          this.slaData.set(data);
          this.slaCompliance.set(data.compliancePct || data.complianceRate || 0);
          this.isLoading.set(false);
          this.cdr.detectChanges();
          setTimeout(() => this.initSlaChart(data), 300);
        } else {
          this.isLoading.set(false);
        }
      },
      error: () => this.isLoading.set(false)
    });
  }

  private buildFilter(): ReportFilter {
    return {
      from: this.startDate.value?.toISOString() || undefined,
      to: this.endDate.value?.toISOString() || undefined
    };
  }

  private initVolumeChart(data: any): void {
    const tryInit = (attempt: number) => {
      const canvasEl = this.volumeChartRef?.nativeElement;
      if (!canvasEl) {
        if (attempt < 3) setTimeout(() => tryInit(attempt + 1), 200);
        return;
      }

      const style = window.getComputedStyle(canvasEl);
      if (style.display === 'none' || canvasEl.width === 0 || canvasEl.height === 0) {
        if (attempt < 3) setTimeout(() => tryInit(attempt + 1), 200);
        return;
      }

      if (this.volumeChart) {
        this.volumeChart.destroy();
        this.volumeChart = null;
      }

      const byDay = data?.byDay || data?.ByDay || {};
      const labels = Object.keys(byDay).sort();
      const values = labels.map(l => byDay[l]);

      if (labels.length === 0) return;

      this.volumeChart = new Chart(canvasEl, {
        type: 'bar',
        data: {
          labels,
          datasets: [{
            label: 'Tickets Created',
            data: values,
            backgroundColor: '#3B82F6',
            borderRadius: 4
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          animation: { duration: 800 },
          plugins: { legend: { display: false } },
          scales: {
            x: { grid: { display: false } },
            y: { grid: { color: '#E2E8F0' }, beginAtZero: true }
          }
        }
      });
    };

    setTimeout(() => tryInit(1), 100);
  }

  private initAgentChart(data: any): void {
    const tryInit = (attempt: number) => {
      const canvasEl = this.agentChartRef?.nativeElement;
      if (!canvasEl) {
        if (attempt < 3) setTimeout(() => tryInit(attempt + 1), 200);
        return;
      }

      if (this.agentChart) this.agentChart.destroy();

      const agentName = data.AgentName || data.agentName || 'Agent';
      const totalResolved = data.TotalResolved || data.totalResolved || 0;

      this.agentChart = new Chart(canvasEl, {
        type: 'bar',
        data: {
          labels: [agentName],
          datasets: [{
            label: 'Resolved Tickets',
            data: [totalResolved],
            backgroundColor: '#059669',
            borderRadius: 6
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          animation: { duration: 800 },
          plugins: { legend: { display: false } },
          scales: {
            y: { grid: { color: '#E2E8F0' }, beginAtZero: true }
          }
        }
      });
    };

    setTimeout(() => tryInit(1), 100);
  }

  private initSlaChart(data: any): void {
    if (!this.slaChartRef?.nativeElement) return;
    if (this.slaChart) this.slaChart.destroy();

    const within = data.withinSla || data.WithinSla || 0;
    const breached = data.breached || data.Breached || 0;

    if (within === 0 && breached === 0) return;

    const canvas = this.slaChartRef.nativeElement;
    const container = canvas.parentElement;
    if (container) {
      const rect = container.getBoundingClientRect();
      canvas.width = rect.width || 300;
      canvas.height = rect.height || 200;
    }

    this.slaChart = new Chart(canvas, {
      type: 'doughnut',
      data: {
        labels: ['Within SLA', 'Breached'],
        datasets: [{
          data: [within, breached],
          backgroundColor: ['#059669', '#DC2626'],
          borderWidth: 0
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '60%',
        animation: { duration: 800 },
        plugins: { legend: { position: 'bottom' } }
      }
    });
  }

  exportCsv(): void {
    this.reportsService.exportTicketsCsv(this.buildFilter()).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `tickets-report-${new Date().toISOString().split('T')[0]}.csv`;
        a.click();
        URL.revokeObjectURL(url);
        this.showSnack('Report exported', 'success');
      },
      error: () => this.showSnack('Export failed', 'error')
    });
  }

  exportPdf(): void {
    this.reportsService.exportTicketsPdf(this.buildFilter()).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `tickets-report-${new Date().toISOString().split('T')[0]}.pdf`;
        a.click();
        URL.revokeObjectURL(url);
        this.showSnack('PDF exported', 'success');
      },
      error: () => this.showSnack('Export failed', 'error')
    });
  }

  showSnack(msg: string, type: 'success' | 'error'): void {
    this.snackBar.open(msg, 'Close', {
      duration: 3000,
      panelClass: type === 'success' ? ['snack-success'] : ['snack-error']
    });
  }
}