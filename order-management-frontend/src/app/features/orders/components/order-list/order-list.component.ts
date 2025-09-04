import { Component, EventEmitter, Input, Output, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';

import { OrderSummary, OrderStatus } from '../../../../shared/models/order.model';
import { PaginatedResponse } from '../../../../shared/models/api-response.model';
import { OrderService, OrderFilters } from '../../services/order.service';

@Component({
  selector: 'app-order-list',
  standalone: false,
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.scss']
})
export class OrderListComponent implements OnInit, OnDestroy {
  @Input() orders: OrderSummary[] = [];
  @Input() pagination: PaginatedResponse<OrderSummary>['pagination'] | null = null;
  @Input() loading = false;
  @Input() searchTerm = '';
  @Input() selectedStatus = '';
  @Input() selectedCustomer = '';
  
  @Output() searchChange = new EventEmitter<string>();
  @Output() statusChange = new EventEmitter<string>();
  @Output() customerChange = new EventEmitter<string>();
  @Output() pageChange = new EventEmitter<number>();
  @Output() refresh = new EventEmitter<void>();

  orderStatuses = Object.values(OrderStatus);
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    // Setup search debouncing
    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe(searchTerm => {
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

  onStatusChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    this.selectedStatus = target.value;
    this.statusChange.emit(this.selectedStatus);
  }

  onCustomerChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.selectedCustomer = target.value;
    this.customerChange.emit(this.selectedCustomer);
  }

  onPageChange(page: number): void {
    this.pageChange.emit(page);
  }

  onRefresh(): void {
    this.refresh.emit();
  }

  formatPrice(price: number, currency: string): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currency
    }).format(price);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  getStatusClass(status: OrderStatus): string {
    const statusClasses: { [key in OrderStatus]: string } = {
      [OrderStatus.Pending]: 'text-yellow-600 bg-yellow-100',
      [OrderStatus.Confirmed]: 'text-blue-600 bg-blue-100',
      [OrderStatus.Processing]: 'text-purple-600 bg-purple-100',
      [OrderStatus.Shipped]: 'text-indigo-600 bg-indigo-100',
      [OrderStatus.Delivered]: 'text-green-600 bg-green-100',
      [OrderStatus.Cancelled]: 'text-red-600 bg-red-100'
    };
    return statusClasses[status] || 'text-gray-600 bg-gray-100';
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
