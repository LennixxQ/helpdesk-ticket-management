import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const token = authService.getToken();
  console.log('AuthGuard: Token exists:', !!token, 'length:', token?.length);

  if (token && typeof token === 'string' && token.length > 0) {
    console.log('AuthGuard: Allowing access');
    return true;
  }

  console.log('AuthGuard: No valid token, redirecting to login');
  router.navigate(['/login']);
  return false;
};
