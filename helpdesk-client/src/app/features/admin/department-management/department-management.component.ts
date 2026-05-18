import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { DepartmentService, DepartmentModel } from '../../../core/services/department.service';
import { UserService, UserModel } from '../../../core/services/user';
import { UserRole } from '../../../core/models/user.model';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-department-management',
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
    MatTableModule,
    MatPaginatorModule,
    MatDialogModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatSlideToggleModule,
    MatChipsModule,
  ],
  templateUrl: './department-management.html',
  styleUrl: './department-management.scss'
})
export class DepartmentManagementComponent implements OnInit {
  private departmentService = inject(DepartmentService);
  private userService = inject(UserService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);

  isLoading = signal(true);
  isSaving = signal(false);
  showForm = signal(false);
  departments = signal<DepartmentModel[]>([]);
  users = signal<UserModel[]>([]);

  pageIndex = signal(0);
  pageSize = signal(10);

  displayedColumns = ['name', 'departmentHead', 'status', 'createdAt', 'actions'];

  filteredDepartments = computed(() => this.departments());

  pagedDepartments = computed(() => {
    const start = this.pageIndex() * this.pageSize();
    return this.filteredDepartments().slice(start, start + this.pageSize());
  });

  totalCount = computed(() => this.departments().length);
  activeCount = computed(() => this.departments().filter(d => d.isActive).length);
  inactiveCount = computed(() => this.departments().filter(d => !d.isActive).length);

  form = new FormGroup({
    id: new FormControl(''),
    name: new FormControl('', [Validators.required, Validators.minLength(2)]),
    departmentHeadId: new FormControl('')
  });

  get isEditing(): boolean {
    return !!this.form.get('id')?.value;
  }

  ngOnInit(): void {
    this.loadDepartments();
    this.loadUsers();
  }

  loadDepartments(): void {
    this.isLoading.set(true);
    this.departmentService.getAll().subscribe({
      next: (res) => {
        if (res.success) this.departments.set(res.data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  loadUsers(): void {
    this.userService.getAll().subscribe({
      next: (res) => {
        console.log('Users API response:', res);
        if (res.success && res.data) {
          // First try to filter by Admin, Agent, DepartmentHead roles
          let filteredUsers = res.data.filter((u: UserModel) => {
            console.log('User:', u.fullName, 'Role:', u.role, 'Type:', typeof u.role);
            const roleNum = typeof u.role === 'string' ? parseInt(u.role) : u.role;
            return u.isActive && (roleNum === 1 || roleNum === 2 || roleNum === 4);
          });

          // If no users with those roles, show all active users (for debugging)
          if (filteredUsers.length === 0) {
            console.log('No users with Admin/Agent/DepartmentHead role, showing all active users');
            filteredUsers = res.data.filter((u: UserModel) => u.isActive);
          }

          console.log('Filtered users:', filteredUsers);
          this.users.set(filteredUsers);
        }
      },
      error: (err) => {
        console.error('Error loading users:', err);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
  }

  openForm(dept?: DepartmentModel): void {
    if (dept) {
      this.form.patchValue({
        id: dept.id,
        name: dept.name,
        departmentHeadId: dept.departmentHeadId || ''
      });
    } else {
      this.form.reset();
    }
    this.showForm.set(true);
  }

  closeForm(): void {
    this.form.reset();
    this.showForm.set(false);
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.isSaving.set(true);
    const data = this.form.value;
    const hasHeadChanged = data.departmentHeadId !== undefined;

    if (data.id) {
      const id = data.id as string;
      this.departmentService.update({ id, name: data.name! }).subscribe({
        next: (res) => {
          if (res.success) {
            if (data.departmentHeadId) {
              this.departmentService.assignHead({ departmentId: id, userId: data.departmentHeadId }).subscribe({
                next: (headRes) => {
                  this.isSaving.set(false);
                  if (headRes.success) {
                    const headName = this.users().find(u => u.id === data.departmentHeadId)?.fullName;
                    this.departments.update(list =>
                      list.map(d => d.id === id ? { ...d, departmentHeadId: data.departmentHeadId || undefined, departmentHeadName: headName } : d)
                    );
                    this.showSnack('Department updated with head', 'success');
                    this.closeForm();
                  }
                },
                error: () => { this.isSaving.set(false); this.showSnack('Failed to assign head', 'error'); }
              });
            } else {
              this.isSaving.set(false);
              this.departments.update(list =>
                list.map(d => d.id === id ? res.data : d)
              );
              this.showSnack('Department updated', 'success');
              this.closeForm();
            }
          } else {
            this.isSaving.set(false);
            this.showSnack('Failed to update', 'error');
          }
        },
        error: () => { this.isSaving.set(false); this.showSnack('Failed to update', 'error'); }
      });
    } else {
      const createData: { name: string; departmentHeadId?: string } = { name: data.name! };
      if (data.departmentHeadId) {
        createData.departmentHeadId = data.departmentHeadId;
      }
      this.departmentService.create(createData).subscribe({
        next: (res) => {
          if (res.success) {
            if (data.departmentHeadId) {
              this.departmentService.assignHead({ departmentId: res.data.id, userId: data.departmentHeadId }).subscribe({
                next: () => {
                  this.isSaving.set(false);
                  const headName = this.users().find(u => u.id === data.departmentHeadId)?.fullName;
                  this.departments.update(list => [{ ...res.data, departmentHeadId: data.departmentHeadId || undefined, departmentHeadName: headName }, ...list]);
                  this.showSnack('Department created with head', 'success');
                  this.closeForm();
                },
                error: () => { this.isSaving.set(false); this.showSnack('Failed to assign head', 'error'); }
              });
            } else {
              this.isSaving.set(false);
              this.departments.update(list => [res.data, ...list]);
              this.showSnack('Department created', 'success');
              this.closeForm();
            }
          } else {
            this.isSaving.set(false);
            this.showSnack('Failed to create', 'error');
          }
        },
        error: () => { this.isSaving.set(false); this.showSnack('Failed to create', 'error'); }
      });
    }
  }

  toggleStatus(dept: DepartmentModel): void {
    const ref = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: dept.isActive ? 'Deactivate Department' : 'Activate Department',
        message: `Are you sure you want to ${dept.isActive ? 'deactivate' : 'activate'} "${dept.name}"?`,
        confirmText: dept.isActive ? 'Deactivate' : 'Activate',
        cancelText: 'Cancel',
        type: 'warning'
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;
      const action = dept.isActive ? 'deactivate' : 'activate';
      const serviceCall = dept.isActive
        ? this.departmentService.deactivate(dept.id)
        : this.departmentService.activate(dept.id);

      serviceCall.subscribe({
        next: (res) => {
          if (res.success) {
            this.departments.update(list =>
              list.map(d => d.id === dept.id ? { ...d, isActive: !d.isActive } : d)
            );
            this.showSnack(`Department ${action}d`, 'success');
          }
        }
      });
    });
  }

  showSnack(msg: string, type: 'success' | 'error'): void {
    this.snackBar.open(msg, 'Close', {
      duration: 3000,
      panelClass: type === 'success' ? ['snack-success'] : ['snack-error']
    });
  }

  getStatusColor(isActive: boolean): string {
    return isActive ? '#059669' : '#DC2626';
  }

  getStatusBg(isActive: boolean): string {
    return isActive ? '#D1FAE5' : '#FEE2E2';
  }

  getRoleName(role: UserRole | string | number): string {
    const roleNum = typeof role === 'string' ? parseInt(role) : role;
    switch (roleNum) {
      case 1: return 'Admin';
      case 2: return 'Agent';
      case 3: return 'User';
      case 4: return 'Dept Head';
      default: return 'Unknown';
    }
  }
}