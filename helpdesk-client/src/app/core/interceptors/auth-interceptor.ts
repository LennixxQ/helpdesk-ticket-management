import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // Skip auth header for login requests
  if (req.url.includes('/auth/login')) {
    return next(req);
  }

  const authService = inject(AuthService);
  const token = authService.getToken();

  // Only add auth header if token is a valid non-empty string
  if (token && typeof token === 'string' && token.length > 0) {
    return next(req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    }));
  }

  return next(req);
};