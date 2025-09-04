import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import { Customer } from '../../../../shared/models/customer.model';
import { PaginatedResponse } from '../../../../shared/models/api-response.model';
import {
  CustomerService,
  CustomerFilters,
} from '../../services/customer.service';
import { CustomerListComponent } from '../../components/customer-list/customer-list.component';

@Component({
  selector: 'app-customer-list-page',
  standalone: false,
  templateUrl: './customer-list-page.component.html',
  styleUrls: ['./customer-list-page.component.scss'],
})
export class CustomerListPageComponent implements OnInit, OnDestroy {
  customers: Customer[] = [];
  pagination: PaginatedResponse<Customer>['pagination'] | null = null;
  loading = false;
  searchTerm = '';

  private destroy$ = new Subject<void>();
  private currentFilters: CustomerFilters = {
    pageNumber: 1,
    pageSize: 10,
    searchTerm: '',
  };

  constructor(
    private customerService: CustomerService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCustomers();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCustomers(): void {
    this.loading = true;
    this.customerService
      .getCustomers(this.currentFilters)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => (this.loading = false))
      )
      .subscribe({
        next: (response) => {
          this.customers = response.data;
          this.pagination = response.pagination;
        },
        error: (error) => {
          console.error('Error loading customers:', error);
          // Handle error - could show notification
        },
      });
  }

  onSearchChange(searchTerm: string): void {
    this.searchTerm = searchTerm;
    this.currentFilters = {
      ...this.currentFilters,
      searchTerm,
      pageNumber: 1, // Reset to first page when searching
    };
    this.loadCustomers();
  }

  onPageChange(page: number): void {
    this.currentFilters = {
      ...this.currentFilters,
      pageNumber: page,
    };
    this.loadCustomers();
  }

  onDeleteCustomer(customerId: string): void {
    this.customerService
      .deleteCustomer(customerId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          // Remove customer from local array
          this.customers = this.customers.filter((c) => c.id !== customerId);
          // Show success notification
          console.log('Customer deleted successfully');
        },
        error: (error) => {
          console.error('Error deleting customer:', error);
          // Handle error - could show notification
        },
      });
  }

  onRefresh(): void {
    this.loadCustomers();
  }

  onCreateCustomer(): void {
    this.router.navigate(['/customers/create']);
  }
}
