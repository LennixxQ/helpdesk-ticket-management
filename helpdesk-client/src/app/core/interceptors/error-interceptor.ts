import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      console.error('HTTP Error:', err.status, err.url, err.error);
      if (err.status === 401) {
        localStorage.clear();
        router.navigate(['/login']);
      }
      if (err.status === 403) router.navigate(['/tickets']);
      return throwError(() => err);
    })
  );
};