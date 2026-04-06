import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
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

    // POST /api/users
    createUser(req: CreateUserRequest): Observable<ApiResponse<UserModel>> {
        return this.http.post<ApiResponse<UserModel>>(this.usersApi, req);
    }

    // GET /api/users/getAll
    getAllUsers(): Observable<ApiResponse<UserModel[]>> {
        return this.http.get<ApiResponse<UserModel[]>>(`${this.usersApi}/getAll`);
    }

    // GET /api/users/getById?userId=xxx
    getUserById(userId: string): Observable<ApiResponse<UserModel>> {
        const params = new HttpParams().set('userId', userId);
        return this.http.get<ApiResponse<UserModel>>(
            `${this.usersApi}/getById`, { params }
        );
    }

    // PUT /api/users/UpdateUsersRole  (userId + newRole in body)
    updateRole(userId: string, req: UpdateRoleRequest): Observable<ApiResponse<UserModel>> {
        return this.http.put<ApiResponse<UserModel>>(
            `${this.usersApi}/UpdateUsersRole`, { userId, ...req }
        );
    }

    // PUT /api/users/DeleteUser  (userId in body)
    deactivate(userId: string): Observable<ApiResponse<UserModel>> {
        return this.http.put<ApiResponse<UserModel>>(
            `${this.usersApi}/DeleteUser`, { userId }
        );
    }

    // GET /api/users/agents/active
    getActiveAgents(): Observable<ApiResponse<UserModel[]>> {
        return this.http.get<ApiResponse<UserModel[]>>(
            `${this.usersApi}/agents/active`
        );
    }

    // ── Categories ────────────────────────────────────────────

    // GET /api/categories
    getCategories(): Observable<ApiResponse<CategoryModel[]>> {
        return this.http.get<ApiResponse<CategoryModel[]>>(this.categoriesApi);
    }

    // POST /api/categories
    createCategory(req: CreateCategoryRequest): Observable<ApiResponse<CategoryModel>> {
        return this.http.post<ApiResponse<CategoryModel>>(this.categoriesApi, req);
    }

    // PUT /api/categories/ActivateCategory  (categoryId in body)
    toggleCategory(categoryId: string): Observable<ApiResponse<CategoryModel>> {
        return this.http.put<ApiResponse<CategoryModel>>(
            `${this.categoriesApi}/ActivateCategory`, { categoryId }
        );
    }

    // ── Dashboard ─────────────────────────────────────────────

    // GET /api/dashboard
    getDashboard(): Observable<ApiResponse<DashboardModel>> {
        return this.http.get<ApiResponse<DashboardModel>>(this.dashboardApi);
    }
}