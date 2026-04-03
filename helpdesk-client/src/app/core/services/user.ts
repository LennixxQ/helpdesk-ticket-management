import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseResponse } from '../models/base-response.model';
import { UserModel, CreateUserRequest, UpdateRoleRequest } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
    private readonly apiUrl = `${environment.apiUrl}/users`;

    constructor(private http: HttpClient) { }

    create(request: CreateUserRequest): Observable<BaseResponse<UserModel>> {
        return this.http.post<BaseResponse<UserModel>>(this.apiUrl, request);
    }

    getAll(): Observable<BaseResponse<UserModel[]>> {
        return this.http.get<BaseResponse<UserModel[]>>(this.apiUrl);
    }

    getById(id: string): Observable<BaseResponse<UserModel>> {
        return this.http.get<BaseResponse<UserModel>>(`${this.apiUrl}/${id}`);
    }

    updateRole(id: string, request: UpdateRoleRequest): Observable<BaseResponse<UserModel>> {
        return this.http.put<BaseResponse<UserModel>>(`${this.apiUrl}/${id}/role`, request);
    }

    deactivate(id: string): Observable<BaseResponse<UserModel>> {
        return this.http.put<BaseResponse<UserModel>>(`${this.apiUrl}/${id}/deactivate`, {});
    }

    getActiveAgents(): Observable<BaseResponse<UserModel[]>> {
        return this.http.get<BaseResponse<UserModel[]>>(`${this.apiUrl}/agents/active`);
    }
}