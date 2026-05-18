import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '../../core/services/auth.service';
import { UserRole } from '../../core/models/user.model';
import { NotificationPreferencesService, NotificationPreferenceModel } from '../../core/services/notification-preferences.service';

interface NotificationGroup {
  title: string;
  icon: string;
  color: string;
  types: number[];
  mandatoryAdmin?: boolean;
}

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatSlideToggleModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatTooltipModule,
    MatDividerModule,
  ],
  templateUrl: './notifications.html',
  styleUrl: './notifications.scss'
})
export class NotificationsComponent implements OnInit {
  private prefService = inject(NotificationPreferencesService);
  private snackBar = inject(MatSnackBar);
  auth = inject(AuthService);

  isLoading = signal(true);
  preferences = signal<NotificationPreferenceModel[]>([]);

  // Helper to check admin role
  isAdmin(): boolean {
    return this.auth.currentRole() === UserRole.Admin;
  }

  readonly notificationGroups: NotificationGroup[] = [
    {
      title: 'Ticket Updates',
      icon: 'confirmation_number',
      color: '#3B82F6',
      types: [1, 2, 3, 4] // TicketCreated, TicketAssigned, TicketStatusChanged, CommentAdded
    },
    {
      title: 'Ticket Lifecycle',
      icon: 'autorenew',
      color: '#8B5CF6',
      types: [5, 6, 15] // TicketClosed, TicketReopened, AgentReassigned
    },
    {
      title: 'SLA & Escalation',
      icon: 'warning',
      color: '#F59E0B',
      types: [7, 8, 9], // TicketEscalated, SlaWarning, SlaBreached
      mandatoryAdmin: true
    },
    {
      title: 'Account & Security',
      icon: 'security',
      color: '#10B981',
      types: [11, 12, 14] // AccountCreated, AccountDeactivated, PasswordChanged
    },
    {
      title: 'Surveys',
      icon: 'rate_review',
      color: '#EC4899',
      types: [13] // SurveyRequest
    }
  ];

  readonly eventNames: Record<number, { name: string; description: string; icon: string }> = {
    // Ticket Updates (Group 1)
    1: { name: 'Ticket Created', description: 'Receive confirmation when you create a new ticket', icon: 'add_circle' },
    2: { name: 'Ticket Assigned', description: 'Get notified when a ticket is assigned to you', icon: 'assignment_ind' },
    3: { name: 'Status Changed', description: 'Updates when ticket status changes (In Progress, Resolved, etc.)', icon: 'swap_horiz' },
    4: { name: 'Comment Added', description: 'Notifications when comments are added to your tickets', icon: 'comment' },

    // Ticket Lifecycle (Group 2)
    5: { name: 'Ticket Closed', description: 'Confirmation when a ticket is closed', icon: 'check_circle' },
    6: { name: 'Ticket Reopened', description: 'Alert when a closed ticket is reopened', icon: 'replay' },
    15: { name: 'Agent Reassigned', description: 'Notify when ticket is assigned to a different agent', icon: 'person_swap' },

    // SLA & Escalation (Group 3) - Mandatory for Admin
    7: { name: 'Ticket Escalated', description: 'Urgent alerts for escalated tickets requiring senior attention', icon: 'priority_high' },
    8: { name: 'SLA Warning', description: 'Reminder at 75% SLA deadline elapsed', icon: 'schedule' },
    9: { name: 'SLA Breached', description: 'Critical alert when SLA resolution time is exceeded', icon: 'error' },

    // Account & Security (Group 4)
    11: { name: 'Account Created', description: 'Welcome email with login credentials', icon: 'person_add' },
    12: { name: 'Account Deactivated', description: 'Notification when an account is deactivated', icon: 'person_remove' },
    14: { name: 'Password Changed', description: 'Security alert when password is updated', icon: 'lock' },

    // Surveys (Group 5)
    13: { name: 'Survey Request', description: 'CSAT survey request after ticket closure (opt-out available)', icon: 'star_rate' }
  };

  ngOnInit(): void {
    this.loadPreferences();
  }

  loadPreferences(): void {
    this.isLoading.set(true);
    this.prefService.getMine().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.preferences.set(res.data);
        } else {
          this.preferences.set([]);
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.preferences.set([]);
        this.isLoading.set(false);
      }
    });
  }

  toggle(pref: NotificationPreferenceModel): void {
    const isAdmin = this.auth.currentRole() === UserRole.Admin;
    const mandatoryEvents = [7, 9]; // TicketEscalated, SlaBreached

    // PRD 5.3 — Admin cannot disable SLA breach + escalation alerts
    if (isAdmin && mandatoryEvents.includes(pref.eventType) && !pref.isEnabled) {
      this.showSnack('SLA breach and escalation notifications are mandatory for Admin', 'error');
      return;
    }

    this.prefService.upsert(pref.eventType, !pref.isEnabled).subscribe({
      next: (res) => {
        if (res.success) {
          this.preferences.update(list =>
            list.map(p => p.eventType === pref.eventType ? { ...p, isEnabled: !p.isEnabled } : p)
          );
          this.showSnack('Preference updated successfully', 'success');
        }
      },
      error: () => this.showSnack('Failed to update preference', 'error')
    });
  }

  getPrefsForGroup(types: number[]): NotificationPreferenceModel[] {
    return (this.preferences() || []).filter(p => types.includes(p.eventType));
  }

  isMandatoryForAdmin(eventType: number): boolean {
    return this.isAdmin() && [7, 9].includes(eventType);
  }

  getEventIcon(eventType: number): string {
    return this.eventNames[eventType]?.icon || 'notifications';
  }

  getEventName(eventType: number): string {
    return this.eventNames[eventType]?.name || 'Unknown Event';
  }

  getEventDescription(eventType: number): string {
    return this.eventNames[eventType]?.description || '';
  }

  showSnack(msg: string, type: 'success' | 'error'): void {
    this.snackBar.open(msg, 'Close', {
      duration: 3000,
      panelClass: type === 'success' ? ['snack-success'] : ['snack-error']
    });
  }
}