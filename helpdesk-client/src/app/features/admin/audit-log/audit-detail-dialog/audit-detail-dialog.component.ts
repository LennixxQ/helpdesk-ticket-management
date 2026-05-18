import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { AuditLogModel } from '../../../../core/services/audit.service';

@Component({
  selector: 'app-audit-detail-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatIconModule,
    MatButtonModule,
    MatDividerModule
  ],
  templateUrl: './audit-detail-dialog.html',
  styleUrl: './audit-detail-dialog.scss'
})
export class AuditDetailDialogComponent {
  data = inject<AuditLogModel>(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<AuditDetailDialogComponent>);

  close(): void {
    this.dialogRef.close();
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

  copyToClipboard(text: string): void {
    navigator.clipboard.writeText(text);
  }

  formatJson(text: string | null | undefined): string {
    if (!text) return 'No additional details';
    try {
      const parsed = JSON.parse(text);
      return JSON.stringify(parsed, null, 2);
    } catch {
      return text;
    }
  }
}