import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap, finalize } from 'rxjs/operators';
import { LoadingService } from '../services/loading.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  constructor(private loadingService: LoadingService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    // Show loading for non-silent requests
    if (!request.headers.has('X-Skip-Loading')) {
      this.loadingService.show();
    }

    return next.handle(request).pipe(
      tap({
        next: (event: HttpEvent<any>) => {
          // Handle successful responses if needed
          if (event instanceof HttpResponse) {
            // Optional: Add success handling logic here
          }
        },
        error: (error: HttpErrorResponse) => {
          // Handle errors if needed
          console.error('HTTP Error:', error);
        },
      }),
      finalize(() => {
        // Always hide loading when request completes
        if (!request.headers.has('X-Skip-Loading')) {
          this.loadingService.hide();
        }
      })
    );
  }
}
