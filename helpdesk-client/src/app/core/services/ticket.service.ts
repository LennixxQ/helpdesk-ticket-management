import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TicketModel, CreateTicketRequest, TicketFilterParams, TicketStatus, TicketPriority } from '../models/ticket.model';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import { CommentModel, AddCommentRequest } from '../models/comment.model';

@Injectable({ providedIn: 'root' })
export class TicketService {
    private readonly api = `${environment.apiUrl}/tickets`;

    constructor(private http: HttpClient) { }

    // POST /api/tickets/createTicket
    create(req: CreateTicketRequest): Observable<ApiResponse<TicketModel>> {
        return this.http.post<ApiResponse<TicketModel>>(`${this.api}/createTicket`, req);
    }

    // POST /api/tickets/GetAllTicket
    getAll(f: TicketFilterParams = {}): Observable<ApiResponse<PagedResult<TicketModel>>> {
        return this.http.post<ApiResponse<PagedResult<TicketModel>>>(
            `${this.api}/GetAllTicket`,
            { page: f.page, pageSize: f.pageSize, status: f.status, priority: f.priority, categoryId: f.categoryId, agentId: f.agentId }
        );
    }

    // POST /api/tickets/getByIdTicket
    getById(ticketId: string): Observable<ApiResponse<TicketModel>> {
        return this.http.post<ApiResponse<TicketModel>>(`${this.api}/getByIdTicket`, { id: ticketId });
    }

    // POST /api/tickets/Agent-assign
    assign(ticketId: string, agentId: string): Observable<ApiResponse<TicketModel>> {
        return this.http.post<ApiResponse<TicketModel>>(`${this.api}/Agent-assign`, { TicketId: ticketId, AgentId: agentId });
    }

    // POST /api/tickets/UpdateTicketStatus
    updateStatus(ticketId: string, newStatus: TicketStatus): Observable<ApiResponse<TicketModel>> {
        return this.http.post<ApiResponse<TicketModel>>(`${this.api}/UpdateTicketStatus`, { TicketId: ticketId, NewStatus: newStatus });
    }

    // POST /api/tickets/UpdatePriority
    updatePriority(ticketId: string, priority: TicketPriority): Observable<ApiResponse<TicketModel>> {
        return this.http.post<ApiResponse<TicketModel>>(`${this.api}/UpdatePriority`, { TicketId: ticketId, Priority: priority });
    }

    // PUT /api/tickets/CloseTicket
    close(ticketId: string): Observable<ApiResponse<TicketModel>> {
        return this.http.post<ApiResponse<TicketModel>>(`${this.api}/CloseTicket`, { id: ticketId });
    }

    // PUT /api/tickets/Ticket-reopen
    reopen(ticketId: string): Observable<ApiResponse<TicketModel>> {
        return this.http.post<ApiResponse<TicketModel>>(`${this.api}/Ticket-reopen`, { id: ticketId });
    }

    // POST /api/tickets/Add-Comment
    addComment(ticketId: string, req: AddCommentRequest): Observable<ApiResponse<CommentModel>> {
        return this.http.post<ApiResponse<CommentModel>>(`${this.api}/Add-Comment`, { ticketId, content: req.content });
    }

    // POST /api/tickets/escalate
    escalate(ticketId: string, reason: string): Observable<ApiResponse<TicketModel>> {
        return this.http.post<ApiResponse<TicketModel>>(`${this.api}/escalate`, { ticketId, reason });
    }

    // POST /api/tickets/markResolvedViaKb
    markResolvedViaKb(ticketId: string, articleId: string): Observable<ApiResponse<TicketModel>> {
        return this.http.post<ApiResponse<TicketModel>>(`${this.api}/markResolvedViaKb`, { ticketId, articleId });
    }
}