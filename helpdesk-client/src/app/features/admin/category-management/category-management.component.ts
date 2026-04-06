import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CategoryModel } from '../../../core/models/category.model';
import { AdminService } from '../../../core/services/admin.service';

@Component({
  selector: 'app-category-management',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
  ],
  templateUrl: './category-management.html',
  styleUrl: './category-management.scss'
})
export class CategoryManagementComponent implements OnInit {
  private adminService = inject(AdminService);
  private snackBar = inject(MatSnackBar);

  isLoading = signal(true);
  isSaving = signal(false);
  categories = signal<CategoryModel[]>([]);

  nameControl = new FormControl('', [
    Validators.required,
    Validators.maxLength(100)
  ]);

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading.set(true);
    this.adminService.getCategories().subscribe({
      next: (res) => {
        if (res.success) this.categories.set(res.data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  createCategory(): void {
    if (this.nameControl.invalid) { this.nameControl.markAsTouched(); return; }
    this.isSaving.set(true);
    this.adminService.createCategory({ name: this.nameControl.value! }).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        if (res.success) {
          this.categories.update(list => [...list, res.data]);
          this.nameControl.reset();
          this.showSnack('Category created', 'success');
        } else {
          this.showSnack(res.message, 'error');
        }
      },
      error: () => {
        this.isSaving.set(false);
        this.showSnack('Failed to create category', 'error');
      }
    });
  }

  toggle(category: CategoryModel): void {
    // ✅ PUT /api/categories/ActivateCategory  — categoryId in body
    this.adminService.toggleCategory(category.id).subscribe({
      next: (res) => {
        if (res.success) {
          this.categories.update(list =>
            list.map(c => c.id === category.id ? res.data : c)
          );
          this.showSnack(
            `Category ${res.data.isActive ? 'activated' : 'deactivated'}`,
            'success'
          );
        }
      }
    });
  }

  activeCount = () => this.categories().filter(c => c.isActive).length;
  inactiveCount = () => this.categories().filter(c => !c.isActive).length;

  showSnack(msg: string, type: 'success' | 'error'): void {
    this.snackBar.open(msg, 'Close', {
      duration: 3000,
      panelClass: type === 'success' ? ['snack-success'] : ['snack-error']
    });
  }
}