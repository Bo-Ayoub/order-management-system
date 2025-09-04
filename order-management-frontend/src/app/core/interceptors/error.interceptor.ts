import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { NotificationService } from '../../shared/services/notification.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private notificationService: NotificationService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An unexpected error occurred';
        
        if (error.error instanceof ErrorEvent) {
          // Client-side error
          errorMessage = `Error: ${error.error.message}`;
        } else {
          // Server-side error
          switch (error.status) {
            case 400:
              errorMessage = error.error?.message || 'Bad Request';
              break;
            case 401:
              errorMessage = 'Unauthorized. Please log in again.';
              // You might want to redirect to login page here
              break;
            case 403:
              errorMessage = 'Access denied. You do not have permission to perform this action.';
              break;
            case 404:
              errorMessage = error.error?.message || 'The requested resource was not found.';
              break;
            case 409:
              errorMessage = error.error?.message || 'Conflict. The resource already exists or has been modified.';
              break;
            case 422:
              errorMessage = error.error?.message || 'Validation error. Please check your input.';
              break;
            case 429:
              errorMessage = 'Too many requests. Please try again later.';
              break;
            case 500:
              errorMessage = 'Internal server error. Please try again later.';
              break;
            case 502:
              errorMessage = 'Bad gateway. The server is temporarily unavailable.';
              break;
            case 503:
              errorMessage = 'Service unavailable. Please try again later.';
              break;
            case 504:
              errorMessage = 'Gateway timeout. The request took too long to complete.';
              break;
            default:
              errorMessage = error.error?.message || `Error ${error.status}: ${error.statusText}`;
          }
        }

        // Show error notification
        this.notificationService.showError(errorMessage);

        // Log error for debugging
        console.error('HTTP Error:', error);

        return throwError(() => error);
      })
    );
  }
}