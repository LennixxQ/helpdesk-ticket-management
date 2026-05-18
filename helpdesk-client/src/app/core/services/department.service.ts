import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

export interface DepartmentModel {
    id: string;
    name: string;
    isActive: boolean;
    createdAt: string;
    lastModifiedAt?: string;
    createdBy?: string;
    departmentHeadId?: string;
    departmentHeadName?: string;
}

@Injectable({ providedIn: 'root' })
export class DepartmentService {
    private http = inject(HttpClient);
    private api = `${environment.apiUrl}/departments`;

    getAll(): Observable<ApiResponse<DepartmentModel[]>> {
        return this.http.get<ApiResponse<DepartmentModel[]>>(`${this.api}/getAll`);
    }

    getById(id: string): Observable<ApiResponse<DepartmentModel>> {
        return this.http.post<ApiResponse<DepartmentModel>>(`${this.api}/getDeptById`, { id });
    }

    create(data: { name: string; departmentHeadId?: string }): Observable<ApiResponse<DepartmentModel>> {
        return this.http.post<ApiResponse<DepartmentModel>>(`${this.api}/create`, data);
    }

    update(data: { id: string; name: string }): Observable<ApiResponse<DepartmentModel>> {
        return this.http.post<ApiResponse<DepartmentModel>>(`${this.api}/update`, data);
    }

    deactivate(id: string): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/deactivate`, { id });
    }

    activate(id: string): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/activate`, { id });
    }

    assignHead(data: { departmentId: string; userId: string }): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/assignHead`, data);
    }
}