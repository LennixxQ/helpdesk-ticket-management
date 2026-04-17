import { TestBed } from '@angular/core/testing';
import { Router, CanActivateFn } from '@angular/router';
import { authGuard } from './auth-guard';
import { AuthService } from '../services/auth.service';

describe('authGuard', () => {
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

  const executeGuard: CanActivateFn = (...guardParameters) =>
    TestBed.runInInjectionContext(() => authGuard(...guardParameters));

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['isLoggedIn']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    });
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });

  it('should allow activation if logged in', () => {
    authServiceSpy.isLoggedIn.and.returnValue(true);
    const result = executeGuard({} as any, {} as any);
    expect(result).toBeTrue();
  });

  it('should redirect to login if not logged in', () => {
    authServiceSpy.isLoggedIn.and.returnValue(false);
    const result = executeGuard({} as any, {} as any);
    expect(result).toBeFalse();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });
});
