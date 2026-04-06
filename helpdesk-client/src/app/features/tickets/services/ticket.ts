import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse, PagedResult } from '../../../core/models/api-response.model';
import { TicketModel, CreateTicketRequest, TicketFilterParams } from '../../../core/models/ticket.model';
import { CommentModel, AddCommentRequest } from '../../../core/models/comment.model';

@Injectable({ providedIn: 'root' })
export class TicketService {
  private readonly api = `${environment.apiUrl}/tickets`;

  constructor(private http: HttpClient) { }

  // POST /api/tickets/createTicket
  create(req: CreateTicketRequest): Observable<ApiResponse<TicketModel>> {
    return this.http.post<ApiResponse<TicketModel>>(`${this.api}/createTicket`, req);
  }

  // POST /api/tickets/GetAllTicket  (filters in body)
  getAll(f: TicketFilterParams = {}): Observable<ApiResponse<PagedResult<TicketModel>>> {
    let params = new HttpParams();

    // ✅ [FromQuery] hai — query params mein denge
    if (f.page) params = params.set('page', f.page);
    if (f.pageSize) params = params.set('pageSize', f.pageSize);
    if (f.status) params = params.set('status', f.status);
    if (f.priority) params = params.set('priority', f.priority);
    if (f.categoryId) params = params.set('categoryId', f.categoryId);
    if (f.agentId) params = params.set('agentId', f.agentId);

    return this.http.post<ApiResponse<PagedResult<TicketModel>>>(
      `${this.api}/GetAllTicket`,
      {},          // ← empty body (POST hai but data query mein hai)
      { params }   // ← query params
    );
  }

  // GET /api/tickets/getByIdTicket?ticketId=xxx
  getById(ticketId: string): Observable<ApiResponse<TicketModel>> {
    const params = new HttpParams().set('ticketId', ticketId);
    return this.http.get<ApiResponse<TicketModel>>(
      `${this.api}/getByIdTicket`, { params }
    );
  }

  // PUT /api/tickets/Agent-assign  (ticketId + agentId in body)
  assign(ticketId: string, agentId: string): Observable<ApiResponse<TicketModel>> {
    return this.http.put<ApiResponse<TicketModel>>(
      `${this.api}/Agent-assign`, { ticketId, agentId }
    );
  }

  // PUT /api/tickets/UpdateTicketStatus  (ticketId + newStatus in body)
  updateStatus(ticketId: string, newStatus: string): Observable<ApiResponse<TicketModel>> {
    return this.http.put<ApiResponse<TicketModel>>(
      `${this.api}/UpdateTicketStatus`, { ticketId, newStatus }
    );
  }

  // PUT /api/tickets/UpdatePriority  (ticketId + priority in body)
  updatePriority(ticketId: string, priority: string): Observable<ApiResponse<TicketModel>> {
    return this.http.put<ApiResponse<TicketModel>>(
      `${this.api}/UpdatePriority`, { ticketId, priority }
    );
  }

  // PUT /api/tickets/CloseTicket  (ticketId in body)
  close(ticketId: string): Observable<ApiResponse<TicketModel>> {
    return this.http.put<ApiResponse<TicketModel>>(
      `${this.api}/CloseTicket`, { ticketId }
    );
  }

  // PUT /api/tickets/Ticket-reopen  (ticketId in body)
  reopen(ticketId: string): Observable<ApiResponse<TicketModel>> {
    return this.http.put<ApiResponse<TicketModel>>(
      `${this.api}/Ticket-reopen`, { ticketId }
    );
  }

  // POST /api/tickets/Add-Comment  (ticketId + content in body)
  addComment(ticketId: string, req: AddCommentRequest): Observable<ApiResponse<CommentModel>> {
    return this.http.post<ApiResponse<CommentModel>>(
      `${this.api}/Add-Comment`, { ticketId, content: req.content }
    );
  }
}