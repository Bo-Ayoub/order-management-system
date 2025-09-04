import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import { OrderSummary, OrderStatus } from '../../../../shared/models/order.model';
import { PaginatedResponse } from '../../../../shared/models/api-response.model';
import { OrderService, OrderFilters } from '../../services/order.service';

@Component({
  selector: 'app-order-list-page',
  standalone: false,
  templateUrl: './order-list-page.component.html',
  styleUrls: ['./order-list-page.component.scss']
})
export class OrderListPageComponent implements OnInit, OnDestroy {
  orders: OrderSummary[] = [];
  pagination: PaginatedResponse<OrderSummary>['pagination'] | null = null;
  loading = false;
  searchTerm = '';
  selectedStatus = '';
  selectedCustomer = '';
  
  private destroy$ = new Subject<void>();

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadOrders(): void {
    this.loading = true;
    
    const filters: OrderFilters = {
      pageNumber: this.pagination?.pageNumber || 1,
      pageSize: 10,
      status: this.selectedStatus ? this.selectedStatus as OrderStatus : undefined,
      fromDate: undefined,
      toDate: undefined
    };

    this.orderService.getOrders(filters)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.loading = false)
      )
      .subscribe({
        next: (response) => {
          this.orders = response.data;
          this.pagination = response.pagination;
        },
        error: (error) => {
          console.error('Failed to load orders:', error);
          // You might want to show a notification here
        }
      });
  }

  onSearchChange(searchTerm: string): void {
    this.searchTerm = searchTerm;
    this.pagination = { ...this.pagination!, pageNumber: 1 };
    this.loadOrders();
  }

  onStatusChange(status: string): void {
    this.selectedStatus = status;
    this.pagination = { ...this.pagination!, pageNumber: 1 };
    this.loadOrders();
  }

  onCustomerChange(customer: string): void {
    this.selectedCustomer = customer;
    this.pagination = { ...this.pagination!, pageNumber: 1 };
    this.loadOrders();
  }

  onPageChange(page: number): void {
    this.pagination = { ...this.pagination!, pageNumber: page };
    this.loadOrders();
  }

  onRefresh(): void {
    this.loadOrders();
  }
}
