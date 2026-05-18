import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { KnowledgeBaseService, KbArticleModel } from '../../../core/services/knowledge-base.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-kb-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatTooltipModule,
  ],
  templateUrl: './kb-list.html',
  styleUrl: './kb-list.scss'
})
export class KbListComponent implements OnInit {
  private kbService = inject(KnowledgeBaseService);
  private router = inject(Router);
  auth = inject(AuthService);

  isLoading = signal(true);
  articles = signal<KbArticleModel[]>([]);
  searchControl = new FormControl('');
  selectedCategory = signal<string>('');

  categories = computed(() => {
    const cats = new Set(this.articles().map(a => a.categoryName).filter(Boolean));
    return ['All', ...Array.from(cats)];
  });

  filteredArticles = computed(() => {
    let result = this.articles();
    const search = this.searchControl.value?.toLowerCase() || '';
    const category = this.selectedCategory();

    if (search) {
      result = result.filter(a => a.title.toLowerCase().includes(search));
    }

    if (category && category !== 'All') {
      result = result.filter(a => a.categoryName === category);
    }

    return result;
  });

  ngOnInit(): void {
    this.loadArticles();
  }

  loadArticles(): void {
    this.isLoading.set(true);
    this.kbService.getAll().subscribe({
      next: (res) => {
        if (res.success) this.articles.set(res.data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  searchArticles(): void {
    const keyword = this.searchControl.value;
    if (!keyword) {
      this.loadArticles();
      return;
    }
    this.isLoading.set(true);
    this.kbService.search(keyword).subscribe({
      next: (res) => {
        if (res.success) this.articles.set(res.data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  selectCategory(cat: string): void {
    this.selectedCategory.set(cat === 'All' ? '' : cat);
  }

  viewArticle(id: string): void {
    this.router.navigate(['/kb', id]);
  }

  getCategoryIcon(category: string): string {
    const map: Record<string, string> = {
      'Hardware': 'computer',
      'Software': 'apps',
      'Network': 'wifi',
      'HR': 'people',
      'General': 'info',
      'Security': 'security'
    };
    return map[category] || 'article';
  }

  getCategoryColor(category: string): string {
    const map: Record<string, { bg: string; color: string }> = {
      'Hardware': { bg: '#DBEAFE', color: '#3B82F6' },
      'Software': { bg: '#D1FAE5', color: '#059669' },
      'Network': { bg: '#FEF3C7', color: '#D97706' },
      'HR': { bg: '#F3E8FF', color: '#7C3AED' },
      'General': { bg: '#E2E8F0', color: '#475569' },
      'Security': { bg: '#FEE2E2', color: '#DC2626' }
    };
    return map[category]?.bg || '#E2E8F0';
  }

  getCategoryBgColor(category: string): string {
    return this.getCategoryColor(category);
  }

  getCategoryTextColor(category: string): string {
    const map: Record<string, string> = {
      'Hardware': '#3B82F6',
      'Software': '#059669',
      'Network': '#D97706',
      'HR': '#7C3AED',
      'General': '#475569',
      'Security': '#DC2626'
    };
    return map[category] || '#475569';
  }

  getCategoryEmoji(category: string): string {
    const map: Record<string, string> = {
      'Hardware': '🖥️',
      'Software': '📀',
      'Network': '📡',
      'HR': '👥',
      'General': '📋',
      'Security': '🔒'
    };
    return map[category] || '📁';
  }

  getTotalViews(): number {
    return this.articles().reduce((sum, a) => sum + (a.viewCount || 0), 0);
  }

  getCategoryCount(category: string): number {
    return this.articles().filter(a => a.categoryName === category).length;
  }
}