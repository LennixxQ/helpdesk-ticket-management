import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { PriorityBadgeComponent } from '../../../shared/components/priority-badge/priority-badge.component';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { AuthService } from '../../../core/services/auth.service';
import { TicketModel, TicketStatus, TicketPriority } from '../../../core/models/ticket.model';
import { UserModel } from '../../../core/models/user.model';
import { AdminService } from '../../../core/services/admin.service';
import { TicketService } from '../../../core/services/ticket.service';

@Component({
  selector: 'app-ticket-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDividerModule,
    MatTooltipModule,
    MatDialogModule,
    StatusBadgeComponent,
    PriorityBadgeComponent,
    ConfirmationDialogComponent,
  ],
  templateUrl: './ticket-detail.html',
  styleUrl: './ticket-detail.scss'
})
export class TicketDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private ticketService = inject(TicketService);
  private adminService = inject(AdminService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);
  auth = inject(AuthService);

  // ── State ──────────────────────────────────────────────────
  isLoading = signal(true);
  isSaving = signal(false);
  isCommenting = signal(false);
  ticket = signal<TicketModel | null>(null);
  agents = signal<UserModel[]>([]);

  // ── Controls ───────────────────────────────────────────────
  commentControl = new FormControl('', [Validators.required, Validators.minLength(3)]);
  agentControl = new FormControl<string>('');
  statusControl = new FormControl<TicketStatus | ''>('');
  priorityControl = new FormControl<TicketPriority | ''>('');

  readonly agentStatuses: TicketStatus[] = ['InProgress', 'OnHold', 'Resolved'];
  readonly adminStatuses: TicketStatus[] = ['InProgress', 'OnHold', 'Resolved', 'Closed', 'Reopened'];
  readonly priorities: TicketPriority[] = ['Low', 'Medium', 'High', 'Critical'];

  readonly statusLabel: Record<TicketStatus, string> = {
    Open: 'Open', InProgress: 'In Progress', OnHold: 'On Hold',
    Resolved: 'Resolved', Closed: 'Closed', Reopened: 'Reopened'
  };

  availableStatuses = computed(() =>
    this.auth.isAdmin() ? this.adminStatuses : this.agentStatuses
  );

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loadTicket(id);
    if (this.auth.isAdmin()) this.loadAgents();
  }

  loadTicket(id: string): void {
    this.ticketService.getById(id).subscribe({
      next: (res) => {
        if (res.success) {
          this.ticket.set(res.data);
          this.statusControl.setValue(res.data.status);
          this.priorityControl.setValue(res.data.priority);
          this.agentControl.setValue(res.data.assignedAgentId ?? '');
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.router.navigate(['/tickets']);
      }
    });
  }

  loadAgents(): void {
    // ✅ GET /api/users/agents/active
    this.adminService.getActiveAgents().subscribe({
      next: (res) => { if (res.success) this.agents.set(res.data); }
    });
  }

  // ── Actions ────────────────────────────────────────────────
  assignAgent(): void {
    const agentId = this.agentControl.value;
    const ticketId = this.ticket()?.id;
    if (!agentId || !ticketId) return;

    this.isSaving.set(true);
    // ✅ PUT /api/tickets/Agent-assign
    this.ticketService.assign(ticketId, agentId).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        if (res.success) {
          this.ticket.set(res.data);
          this.showSnack('Agent assigned successfully', 'success');
        }
      },
      error: () => this.isSaving.set(false)
    });
  }

  updateStatus(): void {
    const status = this.statusControl.value as TicketStatus;
    const ticketId = this.ticket()?.id;
    if (!status || !ticketId) return;

    this.isSaving.set(true);
    this.ticketService.updateStatus(ticketId, status).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        if (res.success) {
          this.ticket.set(res.data);
          this.showSnack('Status updated', 'success');
        }
      },
      error: () => this.isSaving.set(false)
    });
  }

  updatePriority(): void {
    const priority = this.priorityControl.value as TicketPriority;
    const ticketId = this.ticket()?.id;
    if (!priority || !ticketId) return;

    this.isSaving.set(true);
    // ✅ PUT /api/tickets/UpdatePriority
    this.ticketService.updatePriority(ticketId, priority).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        if (res.success) {
          this.ticket.set(res.data);
          this.showSnack('Priority updated', 'success');
        }
      },
      error: () => this.isSaving.set(false)
    });
  }

  closeTicket(): void {
    const ref = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Close Ticket',
        message: 'Are you sure you want to close this ticket?',
        confirmText: 'Close Ticket',
        cancelText: 'Cancel',
        type: 'warning'
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;
      const ticketId = this.ticket()?.id!;
      // ✅ PUT /api/tickets/CloseTicket
      this.ticketService.close(ticketId).subscribe({
        next: (res) => {
          if (res.success) {
            this.ticket.set(res.data);
            this.showSnack('Ticket closed', 'success');
          }
        }
      });
    });
  }

  reopenTicket(): void {
    const ticketId = this.ticket()?.id!;
    // ✅ PUT /api/tickets/Ticket-reopen
    this.ticketService.reopen(ticketId).subscribe({
      next: (res) => {
        if (res.success) {
          this.ticket.set(res.data);
          this.showSnack('Ticket reopened', 'success');
        }
      }
    });
  }

  submitComment(): void {
    if (this.commentControl.invalid) return;
    const ticketId = this.ticket()?.id!;
    this.isCommenting.set(true);

    // ✅ POST /api/tickets/Add-Comment
    this.ticketService.addComment(
      ticketId,
      { content: this.commentControl.value! }
    ).subscribe({
      next: (res) => {
        this.isCommenting.set(false);
        if (res.success) {
          this.ticket.update(t =>
            t ? { ...t, comments: [...t.comments, res.data] } : t
          );
          this.commentControl.reset();
          this.showSnack('Comment added', 'success');
        }
      },
      error: () => this.isCommenting.set(false)
    });
  }

  isMyComment(userId: string): boolean {
    return this.auth.currentUserId() === userId;
  }

  showSnack(msg: string, type: 'success' | 'error'): void {
    this.snackBar.open(msg, 'Close', {
      duration: 3000,
      panelClass: type === 'success' ? ['snack-success'] : ['snack-error']
    });
  }

  goBack(): void { this.router.navigate(['/tickets']); }

  shortId(id: string): string {
    return '#' + id.substring(0, 8).toUpperCase();
  }
}