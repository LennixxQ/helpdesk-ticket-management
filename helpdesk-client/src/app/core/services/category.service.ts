import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseResponse } from '../models/base-response.model';
import { CategoryModel, CreateCategoryRequest } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
    private readonly apiUrl = `${environment.apiUrl}/categories`;

    constructor(private http: HttpClient) { }

    getAll(): Observable<BaseResponse<CategoryModel[]>> {
        return this.http.get<BaseResponse<CategoryModel[]>>(this.apiUrl);
    }

    create(request: CreateCategoryRequest): Observable<BaseResponse<CategoryModel>> {
        return this.http.post<BaseResponse<CategoryModel>>(this.apiUrl, request);
    }

    toggle(id: string): Observable<BaseResponse<CategoryModel>> {
        return this.http.put<BaseResponse<CategoryModel>>(`${this.apiUrl}/${id}/toggle`, {});
    }
}