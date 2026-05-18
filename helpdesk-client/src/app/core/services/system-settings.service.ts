import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

export interface SystemSettingModel {
    key: string;
    value: string;
    description?: string;
}

@Injectable({ providedIn: 'root' })
export class SystemSettingsService {
    private http = inject(HttpClient);
    private api = `${environment.apiUrl}/SystemSettings`;

    getAll(): Observable<ApiResponse<SystemSettingModel[]>> {
        return this.http.get<ApiResponse<SystemSettingModel[]>>(`${this.api}/getAll`);
    }

    getByKey(key: string): Observable<ApiResponse<SystemSettingModel>> {
        return this.http.post<ApiResponse<SystemSettingModel>>(`${this.api}/getByKey`, { key });
    }

    update(key: string, value: string): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/update`, { key, value });
    }

    sendTestEmail(email: string): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/sendTestEmail`, { toEmail: email });
    }
}