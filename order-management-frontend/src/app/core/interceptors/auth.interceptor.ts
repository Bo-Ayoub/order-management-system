import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor() {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    // Skip auth for certain endpoints
    const skipAuth = request.headers.has('X-Skip-Auth');

    if (skipAuth) {
      return next.handle(request);
    }

    // For now, we don't have authentication
    // When you add authentication later, you would:
    // 1. Get the auth token from storage/service
    // 2. Clone the request and add Authorization header
    // 3. Handle token refresh if needed

    /*
    Future implementation:
    
    const authToken = this.authService.getToken();
    
    if (authToken) {
      const authRequest = request.clone({
        setHeaders: {
          Authorization: `Bearer ${authToken}`
        }
      });
      return next.handle(authRequest);
    }
    */

    return next.handle(request);
  }
}
