import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { NotificationService } from '../services/notification.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private notificationService: NotificationService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      // Retry failed requests once (except for POST/PUT/DELETE)
      retry({
        count: this.shouldRetry(request) ? 1 : 0,
        delay: 1000,
      }),
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An unexpected error occurred';
        let errorTitle = 'Error';

        // Handle different error scenarios
        switch (error.status) {
          case 0:
            errorTitle = 'Network Error';
            errorMessage =
              'Unable to connect to the server. Please check your internet connection.';
            break;
          case 400:
            errorTitle = 'Bad Request';
            errorMessage =
              this.extractErrorMessage(error) || 'Invalid request data';
            break;
          case 401:
            errorTitle = 'Unauthorized';
            errorMessage = 'You are not authorized to perform this action';
            break;
          case 403:
            errorTitle = 'Forbidden';
            errorMessage = 'You do not have permission to access this resource';
            break;
          case 404:
            errorTitle = 'Not Found';
            errorMessage = 'The requested resource was not found';
            break;
          case 422:
            errorTitle = 'Validation Error';
            errorMessage =
              this.extractErrorMessage(error) || 'Validation failed';
            break;
          case 500:
            errorTitle = 'Server Error';
            errorMessage = 'An internal server error occurred';
            break;
          case 503:
            errorTitle = 'Service Unavailable';
            errorMessage = 'The service is temporarily unavailable';
            break;
          default:
            errorMessage =
              this.extractErrorMessage(error) ||
              `HTTP ${error.status}: ${error.statusText}`;
        }

        // Show notification for non-silent requests
        if (!request.headers.has('X-Skip-Error-Notification')) {
          this.notificationService.error(errorTitle, errorMessage);
        }

        // Return the error for handling by the calling service
        return throwError(() => ({
          status: error.status,
          message: errorMessage,
          originalError: error,
        }));
      })
    );
  }

  private shouldRetry(request: HttpRequest<any>): boolean {
    // Only retry GET requests
    return request.method === 'GET';
  }

  private extractErrorMessage(error: HttpErrorResponse): string | null {
    // Try to extract error message from different response formats
    if (error.error) {
      // Backend returns { error: "message" }
      if (typeof error.error === 'string') {
        return error.error;
      }

      // Backend returns { error: "message" } as object
      if (error.error.error) {
        return error.error.error;
      }

      // Backend returns { message: "message" }
      if (error.error.message) {
        return error.error.message;
      }

      // Backend returns validation errors array
      if (error.error.errors && Array.isArray(error.error.errors)) {
        return error.error.errors.join(', ');
      }
    }

    return null;
  }
}
