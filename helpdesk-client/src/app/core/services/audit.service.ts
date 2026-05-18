import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api-response.model';

export interface AuditLogModel {
    id: string;
    entityName: string;        // Backend: EntityName
    entityId: string;
    action: string;
    performedBy: string;       // Backend: PerformedBy (user ID)
    actorName: string;         // Backend: ActorName
    actorEmail?: string;       // Backend: ActorEmail
    actorRole?: string;        // Backend: ActorRole
    performedAt: string;      // Backend: PerformedAt
    ipAddress?: string;
    additionalNotes?: string;
    details?: AuditLogDetail[];
}

export interface AuditLogDetail {
    fieldName: string;
    oldValue?: string;
    newValue?: string;
}

export interface AuditFilter {
    from?: string;
    to?: string;
    actor?: string;
    action?: string;
    entityType?: string;
    page?: number;
    pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class AuditService {
    private http = inject(HttpClient);
    private api = `${environment.apiUrl}/audit`;

    getAll(filter: AuditFilter): Observable<ApiResponse<PagedResult<AuditLogModel>>> {
        return this.http.post<ApiResponse<PagedResult<AuditLogModel>>>(`${this.api}/getAll`, {
            from: filter.from,
            to: filter.to,
            actor: filter.actor,
            action: filter.action,
            entityType: filter.entityType,
            page: filter.page || 1,
            pageSize: filter.pageSize || 20
        });
    }

    getByEntity(entityId: string): Observable<ApiResponse<AuditLogModel[]>> {
        return this.http.post<ApiResponse<AuditLogModel[]>>(`${this.api}/getByEntity`, { entityId });
    }
}