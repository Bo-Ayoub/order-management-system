import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { LoadingService } from '../../shared/services/loading.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  private activeRequests = 0;

  constructor(private loadingService: LoadingService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Skip loading indicator for certain requests
    if (this.shouldSkipLoading(request)) {
      return next.handle(request);
    }

    // Increment active requests counter
    this.activeRequests++;
    
    // Show loading indicator if this is the first active request
    if (this.activeRequests === 1) {
      this.loadingService.setLoading(true);
    }

    return next.handle(request).pipe(
      finalize(() => {
        // Decrement active requests counter
        this.activeRequests--;
        
        // Hide loading indicator if no more active requests
        if (this.activeRequests === 0) {
          this.loadingService.setLoading(false);
        }
      })
    );
  }

  private shouldSkipLoading(request: HttpRequest<any>): boolean {
    // Skip loading for certain endpoints or request types
    const skipLoadingPatterns = [
      '/api/health',
      '/api/ping',
      '/api/status'
    ];

    return skipLoadingPatterns.some(pattern => 
      request.url.includes(pattern)
    );
  }
}