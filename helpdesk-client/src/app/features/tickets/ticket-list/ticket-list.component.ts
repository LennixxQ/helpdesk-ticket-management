import {
  Component, inject, signal, OnInit, computed, ViewChild, HostListener
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { PriorityBadgeComponent } from '../../../shared/components/priority-badge/priority-badge.component';
import { AuthService } from '../../../core/services/auth.service';
import { TicketModel, TicketStatus, TicketPriority, TicketFilterParams } from '../../../core/models/ticket.model';
import { TicketService } from '../../../core/services/ticket.service';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    StatusBadgeComponent,
    PriorityBadgeComponent,
  ],
  templateUrl: './ticket-list.html',
  styleUrl: './ticket-list.scss'
})
export class TicketListComponent implements OnInit {
  private ticketService = inject(TicketService);
  auth = inject(AuthService);

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  // ── State ──────────────────────────────────────────────────
  isLoading = signal(true);
  tickets = signal<TicketModel[]>([]);
  totalCount = signal(0);
  page = signal(1);
  pageSize = signal(10);

  // ── Filters ────────────────────────────────────────────────
  statusFilter = new FormControl<TicketStatus | ''>('');
  priorityFilter = new FormControl<TicketPriority | ''>('');

  readonly statuses: TicketStatus[] = ['Open', 'InProgress', 'OnHold', 'Resolved', 'Closed', 'Reopened'];
  readonly priorities: TicketPriority[] = ['Low', 'Medium', 'High', 'Critical'];

  isMobile = signal(window.innerWidth < 768);

  @HostListener('window:resize')
  onResize() { this.isMobile.set(window.innerWidth < 768); }

  readonly displayedColumns = computed(() => {
    if (this.isMobile()) return ['title', 'status', 'actions'];
    const base = ['id', 'title', 'category', 'priority', 'status', 'createdAt', 'actions'];
    if (this.auth.isAdmin()) return ['id', 'title', 'category', 'priority', 'status', 'assignedAgent', 'createdAt', 'actions'];
    return base;
  });

  dataSource = new MatTableDataSource<TicketModel>();

  // ── Status label map ───────────────────────────────────────
  statusLabel: Record<TicketStatus, string> = {
    Open: 'Open', InProgress: 'In Progress', OnHold: 'On Hold',
    Resolved: 'Resolved', Closed: 'Closed', Reopened: 'Reopened'
  };

  ngOnInit(): void {
    this.loadTickets();

    // Filter change listeners
    this.statusFilter.valueChanges.subscribe(() => {
      this.page.set(1);
      this.loadTickets();
    });
    this.priorityFilter.valueChanges.subscribe(() => {
      this.page.set(1);
      this.loadTickets();
    });
  }

  loadTickets(): void {
    this.isLoading.set(true);
    const filters: TicketFilterParams = {
      page: this.page(),
      pageSize: this.pageSize(),
      status: this.statusFilter.value || undefined,
      priority: this.priorityFilter.value || undefined,
    };

    this.ticketService.getAll(filters).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.tickets.set(res.data.items);
          this.totalCount.set(res.data.totalCount);
          this.dataSource.data = res.data.items;
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onPageChange(event: PageEvent): void {
    this.page.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.loadTickets();
  }

  clearFilters(): void {
    this.statusFilter.setValue('');
    this.priorityFilter.setValue('');
  }

  hasActiveFilters = computed(() =>
    !!this.statusFilter.value || !!this.priorityFilter.value
  );

  trackById(_: number, item: TicketModel) { return item.id; }

  shortId(id: string): string {
    return '#' + id.substring(0, 8).toUpperCase();
  }
}