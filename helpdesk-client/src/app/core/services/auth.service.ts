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

  // ✅ MFA claim
  mfaEnabled?: boolean | string;

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

    if (role === 'Admin' || role === '1') return UserRole.Admin;
    if (role === 'Agent' || role === '2') return UserRole.Agent;
    if (role === 'User' || role === '3') return UserRole.User;
    if (role === 'DepartmentHead' || role === 'Department Head' || role === '4') return UserRole.DepartmentHead;

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
  
  // ✅ MFA — multiple claim names try karo
  readonly isMfaEnabledSignal = computed((): boolean => {
    const u = this.currentUser();
    if (!u) return false;

    const val =
      u['mfaEnabled'] ??
      u['mfa_enabled'] ??
      u['IsMfaEnabled'] ??
      u['isMfaEnabled'] ??
      u['http://schemas.microsoft.com/ws/2008/06/identity/claims/mfaEnabled'] ??
      u['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mfaEnabled'] ??
      null;

    return val === true || val === 'true';
  });

  private _profilePicTrigger = signal<number>(0);

  // ✅ Profile Picture URL from localStorage, keyed by userId
  readonly profilePic = computed((): string | null => {
    this._profilePicTrigger(); // reactivity hook
    const userId = this.currentUserId();
    if (!userId) return null;
    return localStorage.getItem(`profile_image_${userId}`);
  });

  constructor() {
    // If already logged in, fetch from server
    setTimeout(() => {
      if (this.isLoggedIn()) {
        this.fetchProfilePicFromServer();
      }
    }, 200);
  }

  private _uploadTimeout: any;

  uploadProfilePicToServer(base64Data: string | null, position: string): void {
    const userId = this.currentUserId();
    if (!userId) return;

    // Cache locally first for instant visual update
    if (base64Data) {
      localStorage.setItem(`profile_image_${userId}`, base64Data);
    } else {
      localStorage.removeItem(`profile_image_${userId}`);
    }
    localStorage.setItem(`profile_image_position_${userId}`, position);
    this._profilePicTrigger.update(n => n + 1);
    this._profilePicPositionTrigger.update(n => n + 1);

    // Debounce the server call by 400ms to avoid flooding while dragging sliders
    if (this._uploadTimeout) {
      clearTimeout(this._uploadTimeout);
    }

    this._uploadTimeout = setTimeout(() => {
      this.http.post<ApiResponse<boolean>>(
        `${environment.apiUrl}/users/profile-picture`,
        { base64Image: base64Data || '', position: position }
      ).subscribe({
        next: (res) => {
          if (res.success) {
            console.log('Profile picture synchronized with server.');
          }
        },
        error: (err) => {
          console.error('Failed to synchronize profile picture with server:', err);
        }
      });
    }, 400);
  }

  fetchProfilePicFromServer(): void {
    const userId = this.currentUserId();
    if (!userId) return;

    this.http.get<ApiResponse<{ base64Image: string | null, position: string }>>(
      `${environment.apiUrl}/users/profile-picture`
    ).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          const pic = res.data.base64Image;
          const pos = res.data.position || '50% 50%';
          
          if (pic) {
            localStorage.setItem(`profile_image_${userId}`, pic);
          } else {
            localStorage.removeItem(`profile_image_${userId}`);
          }
          localStorage.setItem(`profile_image_position_${userId}`, pos);

          this._profilePicTrigger.update(n => n + 1);
          this._profilePicPositionTrigger.update(n => n + 1);
        }
      },
      error: (err) => {
        console.error('Failed to fetch profile picture from server:', err);
      }
    });
  }

  setProfilePic(base64Data: string | null): void {
    const userId = this.currentUserId();
    if (userId) {
      const currentPos = this.profilePicPosition();
      this.uploadProfilePicToServer(base64Data, currentPos);
    }
  }

  private _profilePicPositionTrigger = signal<number>(0);

  // ✅ Profile Picture CSS object-position from localStorage, keyed by userId
  readonly profilePicPosition = computed((): string => {
    this._profilePicPositionTrigger(); // reactivity hook
    const userId = this.currentUserId();
    if (!userId) return '50% 50%';
    return localStorage.getItem(`profile_image_position_${userId}`) ?? '50% 50%';
  });

  setProfilePicPosition(position: string): void {
    const userId = this.currentUserId();
    if (userId) {
      const currentPic = this.profilePic();
      this.uploadProfilePicToServer(currentPic, position);
    }
  }

  login(request: LoginRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(
      `${this.apiUrl}/login`, request
    ).pipe(
      tap(res => {
        if (res.success && res.data) {
          // Extract token - backend returns { token: "...", mfaSessionToken: null, requiresSetup: false }
          let token: string;
          if (typeof res.data === 'string') {
            token = res.data;
          } else if (res.data.token) {
            token = res.data.token;
          } else {
            token = JSON.stringify(res.data);
          }

          this.storage.setToken(token);
          this._token.set(token);
        }
      })
    );
  }

  logout(): void {
    this.storage.clearToken();
    this._token.set(null);
    this.router.navigate(['/login']);
  }

  // Expose the token signal for external updates
  getTokenSignal() { return this._token; }

  getToken(): string | null {
    return this._token();
  }
  isAdmin = () => this.currentRole() === UserRole.Admin;
  isAgent = () => this.currentRole() === UserRole.Agent;
  isUser = () => this.currentRole() === UserRole.User;

  redirectByRole(): void {
    const role = this.currentRole();
    if (role === UserRole.Admin) this.router.navigate(['/dashboard']);
    else if (role === UserRole.Agent) this.router.navigate(['/agent/tickets']);
    else this.router.navigate(['/tickets']);
  }
}