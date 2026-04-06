import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketStatus } from '../../../core/models/ticket.model';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span class="badge" [ngClass]="statusClass">
      <span class="dot"></span>
      {{ label }}
    </span>
  `,
  styleUrl: './status-badge.scss',
})
export class StatusBadgeComponent {
  @Input() status!: TicketStatus;

  get statusClass(): string {
    const map: Record<TicketStatus, string> = {
      Open: 'badge-open',
      InProgress: 'badge-inprogress',
      OnHold: 'badge-onhold',
      Resolved: 'badge-resolved',
      Closed: 'badge-closed',
      Reopened: 'badge-reopened',
    };
    return map[this.status] ?? 'badge-open';
  }

  get label(): string {
    const map: Record<TicketStatus, string> = {
      Open: 'Open',
      InProgress: 'In Progress',
      OnHold: 'On Hold',
      Resolved: 'Resolved',
      Closed: 'Closed',
      Reopened: 'Reopened',
    };
    return map[this.status] ?? this.status;
  }
}
