import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models/user.model';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const currentRole = auth.currentRole();
  const allowedRoles = route.data['roles'] as string[];

  console.log('RoleGuard: currentRole:', currentRole, 'allowedRoles:', allowedRoles);

  if (!currentRole) {
    console.log('RoleGuard: No role, redirecting to login');
    router.navigate(['/login']);
    return false;
  }

  // Convert string roles to enum values for comparison
  const roleMap: Record<string, UserRole> = {
    'Admin': UserRole.Admin,
    'Agent': UserRole.Agent,
    'User': UserRole.User
  };

  const allowed = allowedRoles.map(r => roleMap[r]).filter(r => r !== undefined);
  console.log('RoleGuard: allowed (as enum):', allowed, 'includes current:', allowed.includes(currentRole));

  if (allowed.includes(currentRole)) return true;

  console.log('RoleGuard: Role not allowed, redirecting to /tickets');
  router.navigate(['/tickets']);
  return false;
};