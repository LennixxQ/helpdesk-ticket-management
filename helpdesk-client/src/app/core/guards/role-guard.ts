import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models/user.model';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const auth = inject(AuthService);
  const roles = route.data['roles'] as UserRole[];
  if (roles.includes(auth.currentRole()!)) return true;
  inject(Router).navigate(['/tickets']);
  return false;
};