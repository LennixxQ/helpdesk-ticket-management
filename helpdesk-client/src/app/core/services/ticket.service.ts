import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TicketModel, CreateTicketRequest, TicketFilterParams } from '../models/ticket.model';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import { CommentModel, AddCommentRequest } from '../models/comment.model';

@Injectable({ providedIn: 'root' })
export class TicketService {
    private readonly api = `${environment.apiUrl}/tickets`;

    constructor(private http: HttpClient) { }

    create(req: CreateTicketRequest): Observable<ApiResponse<TicketModel>> {
        return this.http.post<ApiResponse<TicketModel>>(this.api, req);
    }

    getAll(f: TicketFilterParams = {}): Observable<ApiResponse<PagedResult<TicketModel>>> {
        let p = new HttpParams();
        if (f.page) p = p.set('page', f.page);
        if (f.pageSize) p = p.set('pageSize', f.pageSize);
        if (f.status) p = p.set('status', f.status);
        if (f.priority) p = p.set('priority', f.priority);
        if (f.categoryId) p = p.set('categoryId', f.categoryId);
        if (f.agentId) p = p.set('agentId', f.agentId);
        return this.http.get<ApiResponse<PagedResult<TicketModel>>>(this.api, { params: p });
    }

    getById(id: string): Observable<ApiResponse<TicketModel>> {
        return this.http.get<ApiResponse<TicketModel>>(`${this.api}/${id}`);
    }

    assign(id: string, agentId: string): Observable<ApiResponse<TicketModel>> {
        return this.http.put<ApiResponse<TicketModel>>(`${this.api}/${id}/assign`, { agentId });
    }

    updateStatus(id: string, newStatus: string): Observable<ApiResponse<TicketModel>> {
        return this.http.put<ApiResponse<TicketModel>>(`${this.api}/${id}/status`, { newStatus });
    }

    updatePriority(id: string, priority: string): Observable<ApiResponse<TicketModel>> {
        return this.http.put<ApiResponse<TicketModel>>(`${this.api}/${id}/priority`, { priority });
    }

    close(id: string): Observable<ApiResponse<TicketModel>> {
        return this.http.put<ApiResponse<TicketModel>>(`${this.api}/${id}/close`, {});
    }

    reopen(id: string): Observable<ApiResponse<TicketModel>> {
        return this.http.put<ApiResponse<TicketModel>>(`${this.api}/${id}/reopen`, {});
    }

    addComment(id: string, req: AddCommentRequest): Observable<ApiResponse<CommentModel>> {
        return this.http.post<ApiResponse<CommentModel>>(`${this.api}/${id}/comments`, req);
    }
}