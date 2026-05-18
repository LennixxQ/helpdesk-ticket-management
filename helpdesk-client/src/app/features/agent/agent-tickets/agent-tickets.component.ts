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
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { PriorityBadgeComponent } from '../../../shared/components/priority-badge/priority-badge.component';
import { AuthService } from '../../../core/services/auth.service';
import { TicketService } from '../../../core/services/ticket.service';
import { TicketModel, TicketStatus } from '../../../core/models/ticket.model';

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
    StatusBadgeComponent,
    PriorityBadgeComponent,
  ],
  templateUrl: './agent-tickets.html',
  styleUrl: './agent-tickets.scss'
})
export class AgentTicketsComponent implements OnInit {
  private ticketService = inject(TicketService);
  auth = inject(AuthService);

  isLoading = signal(true);
  tickets = signal<TicketModel[]>([]);
  statusFilter = new FormControl<TicketStatus | ''>('');

  readonly statuses = [TicketStatus.Open, TicketStatus.InProgress, TicketStatus.OnHold, TicketStatus.Resolved, TicketStatus.Closed, TicketStatus.Reopened];

  statusLabel: Record<number, string> = {
    [TicketStatus.Open]: 'Open',
    [TicketStatus.InProgress]: 'In Progress',
    [TicketStatus.OnHold]: 'On Hold',
    [TicketStatus.Resolved]: 'Resolved',
    [TicketStatus.Closed]: 'Closed',
    [TicketStatus.Reopened]: 'Reopened'
  };

  filteredTickets = computed(() => {
    const f = this.statusFilter.value;
    const all = this.tickets();
    return f ? all.filter(t => t.status === f) : all;
  });

  inProgressCount = computed(() => this.tickets().filter(t => t.status === TicketStatus.InProgress).length);
  onHoldCount = computed(() => this.tickets().filter(t => t.status === TicketStatus.OnHold).length);
  resolvedCount = computed(() => this.tickets().filter(t => t.status === TicketStatus.Resolved).length);

  ngOnInit(): void { this.loadTickets(); }

  loadTickets(): void {
    this.isLoading.set(true);
    this.ticketService.getAll({ page: 1, pageSize: 100 }).subscribe({
      next: (res: any) => {
        if (res.success) this.tickets.set(res.data.items);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  updateStatus(ticket: TicketModel, newStatus: TicketStatus): void {
    this.ticketService.updateStatus(ticket.id, newStatus).subscribe({
      next: (res) => {
        if (res.success) {
          this.tickets.update(list =>
            list.map(t => t.id === ticket.id ? { ...t, status: res.data.status } : t)
          );
        }
      }
    });
  }

  shortId(id: string): string {
    return '#' + id.substring(0, 8).toUpperCase();
  }
}