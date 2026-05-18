import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { AuthService } from '../../../core/services/auth.service';
import { RecurringTemplatesService, RecurringTemplateModel } from '../../../core/services/recurring-templates.service';
import { CategoryService } from '../../../core/services/category.service';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-recurring-templates',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    MatTooltipModule,
    MatChipsModule,
  ],
  templateUrl: './recurring-templates.html',
  styleUrl: './recurring-templates.scss'
})
export class RecurringTemplatesComponent implements OnInit {
  private templatesService = inject(RecurringTemplatesService);
  private categoryService = inject(CategoryService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);
  auth = inject(AuthService);

  isLoading = signal(true);
  templates = signal<RecurringTemplateModel[]>([]);
  categories = signal<any[]>([]);
  showCreateForm = signal(false);

  createForm = new FormGroup({
    templateName: new FormControl('', Validators.required),
    ticketTitle: new FormControl('', Validators.required),
    description: new FormControl(''),
    categoryId: new FormControl('', Validators.required),
    priority: new FormControl(1, Validators.required),
    recurrencePattern: new FormControl(1, Validators.required),
  });

  isSaving = signal(false);

  readonly recurrenceLabels: Record<number, string> = {
    1: 'Daily',
    2: 'Weekly',
    3: 'Monthly',
    4: 'Custom'
  };

  readonly priorityLabels: Record<number, string> = {
    1: 'Low',
    2: 'Medium',
    3: 'High',
    4: 'Critical'
  };

  readonly recurrenceIcons: Record<number, string> = {
    1: 'today',
    2: 'date_range',
    3: 'calendar_month',
    4: 'settings'
  };

  ngOnInit(): void {
    this.loadTemplates();
    this.loadCategories();
  }

  loadTemplates(): void {
    this.isLoading.set(true);
    this.templatesService.getAll().subscribe({
      next: (res) => {
        if (res.success) this.templates.set(res.data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (res) => {
        if (res.success) this.categories.set(res.data);
      }
    });
  }

  toggleActive(template: RecurringTemplateModel): void {
    this.templatesService.toggleActive(template.id).subscribe({
      next: (res) => {
        if (res.success) {
          this.templates.update(list =>
            list.map(t => t.id === template.id ? { ...t, isActive: !t.isActive } : t)
          );
          this.showSnack(`Template ${template.isActive ? 'deactivated' : 'activated'}`, 'success');
        }
      }
    });
  }

  triggerManual(template: RecurringTemplateModel): void {
    const ref = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Trigger Template',
        message: `Manually trigger "${template.templateName}" now?`,
        confirmText: 'Trigger',
        cancelText: 'Cancel',
        type: 'info'
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;
      this.templatesService.triggerManual(template.id).subscribe({
        next: (res) => {
          if (res.success) {
            this.showSnack('Template triggered successfully', 'success');
            this.loadTemplates();
          }
        },
        error: () => this.showSnack('Failed to trigger template', 'error')
      });
    });
  }

  deleteTemplate(template: RecurringTemplateModel): void {
    const ref = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Delete Template',
        message: `Delete "${template.templateName}"? This cannot be undone.`,
        confirmText: 'Delete',
        cancelText: 'Cancel',
        type: 'danger'
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;
      this.templatesService.delete(template.id).subscribe({
        next: (res) => {
          if (res.success) {
            this.templates.update(list => list.filter(t => t.id !== template.id));
            this.showSnack('Template deleted', 'success');
          }
        },
        error: () => this.showSnack('Failed to delete template', 'error')
      });
    });
  }

  submitCreate(): void {
    if (this.createForm.invalid) return;
    this.isSaving.set(true);

    const formValue = {
      templateName: this.createForm.get('templateName')?.value,
      ticketTitle: this.createForm.get('ticketTitle')?.value,
      description: this.createForm.get('description')?.value || '',
      categoryId: this.createForm.get('categoryId')?.value,
      priority: this.createForm.get('priority')?.value,
      recurrencePattern: this.createForm.get('recurrencePattern')?.value
    };
    this.templatesService.create({
      templateName: formValue.templateName!,
      ticketTitle: formValue.ticketTitle!,
      description: formValue.description || '',
      categoryId: formValue.categoryId!,
      priority: formValue.priority!,
      recurrencePattern: formValue.recurrencePattern!
    }).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        if (res.success) {
          this.showCreateForm.set(false);
          this.createForm.reset({ priority: 1, recurrencePattern: 1 });
          this.loadTemplates();
          this.showSnack('Template created successfully', 'success');
        }
      },
      error: () => {
        this.isSaving.set(false);
        this.showSnack('Failed to create template', 'error');
      }
    });
  }

  formatDate(dateStr: string | undefined): string {
    if (!dateStr) return 'Never';
    return new Date(dateStr).toLocaleDateString('en-US', {
      month: 'short', day: 'numeric', year: 'numeric', hour: '2-digit', minute: '2-digit'
    });
  }

  showSnack(msg: string, type: 'success' | 'error'): void {
    this.snackBar.open(msg, 'Close', {
      duration: 3000,
      panelClass: type === 'success' ? ['snack-success'] : ['snack-error']
    });
  }
}

class FormGroup {
  controls: Record<string, FormControl>;
  value: Record<string, any> = {};
  constructor(initial: Record<string, any>) {
    this.controls = {};
    for (const [key, value] of Object.entries(initial)) {
      this.controls[key] = new FormControl(value);
    }
    this.updateValue();
  }
  get(key: string): FormControl { return this.controls[key]; }
  reset(value?: any): void {
    for (const key of Object.keys(this.controls)) {
      this.controls[key].setValue(value?.[key] ?? null);
    }
    this.updateValue();
  }
  private updateValue(): void {
    this.value = {};
    for (const [key, control] of Object.entries(this.controls)) {
      this.value[key] = control.value;
    }
  }
  get invalid(): boolean { return Object.values(this.controls).some(c => c.invalid); }
}