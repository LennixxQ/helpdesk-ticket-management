import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

export interface NotificationPreferenceModel {
    eventType: number;
    eventName: string;
    isEnabled: boolean;
    description: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationPreferencesService {
    private http = inject(HttpClient);
    private api = `${environment.apiUrl}/notification-preferences`;

    getMine(): Observable<ApiResponse<NotificationPreferenceModel[]>> {
        return this.http.get<ApiResponse<NotificationPreferenceModel[]>>(`${this.api}/getMine`);
    }

    upsert(eventType: number, isEnabled: boolean): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/upsert`, { eventType, isEnabled });
    }
}