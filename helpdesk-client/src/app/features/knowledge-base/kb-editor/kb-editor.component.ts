import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { KnowledgeBaseService, KbArticleDetailModel } from '../../../core/services/knowledge-base.service';
import { CategoryService } from '../../../core/services/category.service';

@Component({
  selector: 'app-kb-editor',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDividerModule,
    MatChipsModule,
    MatAutocompleteModule,
  ],
  templateUrl: './kb-editor.html',
  styleUrl: './kb-editor.scss'
})
export class KbEditorComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private kbService = inject(KnowledgeBaseService);
  private categoryService = inject(CategoryService);
  private snackBar = inject(MatSnackBar);

  isEditMode = signal(false);
  isLoading = signal(false);
  isSaving = signal(false);
  articleId = signal<string | null>(null);
  categories = signal<any[]>([]);

  editorForm!: FormGroup;

  ngOnInit(): void {
    this.editorForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(5)]],
      content: ['', [Validators.required, Validators.minLength(20)]],
      categoryId: ['', Validators.required],
      tags: [''],
    });

    this.loadCategories();

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.articleId.set(id);
      this.loadArticle(id);
    }
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (res) => {
        if (res.success) this.categories.set(res.data);
      }
    });
  }

  loadArticle(id: string): void {
    this.isLoading.set(true);
    this.kbService.getById(id).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success) {
          const article = res.data;
          this.editorForm.patchValue({
            title: article.title,
            content: article.content,
            categoryId: article.categoryId,
            tags: article.tags || ''
          });
        }
      },
      error: () => {
        this.isLoading.set(false);
        this.showSnack('Failed to load article', 'error');
        this.router.navigate(['/kb']);
      }
    });
  }

  save(): void {
    if (this.editorForm.invalid) {
      this.showSnack('Please fill in all required fields', 'error');
      return;
    }

    this.isSaving.set(true);
    const formValue = this.editorForm.value;

    const data = {
      title: formValue.title,
      content: formValue.content,
      categoryId: formValue.categoryId,
      tags: formValue.tags || ''
    };

    const request = this.isEditMode()
      ? this.kbService.update({ id: this.articleId()!, ...data })
      : this.kbService.create(data);

    request.subscribe({
      next: (res) => {
        this.isSaving.set(false);
        if (res.success) {
          this.showSnack(
            this.isEditMode() ? 'Article updated successfully' : 'Article created successfully',
            'success'
          );
          const redirectId = this.articleId() || res.data?.id;
          this.router.navigate(['/kb', redirectId || 'list']);
        }
      },
      error: () => {
        this.isSaving.set(false);
        this.showSnack('Failed to save article', 'error');
      }
    });
  }

  cancel(): void {
    if (this.isEditMode()) {
      this.router.navigate(['/kb', this.articleId()]);
    } else {
      this.router.navigate(['/kb']);
    }
  }

  showSnack(msg: string, type: 'success' | 'error'): void {
    this.snackBar.open(msg, 'Close', {
      duration: 3000,
      panelClass: type === 'success' ? ['snack-success'] : ['snack-error']
    });
  }

  getCategoryEmoji(categoryName: string): string {
    const map: Record<string, string> = {
      'Hardware': '🖥️',
      'Software': '📀',
      'Network': '📡',
      'HR': '👥',
      'General': '📋',
      'Security': '🔒'
    };
    return map[categoryName] || '📁';
  }

  getTagsCount(): number {
    const tags = this.editorForm.get('tags')?.value || '';
    if (!tags.trim()) return 0;
    return tags.split(',').filter((t: string) => t.trim()).length;
  }
}
