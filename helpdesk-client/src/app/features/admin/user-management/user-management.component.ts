import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatPaginatorModule, MatPaginator, PageEvent } from '@angular/material/paginator';
import { ViewChild, HostListener } from '@angular/core';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { UserModel, UserRole } from '../../../core/models/user.model';
import { AdminService } from '../../../core/services/admin.service';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDialogModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatChipsModule,
    MatSortModule,
    MatPaginatorModule,
  ],
  templateUrl: './user-management.html',
  styleUrl: './user-management.scss'
})
export class UserManagementComponent implements OnInit {
  private adminService = inject(AdminService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);
  private fb = inject(FormBuilder);

  @ViewChild(MatSort) sort!: MatSort;

  // ── State ──────────────────────────────────────────────────
  isLoading = signal(true);
  isSaving = signal(false);
  showForm = signal(false);
  users = signal<UserModel[]>([]);
  dataSource = new MatTableDataSource<UserModel>();

  isMobile = signal(window.innerWidth < 768);

  @HostListener('window:resize')
  onResize() { this.isMobile.set(window.innerWidth < 768); }

  get displayedColumns(): string[] {
    return this.isMobile()
      ? ['avatar', 'name', 'role', 'actions']
      : ['avatar', 'name', 'email', 'role', 'status', 'createdAt', 'actions'];
  }

  // ── Pagination ─────────────────────────────────────────────
  pageIndex = signal(0);
  pageSize = signal(10);

  get pagedUsers(): UserModel[] {
    const start = this.pageIndex() * this.pageSize();
    return this.users().slice(start, start + this.pageSize());
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
  }

  readonly roles: UserRole[] = ['Admin', 'Agent', 'User'];

  // ── Stats ──────────────────────────────────────────────────
  totalUsers = computed(() => this.users().length);
  totalAdmins = computed(() => this.users().filter(u => u.role === 'Admin').length);
  totalAgents = computed(() => this.users().filter(u => u.role === 'Agent').length);
  totalActive = computed(() => this.users().filter(u => u.isActive).length);

  readonly statsCards = [
    { label: 'Total Users', value: this.totalUsers, icon: 'group', color: '#7B9EE5', bg: '#EEF3FC' },
    { label: 'Admins', value: this.totalAdmins, icon: 'admin_panel_settings', color: '#B5A4E8', bg: '#F3EEFF' },
    { label: 'Agents', value: this.totalAgents, icon: 'support_agent', color: '#82D8C8', bg: '#E8F8F5' },
    { label: 'Active', value: this.totalActive, icon: 'check_circle', color: '#1A8C5A', bg: '#D6F5E8' },
  ];

  // ── Form ───────────────────────────────────────────────────
  form: FormGroup = this.fb.group({
    fullName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    role: ['User', Validators.required],
  });

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading.set(true);
    // ✅ GET /api/users/getAll
    this.adminService.getAllUsers().subscribe({
      next: (res) => {
        if (res.success) {
          this.users.set(res.data);
          this.dataSource.data = res.data;
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  // ── Create User ────────────────────────────────────────────
  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isSaving.set(true);
    this.adminService.createUser(this.form.value).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        if (res.success) {
          this.showSnack('User created successfully', 'success');
          this.form.reset({ role: 'User' });
          this.showForm.set(false);
          this.loadUsers();
        } else {
          this.showSnack(res.message, 'error');
        }
      },
      error: () => {
        this.isSaving.set(false);
        this.showSnack('Failed to create user', 'error');
      }
    });
  }

  // ── Update Role ────────────────────────────────────────────
  updateRole(user: UserModel, newRole: UserRole): void {
    if (user.role === newRole) return;
    // ✅ PUT /api/users/UpdateUsersRole
    this.adminService.updateRole(user.id, { newRole }).subscribe({
      next: (res) => {
        if (res.success) {
          this.users.update(list =>
            list.map(u => u.id === user.id ? { ...u, role: newRole } : u)
          );
          this.dataSource.data = this.users();
          this.showSnack(`Role updated to ${newRole}`, 'success');
        }
      }
    });
  }

  // ── Deactivate ─────────────────────────────────────────────
  deactivate(user: UserModel): void {
    const ref = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Deactivate User',
        message: `Are you sure you want to deactivate ${user.fullName}?`,
        confirmText: 'Deactivate',
        cancelText: 'Cancel',
        type: 'danger'
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;
      // ✅ PUT /api/users/DeleteUser
      this.adminService.deactivate(user.id).subscribe({
        next: (res) => {
          if (res.success) {
            this.users.update(list =>
              list.map(u => u.id === user.id ? { ...u, isActive: false } : u)
            );
            this.dataSource.data = this.users();
            this.showSnack('User deactivated', 'success');
          }
        }
      });
    });
  }

  // ── Helpers ────────────────────────────────────────────────
  applyFilter(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.dataSource.filter = value.trim().toLowerCase();
  }

  roleColor(role: UserRole): string {
    const map: Record<UserRole, string> = {
      Admin: '#7B9EE5',
      Agent: '#82D8C8',
      User: '#F2A7C3'
    };
    return map[role];
  }

  roleBg(role: UserRole): string {
    const map: Record<UserRole, string> = {
      Admin: '#EEF3FC',
      Agent: '#E8F8F5',
      User: '#FDE8F0'
    };
    return map[role];
  }

  getInitial(name: string): string {
    return name?.charAt(0)?.toUpperCase() ?? '?';
  }

  showSnack(msg: string, type: 'success' | 'error'): void {
    this.snackBar.open(msg, 'Close', {
      duration: 3000,
      panelClass: type === 'success' ? ['snack-success'] : ['snack-error']
    });
  }
}