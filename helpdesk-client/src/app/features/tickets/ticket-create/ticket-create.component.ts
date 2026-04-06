import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/services/auth.service';
import { CategoryModel } from '../../../core/models/category.model';
import { UserModel } from '../../../core/models/user.model';
import { TicketPriority } from '../../../core/models/ticket.model';
import { TicketService } from '../../../core/services/ticket.service';
import { AdminService } from '../../../core/services/admin.service';

@Component({
  selector: 'app-ticket-create',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
  ],
  templateUrl: './ticket-create.html',
  styleUrl: './ticket-create.scss'
})
export class TicketCreateComponent implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private ticketService = inject(TicketService);
  private adminService = inject(AdminService);
  private snackBar = inject(MatSnackBar);
  auth = inject(AuthService);

  isLoading = signal(false);
  categories = signal<CategoryModel[]>([]);
  users = signal<UserModel[]>([]);

  readonly priorities: { value: TicketPriority; label: string; icon: string }[] = [
    { value: 'Low', label: 'Low', icon: 'arrow_downward' },
    { value: 'Medium', label: 'Medium', icon: 'remove' },
    { value: 'High', label: 'High', icon: 'arrow_upward' },
    { value: 'Critical', label: 'Critical', icon: 'priority_high' },
  ];

  form: FormGroup = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(150)]],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    categoryId: ['', Validators.required],
    priority: ['', Validators.required],
    raisedByUserId: [''],
  });

  get titleLength(): number {
    return this.form.get('title')?.value?.length ?? 0;
  }

  get descLength(): number {
    return this.form.get('description')?.value?.length ?? 0;
  }

  getSelectedPriority() {
    const val = this.form.get('priority')?.value;
    return this.priorities.find(p => p.value === val);
  }

  ngOnInit(): void {
    this.loadCategories();
    if (this.auth.isAdmin()) this.loadUsers();
  }

  loadCategories(): void {
    this.adminService.getCategories().subscribe({
      next: (res) => {
        if (res.success)
          this.categories.set(res.data.filter(c => c.isActive));
      }
    });
  }

  loadUsers(): void {
    this.adminService.getAllUsers().subscribe({
      next: (res) => {
        if (res.success) {
          const currentId = this.auth.currentUserId();
          this.users.set(res.data.filter(u => u.isActive && u.id !== currentId));
        }
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isLoading.set(true);

    const payload = { ...this.form.value };
    if (!this.auth.isAdmin()) delete payload.raisedByUserId;
    if (!payload.raisedByUserId) delete payload.raisedByUserId;

    this.ticketService.create(payload).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success) {
          this.snackBar.open('Ticket created successfully! 🎉', 'Close', {
            duration: 3000,
            panelClass: ['snack-success']
          });
          this.router.navigate(['/tickets', res.data.id]);
        } else {
          this.snackBar.open(res.message, 'Close', {
            duration: 4000,
            panelClass: ['snack-error']
          });
        }
      },
      error: () => {
        this.isLoading.set(false);
        this.snackBar.open('Failed to create ticket', 'Close', {
          duration: 4000,
          panelClass: ['snack-error']
        });
      }
    });
  }

  cancel(): void { this.router.navigate(['/tickets']); }
}