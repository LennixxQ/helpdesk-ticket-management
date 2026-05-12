import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { of, tap } from 'rxjs';

const cache = new Map<string, { response: HttpResponse<any>, expiry: number }>();
const CACHE_DURATION = 5 * 60 * 1000; // 5 minutes

export const cacheInterceptor: HttpInterceptorFn = (req, next) => {
  // Only cache GET requests
  if (req.method !== 'GET') {
    return next(req);
  }

  // Define which endpoints to cache (e.g., categories, departments, settings)
  const cacheableUrls = ['/api/category', '/api/department', '/api/systemsetting', '/api/kbarticle'];
  const isCacheable = cacheableUrls.some(url => req.url.toLowerCase().includes(url));

  if (!isCacheable) {
    return next(req);
  }

  const cached = cache.get(req.urlWithParams);
  if (cached && cached.expiry > Date.now()) {
    return of(cached.response);
  }

  return next(req).pipe(
    tap(event => {
      if (event instanceof HttpResponse) {
        cache.set(req.urlWithParams, {
          response: event,
          expiry: Date.now() + CACHE_DURATION
        });
      }
    })
  );
};
