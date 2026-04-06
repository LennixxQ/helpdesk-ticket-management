import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { LoginRequest, UserRole } from '../models/user.model';
import { StorageService } from './storage.service';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  // ✅ ASP.NET Core Identity ke exact claim names
  sub?: string;
  email?: string;
  unique_name?: string;
  given_name?: string;

  // ✅ Role claim — multiple possible names
  role?: string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'?: string;

  exp?: number;
  [key: string]: unknown;   // extra claims ignore karo
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private storage = inject(StorageService);
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private _token = signal<string | null>(this.storage.getToken());

  readonly isLoggedIn = computed(() => !!this._token());

  readonly currentUser = computed(() => {
    const t = this._token();
    if (!t) return null;
    try { return jwtDecode<JwtPayload>(t); }
    catch { return null; }
  });

  // ✅ Role — multiple claim names try karo
  readonly currentRole = computed((): UserRole | null => {
    const u = this.currentUser();
    if (!u) return null;

    const role =
      u['role'] ??
      u['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
      null;

    return role as UserRole | null;
  });

  // ✅ UserId — sub claim
  readonly currentUserId = computed(() => {
    const u = this.currentUser();
    if (!u) return null;
    return (u['sub'] ?? u['nameid'] ?? null) as string | null;
  });

  // ✅ Name — multiple claim names try karo
  readonly currentUserName = computed(() => {
    const u = this.currentUser();
    if (!u) return null;

    return (
      u['unique_name'] ??
      u['given_name'] ??
      u['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ??
      u['name'] ??
      u['email'] ??
      null
    ) as string | null;
  });

  login(request: LoginRequest): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(
      `${this.apiUrl}/login`, request
    ).pipe(
      tap(res => {
        if (res.success && res.data) {
          this.storage.setToken(res.data);
          this._token.set(res.data);

          // 🔍 Debug — console mein dekho kya aa raha hai
          console.log('JWT Payload:', jwtDecode(res.data));
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