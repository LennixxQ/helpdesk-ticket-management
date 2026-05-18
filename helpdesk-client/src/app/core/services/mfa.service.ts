import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

export interface MfaSetupResponse {
    secretKey: string;
    qrCodeDataUri: string;
}

export interface MfaVerifyRequest {
    jwtToken?: string;
    code: string;
}

export interface MfaStatusResponse {
    isMfaEnabled: boolean;
}

export interface LoginResponse {
    token?: string;
    mfaSessionToken?: string;
    requiresSetup?: boolean;
}

@Injectable({ providedIn: 'root' })
export class MfaService {
    private http = inject(HttpClient);
    private api = `${environment.apiUrl}/Mfa`;

    getSetup(): Observable<ApiResponse<MfaSetupResponse>> {
        return this.http.get<ApiResponse<MfaSetupResponse>>(`${this.api}/setup`);
    }

    verifySetup(code: string, jwtToken?: string): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/verify-setup`, { code, jwtToken });
    }

    verifyLogin(code: string, jwtToken: string): Observable<ApiResponse<LoginResponse>> {
        return this.http.post<ApiResponse<LoginResponse>>(`${this.api}/verify-login`, { code, jwtToken });
    }

    disable(code: string): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(`${this.api}/disable`, { code });
    }
}
