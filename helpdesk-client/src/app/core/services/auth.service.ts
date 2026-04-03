import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { LoginRequest, UserRole } from '../models/user.model';
import { jwtDecode } from 'jwt-decode';
import { StorageService } from './storage.service';

interface JwtPayload {
  sub: string;
  email: string;
  unique_name: string;
  role: UserRole;
  exp: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private _token = signal<string | null>(null);

  readonly isLoggedIn = computed(() => !!this._token());
  readonly currentUser = computed(() => {
    const t = this._token();
    if (!t) return null;
    try { return jwtDecode<JwtPayload>(t); } catch { return null; }
  });
  readonly currentRole = computed(() => this.currentUser()?.role ?? null);
  readonly currentUserId = computed(() => this.currentUser()?.sub ?? null);
  readonly currentUserName = computed(() => this.currentUser()?.unique_name ?? null);

  constructor(
    private http: HttpClient,
    private router: Router,
    private storage: StorageService
  ) { this._token.set(this.storage.getToken()) }

  login(request: LoginRequest): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/login`, request).pipe(
      tap(res => {
        if (res.success && res.data) {
          this.storage.setToken(res.data);
          this._token.set(res.data);
        }
      })
    );
  }

  logout(): void {
    this.storage.clearToken();
    this._token.set(null);
    this.router.navigate(['/login']);
  }

  getToken = () => this._token();
  isAdmin = () => this.currentRole() === 'Admin';
  isAgent = () => this.currentRole() === 'Agent';
  isUser = () => this.currentRole() === 'User';

  redirectByRole(): void {
    const role = this.currentRole();
    if (role === 'Admin') this.router.navigate(['/dashboard']);
    else if (role === 'Agent') this.router.navigate(['/agent/tickets']);
    else this.router.navigate(['/tickets']);
  }
}