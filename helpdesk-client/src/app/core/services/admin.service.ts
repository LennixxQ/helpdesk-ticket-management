import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { UserModel, CreateUserRequest, UpdateRoleRequest } from '../models/user.model';
import { CategoryModel, CreateCategoryRequest } from '../models/category.model';
import { DashboardModel } from '../models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class AdminService {
    private readonly usersApi = `${environment.apiUrl}/users`;
    private readonly categoriesApi = `${environment.apiUrl}/categories`;
    private readonly dashboardApi = `${environment.apiUrl}/dashboard`;

    constructor(private http: HttpClient) { }

    // ── Users ─────────────────────────────────────────────────
    createUser(req: CreateUserRequest): Observable<ApiResponse<UserModel>> {
        return this.http.post<ApiResponse<UserModel>>(this.usersApi, req);
    }
    getAllUsers(): Observable<ApiResponse<UserModel[]>> {
        return this.http.get<ApiResponse<UserModel[]>>(this.usersApi);
    }
    updateRole(id: string, req: UpdateRoleRequest): Observable<ApiResponse<UserModel>> {
        return this.http.put<ApiResponse<UserModel>>(`${this.usersApi}/${id}/role`, req);
    }
    deactivate(id: string): Observable<ApiResponse<UserModel>> {
        return this.http.put<ApiResponse<UserModel>>(`${this.usersApi}/${id}/deactivate`, {});
    }
    getActiveAgents(): Observable<ApiResponse<UserModel[]>> {
        return this.http.get<ApiResponse<UserModel[]>>(`${this.usersApi}/agents/active`);
    }

    // ── Categories ────────────────────────────────────────────
    getCategories(): Observable<ApiResponse<CategoryModel[]>> {
        return this.http.get<ApiResponse<CategoryModel[]>>(this.categoriesApi);
    }
    createCategory(req: CreateCategoryRequest): Observable<ApiResponse<CategoryModel>> {
        return this.http.post<ApiResponse<CategoryModel>>(this.categoriesApi, req);
    }
    toggleCategory(id: string): Observable<ApiResponse<CategoryModel>> {
        return this.http.put<ApiResponse<CategoryModel>>(`${this.categoriesApi}/${id}/toggle`, {});
    }

    // ── Dashboard ─────────────────────────────────────────────
    getDashboard(): Observable<ApiResponse<DashboardModel>> {
        return this.http.get<ApiResponse<DashboardModel>>(this.dashboardApi);
    }
}