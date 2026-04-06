import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { PriorityBadgeComponent } from '../../../shared/components/priority-badge/priority-badge.component';
import { AuthService } from '../../../core/services/auth.service';
import { TicketModel, TicketStatus } from '../../../core/models/ticket.model';
import { TicketService } from '../../../core/services/ticket.service';

@Component({
  selector: 'app-agent-tickets',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatSnackBarModule,
    StatusBadgeComponent,
    PriorityBadgeComponent,
  ],
  templateUrl: './agent-tickets.html',
  styleUrl: './agent-tickets.scss'
})
export class AgentTicketsComponent implements OnInit {
  private ticketService = inject(TicketService);
  private snackBar = inject(MatSnackBar);
  auth = inject(AuthService);

  isLoading = signal(true);
  tickets = signal<TicketModel[]>([]);
  statusFilter = new FormControl<TicketStatus | ''>('');

  readonly statuses: TicketStatus[] = ['InProgress', 'OnHold', 'Resolved'];

  readonly statusLabel: Record<TicketStatus, string> = {
    Open: 'Open', InProgress: 'In Progress', OnHold: 'On Hold',
    Resolved: 'Resolved', Closed: 'Closed', Reopened: 'Reopened'
  };

  // ── Computed stats ─────────────────────────────────────────
  inProgressCount = computed(() =>
    this.tickets().filter(t => t.status === 'InProgress').length
  );
  onHoldCount = computed(() =>
    this.tickets().filter(t => t.status === 'OnHold').length
  );
  resolvedCount = computed(() =>
    this.tickets().filter(t => t.status === 'Resolved').length
  );

  filteredTickets = computed(() => {
    const status = this.statusFilter.value;
    if (!status) return this.tickets();
    return this.tickets().filter(t => t.status === status);
  });

  ngOnInit(): void {
    this.loadTickets();
    this.statusFilter.valueChanges.subscribe(() => { });
  }

  loadTickets(): void {
    this.isLoading.set(true);
    // ✅ POST /api/tickets/GetAllTicket — body mein filters
    this.ticketService.getAll({ pageSize: 100 }).subscribe({
      next: (res) => {
        if (res.success) this.tickets.set(res.data.items);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  updateStatus(ticket: TicketModel, newStatus: TicketStatus): void {
    // ✅ PUT /api/tickets/UpdateTicketStatus — ticketId + newStatus in body
    this.ticketService.updateStatus(ticket.id, newStatus).subscribe({
      next: (res) => {
        if (res.success) {
          this.tickets.update(list =>
            list.map(t => t.id === ticket.id ? res.data : t)
          );
          this.snackBar.open('Status updated', 'Close', {
            duration: 2500,
            panelClass: ['snack-success']
          });
        }
      }
    });
  }

  shortId(id: string): string {
    return '#' + id.substring(0, 8).toUpperCase();
  }
}