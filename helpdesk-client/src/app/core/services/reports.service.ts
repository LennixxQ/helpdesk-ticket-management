import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api-response.model';

export interface ReportFilter {
    from?: string;
    to?: string;
    priority?: string;
    status?: string;
    agentId?: string;
    categoryId?: string;
    departmentId?: string;
}

// Backend returns ReportDataDto (single object)
export interface TicketVolumeReport {
    totalTickets: number;
    byStatus: Record<string, number>;
    byPriority: Record<string, number>;
    byCategory: Record<string, number>;
    byDay: Record<string, number>;
}

// Backend returns AgentPerformanceReportDto (single object)
export interface AgentPerformanceReport {
    agentId: string;
    agentName: string;
    totalResolved: number;
    avgResolutionHours: number;
    slaCompliancePct: number;
    csatScore: number;
}

// Backend returns SlaComplianceReportDto (single object)
export interface SlaComplianceReport {
    totalTickets: number;
    withinSla: number;
    breached: number;
    compliancePct: number;
}

@Injectable({ providedIn: 'root' })
export class ReportsService {
    private http = inject(HttpClient);
    private api = `${environment.apiUrl}/reports`;

    getTicketVolume(filter: ReportFilter): Observable<ApiResponse<TicketVolumeReport>> {
        return this.http.post<ApiResponse<TicketVolumeReport>>(`${this.api}/ticketVolume`, {
            From: filter.from || new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
            To: filter.to || new Date().toISOString()
        });
    }

    getAgentPerformance(filter: ReportFilter): Observable<any> {
        return this.http.post(`${this.api}/agentPerformance`, {
            AgentId: filter.agentId || null,
            Filter: {
                From: filter.from || new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
                To: filter.to || new Date().toISOString()
            }
        });
    }

    getSlaCompliance(filter: ReportFilter): Observable<ApiResponse<SlaComplianceReport>> {
        return this.http.post<ApiResponse<SlaComplianceReport>>(`${this.api}/slaCompliance`, {
            From: filter.from || new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
            To: filter.to || new Date().toISOString()
        });
    }

    getAgentSelf(): Observable<any> {
        return this.http.get(`${this.api}/agentSelf`);
    }

    getTopAgent(): Observable<any> {
        return this.http.get(`${this.api}/topAgent`);
    }

    exportTicketsCsv(filter: ReportFilter): Observable<Blob> {
        return this.http.post(`${this.api}/exportTicketsCsv`, {
            From: filter.from || new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
            To: filter.to || new Date().toISOString()
        }, { responseType: 'blob' });
    }

    exportTicketsPdf(filter: ReportFilter): Observable<Blob> {
        return this.http.post(`${this.api}/exportTicketsPdf`, {
            From: filter.from || new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
            To: filter.to || new Date().toISOString()
        }, { responseType: 'blob' });
    }

    exportAuditCsv(filter: ReportFilter): Observable<Blob> {
        return this.http.post(`${this.api}/exportAuditCsv`, {
            From: filter.from || new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
            To: filter.to || new Date().toISOString()
        }, { responseType: 'blob' });
    }
}