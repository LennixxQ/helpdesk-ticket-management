import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuditService, AuditLogModel } from '../../../core/services/audit.service';
import { AuditDetailDialogComponent } from './audit-detail-dialog/audit-detail-dialog.component';

@Component({
  selector: 'app-audit-log',
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
    MatTableModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTooltipModule,
    MatMenuModule,
    MatDialogModule,
    MatSnackBarModule,
  ],
  templateUrl: './audit-log.html',
  styleUrl: './audit-log.scss'
})
export class AuditLogComponent implements OnInit {
  private auditService = inject(AuditService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  isLoading = signal(true);
  logs = signal<AuditLogModel[]>([]);
  totalCount = signal(0);
  selectedLog = signal<AuditLogModel | null>(null);

  pageIndex = signal(0);
  pageSize = signal(20);

  startDate = new FormControl<Date | null>(null);
  endDate = new FormControl<Date | null>(null);
  actionFilter = new FormControl<string>('');
  entityFilter = new FormControl<string>('');
  userSearch = new FormControl<string>('');
  selectedQuickFilter = signal<string>('');

  displayedColumns = ['timeline', 'timestamp', 'user', 'action', 'entity', 'changes', 'expand'];

  actionTypes = ['Created', 'Updated', 'Deleted', 'Assigned', 'Closed', 'Reopened', 'StatusChanged', 'Login', 'Logout'];
  entityTypes = ['Ticket', 'User', 'Category', 'Department', 'System'];

  private actionCounts = signal<{ created: number; updated: number }>({ created: 0, updated: 0 });

  ngOnInit(): void {
    this.loadLogs();
  }

  loadLogs(): void {
    this.isLoading.set(true);
    const filter = {
      from: this.startDate.value?.toISOString() || undefined,
      to: this.endDate.value?.toISOString() || undefined,
      action: this.actionFilter.value || this.selectedQuickFilter() || undefined,
      entityType: this.entityFilter.value || undefined,
      userSearch: this.userSearch.value || undefined,
      page: this.pageIndex() + 1,
      pageSize: this.pageSize()
    };

    this.auditService.getAll(filter).subscribe({
      next: (res) => {
        if (res.success) {
          this.logs.set(res.data.items);
          this.totalCount.set(res.data.totalCount);
          this.calculateCounts();
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  private calculateCounts(): void {
    const created = this.logs().filter(l => l.action === 'Created').length;
    const updated = this.logs().filter(l => l.action === 'Updated').length;
    this.actionCounts.set({ created, updated });
  }

  getCreatedCount(): number {
    return this.actionCounts().created;
  }

  getUpdatedCount(): number {
    return this.actionCounts().updated;
  }

  setQuickFilter(filter: string): void {
    this.selectedQuickFilter.set(filter);
    this.pageIndex.set(0);
    this.loadLogs();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadLogs();
  }

  applyFilters(): void {
    this.pageIndex.set(0);
    this.loadLogs();
  }

  clearFilters(): void {
    this.startDate.setValue(null);
    this.endDate.setValue(null);
    this.actionFilter.setValue('');
    this.entityFilter.setValue('');
    this.userSearch.setValue('');
    this.selectedQuickFilter.set('');
    this.applyFilters();
  }

  hasActiveFilters(): boolean {
    return !!(this.startDate.value || this.endDate.value || this.actionFilter.value || this.entityFilter.value || this.userSearch.value || this.selectedQuickFilter());
  }

  getActionIcon(action: string): string {
    const map: Record<string, string> = {
      'Created': 'add_circle',
      'Updated': 'edit',
      'Deleted': 'delete',
      'Assigned': 'assignment_ind',
      'Closed': 'check_circle',
      'Reopened': 'replay',
      'StatusChanged': 'swap_horiz',
      'Login': 'login',
      'Logout': 'logout'
    };
    return map[action] || 'info';
  }

  getActionColor(action: string): string {
    const map: Record<string, string> = {
      'Created': '#10B981',
      'Updated': '#3B82F6',
      'Deleted': '#EF4444',
      'Assigned': '#8B5CF6',
      'Closed': '#6B7280',
      'Reopened': '#F59E0B',
      'StatusChanged': '#06B6D4',
      'Login': '#14B8A6',
      'Logout': '#64748B'
    };
    return map[action] || '#3B82F6';
  }

  getEntityIcon(entity: string): string {
    const map: Record<string, string> = {
      'Ticket': 'confirmation_number',
      'User': 'person',
      'Category': 'category',
      'Department': 'business',
      'System': 'settings'
    };
    return map[entity] || 'folder';
  }

  getAvatarColor(name: string | null): string {
    if (!name) return 'linear-gradient(135deg, #6B7280, #4B5563)';
    const colors = [
      'linear-gradient(135deg, #3B82F6, #1D4ED8)',
      'linear-gradient(135deg, #10B981, #059669)',
      'linear-gradient(135deg, #8B5CF6, #7C3AED)',
      'linear-gradient(135deg, #F59E0B, #D97706)',
      'linear-gradient(135deg, #EF4444, #DC2626)',
      'linear-gradient(135deg, #06B6D4, #0891B2)',
      'linear-gradient(135deg, #EC4899, #DB2777)',
    ];
    const index = name.charCodeAt(0) % colors.length;
    return colors[index];
  }

  formatEntityId(id: string | null): string {
    if (!id) return 'N/A';
    return id.length > 12 ? id.substring(0, 12) + '...' : id;
  }

  formatDetails(details: string): string {
    if (!details) return '-';
    if (details.length > 60) return details.substring(0, 60) + '...';
    return details;
  }

  getShowingCount(): number {
    const start = this.pageIndex() * this.pageSize();
    const end = Math.min(start + this.pageSize(), this.totalCount());
    return this.logs().length > 0 ? start + 1 : 0;
  }

  viewDetails(log: AuditLogModel): void {
    this.dialog.open(AuditDetailDialogComponent, {
      data: log,
      width: '600px',
      maxWidth: '95vw',
      panelClass: 'audit-detail-dialog-panel'
    });
  }

  copyToClipboard(text: string): void {
    navigator.clipboard.writeText(text).then(() => {
      this.snackBar.open('Copied to clipboard', 'Close', {
        duration: 2000
      });
    });
  }

  getActiveFilterCount(): number {
    let count = 0;
    if (this.startDate.value) count++;
    if (this.endDate.value) count++;
    if (this.actionFilter.value) count++;
    if (this.entityFilter.value) count++;
    if (this.userSearch.value) count++;
    if (this.selectedQuickFilter()) count++;
    return count;
  }

  getEntityBgColor(entity: string): string {
    const map: Record<string, string> = {
      'Ticket': 'rgba(59, 130, 246, 0.1)',
      'User': 'rgba(16, 185, 129, 0.1)',
      'Category': 'rgba(139, 92, 246, 0.1)',
      'Department': 'rgba(245, 158, 11, 0.1)',
      'System': 'rgba(107, 114, 128, 0.1)'
    };
    return map[entity] || 'rgba(107, 114, 128, 0.1)';
  }

  getEntityIconColor(entity: string): string {
    const map: Record<string, string> = {
      'Ticket': '#3B82F6',
      'User': '#10B981',
      'Category': '#8B5CF6',
      'Department': '#F59E0B',
      'System': '#6B7280'
    };
    return map[entity] || '#6B7280';
  }

  filterByUser(userName: string | null): void {
    if (userName) {
      this.userSearch.setValue(userName);
      this.applyFilters();
      this.snackBar.open(`Filtering by user: ${userName}`, 'Close', { duration: 2000 });
    }
  }

  getShowingStart(): number {
    return this.pageIndex() * this.pageSize() + 1;
  }

  getShowingEnd(): number {
    return Math.min((this.pageIndex() + 1) * this.pageSize(), this.totalCount());
  }
}