import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { TicketPriority } from '../../../core/models/ticket.model';

@Component({
  selector: 'app-priority-badge',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  template: `
    <span class="priority-badge" [ngClass]="priorityClass">
      <mat-icon class="priority-icon">{{ icon }}</mat-icon>
      {{ label }}
    </span>
  `,
  styleUrl: './priority-badge.scss'
})
export class PriorityBadgeComponent {
  @Input() priority!: TicketPriority;

  get priorityClass(): string {
    const map: Record<number, string> = {
      [TicketPriority.Low]: 'priority-low',
      [TicketPriority.Medium]: 'priority-medium',
      [TicketPriority.High]: 'priority-high',
      [TicketPriority.Critical]: 'priority-critical',
    };
    return map[this.priority] ?? 'priority-low';
  }

  get icon(): string {
    const map: Record<number, string> = {
      [TicketPriority.Low]: 'arrow_downward',
      [TicketPriority.Medium]: 'remove',
      [TicketPriority.High]: 'arrow_upward',
      [TicketPriority.Critical]: 'priority_high',
    };
    return map[this.priority] ?? 'remove';
  }

  get label(): string {
    const map: Record<number, string> = {
      [TicketPriority.Low]: 'Low',
      [TicketPriority.Medium]: 'Medium',
      [TicketPriority.High]: 'High',
      [TicketPriority.Critical]: 'Critical',
    };
    return map[this.priority] ?? 'Unknown';
  }
}