import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

export interface RecurringTemplateModel {
    id: string;
    templateName: string;
    ticketTitle: string;
    priority: number;
    recurrencePattern: number;
    isActive: boolean;
    nextRunAt?: string;
    lastRunAt?: string;
    runCount: number;
    categoryName: string;
}

export interface CreateTemplateRequest {
    templateName: string;
    ticketTitle: string;
    description: string;
    categoryId: string;
    priority: number;
    recurrencePattern: number;
}

@Injectable({ providedIn: 'root' })
export class RecurringTemplatesService {
    private http = inject(HttpClient);
    private api = `${environment.apiUrl}/recurring-templates`;

    getAll(): Observable<ApiResponse<RecurringTemplateModel[]>> {
        return this.http.get<ApiResponse<RecurringTemplateModel[]>>(`${this.api}/getAll`);
    }

    getById(id: string): Observable<ApiResponse<RecurringTemplateModel>> {
        return this.http.post<ApiResponse<RecurringTemplateModel>>(`${this.api}/getById`, { id });
    }

    create(data: CreateTemplateRequest): Observable<ApiResponse<RecurringTemplateModel>> {
        return this.http.post<ApiResponse<RecurringTemplateModel>>(`${this.api}/create`, data);
    }

    toggleActive(id: string): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/toggleActive`, { id });
    }

    triggerManual(id: string): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/triggerManual`, { id });
    }

    delete(id: string): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/delete`, { id });
    }
}