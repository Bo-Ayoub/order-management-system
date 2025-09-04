import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private readonly baseUrl = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
    });
  }

  get<T>(endpoint: string, params?: any): Observable<T> {
    let httpParams = new HttpParams();

    if (params) {
      Object.keys(params).forEach((key) => {
        if (params[key] !== null && params[key] !== undefined) {
          httpParams = httpParams.append(key, params[key].toString());
        }
      });
    }

    return this.http
      .get<T>(`${this.baseUrl}${endpoint}`, {
        headers: this.getHeaders(),
        params: httpParams,
        observe: 'response',
      })
      .pipe(
        map((response) => {
          // Extract pagination headers if they exist
          const totalCount = response.headers.get('X-Pagination-TotalCount');
          const pageNumber = response.headers.get('X-Pagination-PageNumber');
          const totalPages = response.headers.get('X-Pagination-TotalPages');
          const hasNext = response.headers.get('X-Pagination-HasNext');
          const hasPrevious = response.headers.get('X-Pagination-HasPrevious');

          if (totalCount && pageNumber && totalPages) {
            return {
              data: response.body,
              pagination: {
                pageNumber: parseInt(pageNumber),
                totalPages: parseInt(totalPages),
                totalCount: parseInt(totalCount),
                hasNextPage: hasNext === 'true',
                hasPreviousPage: hasPrevious === 'true',
              },
            } as any;
          }

          return response.body as T;
        }),
        catchError(this.handleError)
      );
  }

  post<T>(endpoint: string, data: any): Observable<T> {
    return this.http
      .post<T>(`${this.baseUrl}${endpoint}`, data, {
        headers: this.getHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  put<T>(endpoint: string, data: any): Observable<T> {
    return this.http
      .put<T>(`${this.baseUrl}${endpoint}`, data, {
        headers: this.getHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  delete<T>(endpoint: string): Observable<T> {
    return this.http
      .delete<T>(`${this.baseUrl}${endpoint}`, {
        headers: this.getHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    console.error('API Error:', error);

    if (error.error && error.error.error) {
      return throwError(() => new Error(error.error.error));
    }

    if (error.message) {
      return throwError(() => new Error(error.message));
    }

    return throwError(() => new Error('An unexpected error occurred'));
  }
}
