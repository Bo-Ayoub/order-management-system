import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../../core/services/api.service';
import {
  Customer,
  CreateCustomerRequest,
  CustomerSummary,
} from '../../../shared/models/customer.model';
import { PaginatedResponse } from '../../../shared/models/api-response.model';

export interface CustomerFilters {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
}

@Injectable({
  providedIn: 'root',
})
export class CustomerService {
  constructor(@Inject(ApiService) private apiService: ApiService) {}

  getCustomers(
    filters: CustomerFilters = {}
  ): Observable<PaginatedResponse<Customer>> {
    const params = {
      pageNumber: filters.pageNumber || 1,
      pageSize: filters.pageSize || 10,
      ...(filters.searchTerm && { searchTerm: filters.searchTerm }),
    };

    return this.apiService.get<PaginatedResponse<Customer>>(
      '/customers',
      params
    );
  }

  getCustomer(id: string): Observable<Customer> {
    return this.apiService.get<Customer>(`/customers/${id}`);
  }

  createCustomer(customer: CreateCustomerRequest): Observable<{ id: string }> {
    return this.apiService.post<{ id: string }>('/customers', customer);
  }

  updateCustomer(
    id: string,
    customer: Partial<CreateCustomerRequest>
  ): Observable<void> {
    return this.apiService.put<void>(`/customers/${id}`, customer);
  }

  deleteCustomer(id: string): Observable<void> {
    return this.apiService.delete<void>(`/customers/${id}`);
  }

  // Search customers by name or email
  searchCustomers(
    searchTerm: string,
    pageSize: number = 10
  ): Observable<Customer[]> {
    return this.getCustomers({
      searchTerm,
      pageSize,
      pageNumber: 1,
    }).pipe(map((response) => response.data));
  }

  // Get customer summary for dropdowns/selectors
  getCustomerSummaries(): Observable<CustomerSummary[]> {
    return this.getCustomers({ pageSize: 100 }).pipe(
      map((response) =>
        response.data.map((customer) => ({
          id: customer.id,
          fullName: `${customer.firstName} ${customer.lastName}`,
          email: customer.email,
          totalOrders: 0, // This would come from backend if available
        }))
      )
    );
  }

  // Validate if customer exists
  customerExists(id: string): Observable<boolean> {
    return this.getCustomer(id).pipe(
      map(() => true)
      // If error occurs (customer not found), return false
    );
  }
}
