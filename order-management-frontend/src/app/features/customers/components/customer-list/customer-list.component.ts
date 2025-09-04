import {
  Component,
  EventEmitter,
  Input,
  Output,
  OnInit,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';

import {
  Customer,
  CustomerSummary,
} from '../../../../shared/models/customer.model';
import { PaginatedResponse } from '../../../../shared/models/api-response.model';
import {
  CustomerService,
  CustomerFilters,
} from '../../services/customer.service';

@Component({
  selector: 'app-customer-list',
  standalone: false,
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss'],
})
export class CustomerListComponent implements OnInit, OnDestroy {
  @Input() customers: Customer[] = [];
  @Input() pagination: PaginatedResponse<Customer>['pagination'] | null = null;
  @Input() loading = false;
  @Input() searchTerm = '';

  @Output() searchChange = new EventEmitter<string>();
  @Output() pageChange = new EventEmitter<number>();
  @Output() deleteCustomer = new EventEmitter<string>();
  @Output() refresh = new EventEmitter<void>();

  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  constructor(private customerService: CustomerService) {}

  ngOnInit(): void {
    // Setup search debouncing
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe((searchTerm) => {
        this.searchChange.emit(searchTerm);
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSearchChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchTerm = target.value;
    this.searchSubject.next(this.searchTerm);
  }

  onPageChange(page: number): void {
    this.pageChange.emit(page);
  }

  onDeleteCustomer(customerId: string): void {
    if (confirm('Are you sure you want to delete this customer?')) {
      this.deleteCustomer.emit(customerId);
    }
  }

  onRefresh(): void {
    this.refresh.emit();
  }

  getFullName(customer: Customer): string {
    return `${customer.firstName} ${customer.lastName}`;
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  getPageNumbers(): number[] {
    if (!this.pagination) return [];

    const currentPage = this.pagination.pageNumber;
    const totalPages = this.pagination.totalPages;
    const pages: number[] = [];

    // Show up to 5 page numbers
    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, startPage + 4);

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    return pages;
  }

  // Expose Math to template
  Math = Math;
}
