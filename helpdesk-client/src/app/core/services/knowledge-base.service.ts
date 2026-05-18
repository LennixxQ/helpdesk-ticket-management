import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

export interface KbArticleModel {
    id: string;
    title: string;
    categoryName: string;
    viewCount: number;
    status: number;
    authorName: string;
    lastModifiedAt?: string;
}

export interface KbArticleDetailModel {
    id: string;
    title: string;
    content: string;
    categoryId: string;
    categoryName: string;
    status: number;
    viewCount: number;
    authorId: string;
    authorName: string;
    createdAt: string;
    lastModifiedAt?: string;
    tags?: string;
    helpfulCount: number;
    notHelpfulCount: number;
}

export interface KbVersionHistoryModel {
    version: number;
    title: string;
    content: string;
    modifiedByName: string;
    modifiedAt: string;
    changeDescription?: string;
}

@Injectable({ providedIn: 'root' })
export class KnowledgeBaseService {
    private http = inject(HttpClient);
    private api = `${environment.apiUrl}/kb`;

    getAll(): Observable<ApiResponse<KbArticleModel[]>> {
        return this.http.get<ApiResponse<KbArticleModel[]>>(`${this.api}/getAll`);
    }

    getById(id: string): Observable<ApiResponse<KbArticleDetailModel>> {
        return this.http.post<ApiResponse<KbArticleDetailModel>>(`${this.api}/getById`, { id });
    }

    search(keyword: string): Observable<ApiResponse<KbArticleModel[]>> {
        return this.http.get<ApiResponse<KbArticleModel[]>>(`${this.api}/search?keyword=${encodeURIComponent(keyword)}`);
    }

    suggest(title: string): Observable<ApiResponse<string[]>> {
        return this.http.get<ApiResponse<string[]>>(`${this.api}/suggest?title=${encodeURIComponent(title)}`);
    }

    create(data: { title: string; content: string; categoryId: string; tags?: string }): Observable<ApiResponse<KbArticleModel>> {
        return this.http.post<ApiResponse<KbArticleModel>>(`${this.api}/create`, data);
    }

    update(data: { id: string; title: string; content: string; categoryId: string; tags?: string }): Observable<ApiResponse<KbArticleModel>> {
        return this.http.post<ApiResponse<KbArticleModel>>(`${this.api}/update`, data);
    }

    publish(id: string): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/publish`, { id });
    }

    unpublish(id: string): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/unpublish`, { id });
    }

    delete(id: string): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/delete`, { id });
    }

    submitFeedback(id: string, helpful: boolean): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/submitFeedback`, { articleId: id, isHelpful: helpful });
    }

    getHistory(id: string): Observable<ApiResponse<KbVersionHistoryModel[]>> {
        return this.http.get<ApiResponse<KbVersionHistoryModel[]>>(`${this.api}/history?id=${id}`);
    }

    restore(id: string, version: number): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/restore?id=${id}&versionNumber=${version}`, {});
    }
}
