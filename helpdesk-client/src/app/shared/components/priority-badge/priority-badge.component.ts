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
      {{ priority }}
    </span>
  `,
  styleUrl: './priority-badge.scss'
})
export class PriorityBadgeComponent {
  @Input() priority!: TicketPriority;

  get priorityClass(): string {
    const map: Record<TicketPriority, string> = {
      Low: 'priority-low',
      Medium: 'priority-medium',
      High: 'priority-high',
      Critical: 'priority-critical',
    };
    return map[this.priority] ?? 'priority-low';
  }

  get icon(): string {
    const map: Record<TicketPriority, string> = {
      Low: 'arrow_downward',
      Medium: 'remove',
      High: 'arrow_upward',
      Critical: 'priority_high',
    };
    return map[this.priority] ?? 'remove';
  }
}