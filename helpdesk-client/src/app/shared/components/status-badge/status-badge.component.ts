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
    const map: Record<number, string> = {
      [TicketStatus.Open]: 'badge-open',
      [TicketStatus.InProgress]: 'badge-inprogress',
      [TicketStatus.OnHold]: 'badge-onhold',
      [TicketStatus.Resolved]: 'badge-resolved',
      [TicketStatus.Closed]: 'badge-closed',
      [TicketStatus.Reopened]: 'badge-reopened',
    };
    return map[this.status] ?? 'badge-open';
  }

  get label(): string {
    const map: Record<number, string> = {
      [TicketStatus.Open]: 'Open',
      [TicketStatus.InProgress]: 'In Progress',
      [TicketStatus.OnHold]: 'On Hold',
      [TicketStatus.Resolved]: 'Resolved',
      [TicketStatus.Closed]: 'Closed',
      [TicketStatus.Reopened]: 'Reopened',
    };
    return map[this.status] ?? 'Unknown';
  }
}
