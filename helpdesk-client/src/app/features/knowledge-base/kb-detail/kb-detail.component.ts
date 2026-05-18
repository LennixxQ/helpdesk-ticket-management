import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { KnowledgeBaseService, KbArticleDetailModel } from '../../../core/services/knowledge-base.service';
import { AuthService } from '../../../core/services/auth.service';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-kb-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDividerModule,
    MatTooltipModule,
    MatDialogModule,
  ],
  templateUrl: './kb-detail.html',
  styleUrl: './kb-detail.scss'
})
export class KbDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private kbService = inject(KnowledgeBaseService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);
  auth = inject(AuthService);

  isLoading = signal(true);
  article = signal<KbArticleDetailModel | null>(null);
  showFeedback = signal(false);
  isSubmittingFeedback = signal(false);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadArticle(id);
    } else {
      this.router.navigate(['/kb']);
    }
  }

  loadArticle(id: string): void {
    this.isLoading.set(true);
    this.kbService.getById(id).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success) {
          this.article.set(res.data);
        } else {
          this.showSnack('Article not found', 'error');
          this.router.navigate(['/kb']);
        }
      },
      error: () => {
        this.isLoading.set(false);
        this.showSnack('Failed to load article', 'error');
        this.router.navigate(['/kb']);
      }
    });
  }

  submitFeedback(helpful: boolean): void {
    const articleData = this.article();
    if (!articleData) return;

    this.isSubmittingFeedback.set(true);
    this.kbService.submitFeedback(articleData.id, helpful).subscribe({
      next: (res) => {
        this.isSubmittingFeedback.set(false);
        this.showFeedback.set(false);
        if (res.success) {
          this.article.update(a => a ? {
            ...a,
            helpfulCount: helpful ? a.helpfulCount + 1 : a.helpfulCount,
            notHelpfulCount: helpful ? a.notHelpfulCount : a.notHelpfulCount + 1
          } : null);
          this.showSnack(helpful ? 'Thank you for your feedback!' : 'Thank you for letting us know', 'success');
        }
      },
      error: () => {
        this.isSubmittingFeedback.set(false);
        this.showSnack('Failed to submit feedback', 'error');
      }
    });
  }

  deleteArticle(): void {
    const articleData = this.article();
    if (!articleData) return;

    const ref = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Delete Article',
        message: `Delete "${articleData.title}"? This action cannot be undone.`,
        confirmText: 'Delete',
        cancelText: 'Cancel',
        type: 'danger'
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;

      this.kbService.delete(articleData.id).subscribe({
        next: (res) => {
          if (res.success) {
            this.showSnack('Article deleted', 'success');
            this.router.navigate(['/kb']);
          }
        },
        error: () => this.showSnack('Failed to delete article', 'error')
      });
    });
  }

  togglePublish(): void {
    const articleData = this.article();
    if (!articleData) return;

    const isPublishing = articleData.status === 1; // Draft status = 1

    this.kbService[isPublishing ? 'publish' : 'unpublish'](articleData.id).subscribe({
      next: (res) => {
        if (res.success) {
          this.article.update(a => a ? { ...a, status: isPublishing ? 2 : 1 } : null);
          this.showSnack(isPublishing ? 'Article published' : 'Article unpublished', 'success');
        }
      },
      error: () => this.showSnack('Failed to update article status', 'error')
    });
  }

  getStatusLabel(status: number): string {
    const labels: Record<number, string> = { 1: 'Draft', 2: 'Published', 3: 'Archived' };
    return labels[status] || 'Unknown';
  }

  getStatusClass(status: number): string {
    const classes: Record<number, string> = { 1: 'draft', 2: 'published', 3: 'archived' };
    return classes[status] || '';
  }

  formatDate(dateStr: string | undefined): string {
    if (!dateStr) return 'N/A';
    return new Date(dateStr).toLocaleDateString('en-US', {
      month: 'long', day: 'numeric', year: 'numeric'
    });
  }

  canEdit(): boolean {
    return this.auth.isAdmin() || this.auth.isAgent();
  }

  showSnack(msg: string, type: 'success' | 'error'): void {
    this.snackBar.open(msg, 'Close', {
      duration: 3000,
      panelClass: type === 'success' ? ['snack-success'] : ['snack-error']
    });
  }
}
