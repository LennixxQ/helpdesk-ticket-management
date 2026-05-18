import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { MatSelectModule } from '@angular/material/select';
import { MatExpansionModule } from '@angular/material/expansion';
import { SystemSettingsService, SystemSettingModel } from '../../../core/services/system-settings.service';

interface SettingsGroup {
  title: string;
  icon: string;
  color: string;
  settings: SystemSettingModel[];
}

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSlideToggleModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatTooltipModule,
    MatDividerModule,
    MatSelectModule,
    MatExpansionModule,
  ],
  templateUrl: './settings.html',
  styleUrl: './settings.scss'
})
export class SettingsComponent implements OnInit {
  private settingsService = inject(SystemSettingsService);
  private snackBar = inject(MatSnackBar);

  isLoading = signal(true);
  allSettings = signal<SystemSettingModel[]>([]);
  savingKey = signal<string | null>(null);
  expandedPanel = signal<string>('email');
  testEmailControl = new FormControl('', [Validators.required, Validators.email]);
  isSendingTest = signal(false);

  settingsGroups = signal<SettingsGroup[]>([
    {
      title: 'Email Configuration',
      icon: 'email',
      color: '#3B82F6',
      settings: []
    },
    {
      title: 'Ticket Settings',
      icon: 'confirmation_number',
      color: '#8B5CF6',
      settings: []
    },
    {
      title: 'SLA Configuration',
      icon: 'timer',
      color: '#F59E0B',
      settings: []
    },
    {
      title: 'Other Settings',
      icon: 'settings',
      color: '#64748B',
      settings: []
    }
  ]);

  ngOnInit(): void {
    this.loadSettings();
  }

  loadSettings(): void {
    this.isLoading.set(true);
    this.settingsService.getAll().subscribe({
      next: (res) => {
        if (res.success) {
          this.allSettings.set(res.data);
          this.groupSettings(res.data);
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.showSnack('Failed to load settings', 'error');
      }
    });
  }

  private groupSettings(settings: SystemSettingModel[]): void {
    const emailPatterns = ['smtp', 'email'];
    const ticketPatterns = ['ticket', 'attachment', 'priority', 'reopen'];
    const slaPatterns = ['sla', 'escalation', 'deadline'];

    const groups = this.settingsGroups();

    groups[0].settings = settings.filter(s =>
      emailPatterns.some(p => s.key.toLowerCase().includes(p))
    );

    groups[1].settings = settings.filter(s =>
      ticketPatterns.some(p => s.key.toLowerCase().includes(p))
    );

    groups[2].settings = settings.filter(s =>
      slaPatterns.some(p => s.key.toLowerCase().includes(p))
    );

    groups[3].settings = settings.filter(s =>
      !emailPatterns.some(p => s.key.toLowerCase().includes(p)) &&
      !ticketPatterns.some(p => s.key.toLowerCase().includes(p)) &&
      !slaPatterns.some(p => s.key.toLowerCase().includes(p))
    );

    this.settingsGroups.set([...groups]);

    // Auto-expand first non-empty group
    const firstNonEmpty = groups.findIndex(g => g.settings.length > 0);
    if (firstNonEmpty >= 0) {
      this.expandedPanel.set(['email', 'ticket', 'sla', 'other'][firstNonEmpty]);
    }
  }

  updateSetting(setting: SystemSettingModel, newValue: string): void {
    if (setting.value === newValue) return;

    this.savingKey.set(setting.key);
    setting.value = newValue;

    this.settingsService.update(setting.key, newValue).subscribe({
      next: (res) => {
        this.savingKey.set(null);
        if (res.success) {
          this.showSnack('Setting updated successfully', 'success');
        }
      },
      error: () => {
        this.savingKey.set(null);
        this.showSnack('Failed to update setting', 'error');
      }
    });
  }

  onToggleChange(setting: SystemSettingModel, checked: boolean): void {
    this.updateSetting(setting, checked.toString());
  }

  onInputChange(setting: SystemSettingModel, event: Event): void {
    const input = event.target as HTMLInputElement;
    this.updateSetting(setting, input.value);
  }

  onNumberChange(setting: SystemSettingModel, event: Event): void {
    const input = event.target as HTMLInputElement;
    this.updateSetting(setting, input.value);
  }

  sendTestEmail(): void {
    if (this.testEmailControl.invalid) {
      this.showSnack('Please enter a valid email address', 'error');
      return;
    }

    this.isSendingTest.set(true);
    this.settingsService.sendTestEmail(this.testEmailControl.value!).subscribe({
      next: (res) => {
        this.isSendingTest.set(false);
        if (res.success) {
          this.showSnack('Test email sent successfully!', 'success');
        }
      },
      error: () => {
        this.isSendingTest.set(false);
        this.showSnack('Failed to send test email', 'error');
      }
    });
  }

  isBoolValue(value: string): boolean {
    return value === 'true' || value === 'false';
  }

  isNumericValue(value: string): boolean {
    return !isNaN(Number(value)) && !this.isBoolValue(value);
  }

  getDisplayName(key: string): string {
    return key
      .replace(/([A-Z])/g, ' $1')
      .replace(/^./, str => str.toUpperCase())
      .replace(/Sla/gi, 'SLA')
      .replace(/Smtp/gi, 'SMTP')
      .replace(/Id/gi, 'ID')
      .trim();
  }

  getInputType(setting: SystemSettingModel): string {
    if (this.isNumericValue(setting.value)) return 'number';
    if (this.isBoolValue(setting.value)) return 'boolean';
    return 'text';
  }

  showSnack(msg: string, type: 'success' | 'error'): void {
    this.snackBar.open(msg, 'Close', {
      duration: 3000,
      panelClass: type === 'success' ? ['snack-success'] : ['snack-error']
    });
  }
}
