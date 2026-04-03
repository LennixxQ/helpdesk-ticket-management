import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseResponse } from '../models/base-response.model';
import { DashboardModel } from '../models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
    private readonly apiUrl = `${environment.apiUrl}/dashboard`;

    constructor(private http: HttpClient) { }

    getDashboard(): Observable<BaseResponse<DashboardModel>> {
        return this.http.get<BaseResponse<DashboardModel>>(this.apiUrl);
    }
}