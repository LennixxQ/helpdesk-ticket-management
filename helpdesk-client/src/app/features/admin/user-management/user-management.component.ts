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
import { ViewChild, HostListener, TemplateRef } from '@angular/core';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { UserModel, UserRole } from '../../../core/models/user.model';
import { AdminService } from '../../../core/services/admin.service';
import { DepartmentService, DepartmentModel } from '../../../core/services/department.service';
import { forkJoin, Observable } from 'rxjs';

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
  private departmentService = inject(DepartmentService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);
  private fb = inject(FormBuilder);

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild('editDialog') editDialog!: TemplateRef<any>;
  private dialogRef: any;

  // ── State ──────────────────────────────────────────────────
  isLoading = signal(true);
  isSaving = signal(false);
  showForm = signal(false);
  users = signal<UserModel[]>([]);
  departments = signal<DepartmentModel[]>([]);
  searchQuery = signal('');

  isMobile = signal(window.innerWidth < 768);

  @HostListener('window:resize')
  onResize() { this.isMobile.set(window.innerWidth < 768); }

  get displayedColumns(): string[] {
    return this.isMobile()
      ? ['avatar', 'name', 'role', 'actions']
      : ['avatar', 'name', 'email', 'role', 'status', 'createdAt', 'actions'];
  }

  // ── Search & Pagination ───────────────────────────────────
  pageIndex = signal(0);
  pageSize = signal(10);

  filteredUsers = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    if (!query) return this.users();

    return this.users().filter(u =>
      u.fullName.toLowerCase().includes(query) ||
      u.email.toLowerCase().includes(query)
    );
  });

  pagedUsers = computed(() => {
    const start = this.pageIndex() * this.pageSize();
    return this.filteredUsers().slice(start, start + this.pageSize());
  });

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
  }

  protected readonly UserRole = UserRole;

  readonly roles: { value: UserRole; label: string }[] = [
    { value: UserRole.Admin, label: 'Admin' },
    { value: UserRole.Agent, label: 'Agent' },
    { value: UserRole.User, label: 'User' },
    { value: UserRole.DepartmentHead, label: 'Department Head' }
  ];

  // ── Stats ──────────────────────────────────────────────────
  totalUsers = computed(() => this.users().length);
  totalAdmins = computed(() => this.users().filter(u => u.role === UserRole.Admin).length);
  totalAgents = computed(() => this.users().filter(u => u.role === UserRole.Agent).length);
  totalActive = computed(() => this.users().filter(u => u.isActive).length);

  readonly statsCards = [
    { label: 'Total Users', value: this.totalUsers, icon: 'group', color: '#3B82F6', bg: '#DBEAFE' },
    { label: 'Admins', value: this.totalAdmins, icon: 'admin_panel_settings', color: '#8B5CF6', bg: '#EDE9FE' },
    { label: 'Agents', value: this.totalAgents, icon: 'support_agent', color: '#0D9488', bg: '#CCFBF1' },
    { label: 'Active', value: this.totalActive, icon: 'check_circle', color: '#059669', bg: '#D1FAE5' },
  ];

  // ── Form ───────────────────────────────────────────────────
  form: FormGroup = this.fb.group({
    id: [''],
    fullName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    role: [UserRole.User, Validators.required],
    departmentId: ['']
  });

  ngOnInit(): void {
    this.loadUsers();
    this.loadDepartments();
  }

  loadDepartments(): void {
    this.departmentService.getAll().subscribe({
      next: (res) => {
        if (res.success) this.departments.set(res.data);
      }
    });
  }

  loadUsers(): void {
    this.isLoading.set(true);
    // ✅ GET /api/users/getAll
    this.adminService.getAllUsers().subscribe({
      next: (res) => {
        if (res.success) {
          this.users.set(res.data);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  // ── Form Triggers ──────────────────────────────────────────
  openCreateForm(): void {
    this.form.reset({ role: UserRole.User, id: '', departmentId: '' });
    this.form.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.form.get('password')?.updateValueAndValidity();
    this.showForm.set(true);
  }

  openEditForm(user: UserModel): void {
    this.form.patchValue({
      id: user.id,
      fullName: user.fullName,
      email: user.email,
      role: user.role,
      departmentId: user.departmentId || ''
    });
    this.form.get('password')?.clearValidators();
    this.form.get('password')?.setValue('');
    this.form.get('password')?.updateValueAndValidity();
    
    this.dialogRef = this.dialog.open(this.editDialog, {
      width: '550px',
      maxWidth: '95vw',
      panelClass: 'edit-user-dialog-container'
    });
  }

  closeDialog(): void {
    if (this.dialogRef) {
      this.dialogRef.close();
    }
    this.form.reset({ role: UserRole.User, id: '', departmentId: '' });
  }

  // ── Create User (Inline Form) ──────────────────────────────
  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isSaving.set(true);

    const data = this.form.value;
    this.adminService.createUser(data).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        if (res.success) {
          this.showSnack('User created successfully', 'success');
          this.form.reset({ role: UserRole.User });
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

  // ── Update User (Dialog Form) ──────────────────────────────
  onDialogSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isSaving.set(true);

    const data = this.form.value;
    const originalUser = this.users().find(u => u.id === data.id);
    if (!originalUser) {
      this.isSaving.set(false);
      return;
    }

    // Check if fields actually changed
    const roleChanged = originalUser.role !== data.role;
    const deptChanged = (originalUser.departmentId || '') !== (data.departmentId || '');

    const updates: Observable<any>[] = [];
    if (roleChanged) {
      updates.push(this.adminService.updateRole(data.id, { newRole: data.role }));
    }
    if (deptChanged) {
      updates.push(this.adminService.moveDepartment(data.id, data.departmentId || '00000000-0000-0000-0000-000000000000'));
    }

    if (updates.length === 0) {
      this.isSaving.set(false);
      this.closeDialog();
      this.showSnack('No changes detected', 'success');
      return;
    }

    forkJoin(updates).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.showSnack('User updated successfully', 'success');
        this.closeDialog();
        this.loadUsers();
      },
      error: (err) => {
        this.isSaving.set(false);
        this.showSnack(err?.error?.message || 'Failed to update user', 'error');
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
            this.showSnack('User deactivated', 'success');
          }
        }
      });
    });
  }

  // ── Helpers ────────────────────────────────────────────────
  applyFilter(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchQuery.set(value);
    this.pageIndex.set(0); // Reset to first page on search
  }

  roleColor(role: UserRole | string | number): string {
    const map: Record<number, string> = {
      [UserRole.Admin]: '#3B82F6',
      [UserRole.Agent]: '#0D9488',
      [UserRole.User]: '#8B5CF6',
      [UserRole.DepartmentHead]: '#EA580C'
    };
    const roleNum = typeof role === 'string' ? parseInt(role) : role;
    return map[roleNum] || '#8B5CF6';
  }

  roleBg(role: UserRole | string | number): string {
    const map: Record<number, string> = {
      [UserRole.Admin]: '#DBEAFE',
      [UserRole.Agent]: '#CCFBF1',
      [UserRole.User]: '#EDE9FE',
      [UserRole.DepartmentHead]: '#FFEDD5'
    };
    const roleNum = typeof role === 'string' ? parseInt(role) : role;
    return map[roleNum] || '#EDE9FE';
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