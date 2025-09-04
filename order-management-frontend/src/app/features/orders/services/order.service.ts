import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../../core/services/api.service';
import {
  Order,
  CreateOrderRequest,
  OrderSummary,
  OrderStatus,
} from '../../../shared/models/order.model';
import { PaginatedResponse } from '../../../shared/models/api-response.model';

export interface OrderFilters {
  pageNumber?: number;
  pageSize?: number;
  customerId?: string;
  status?: OrderStatus;
  fromDate?: string;
  toDate?: string;
}

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  constructor(@Inject(ApiService) private apiService: ApiService) {}

  getOrders(
    filters: OrderFilters = {}
  ): Observable<PaginatedResponse<OrderSummary>> {
    const params: any = {
      pageNumber: filters.pageNumber || 1,
      pageSize: filters.pageSize || 10,
    };

    // Add optional filters
    if (filters.customerId) params.customerId = filters.customerId;
    if (filters.status) params.status = filters.status;
    if (filters.fromDate) params.fromDate = filters.fromDate;
    if (filters.toDate) params.toDate = filters.toDate;

    return this.apiService.get<PaginatedResponse<OrderSummary>>(
      '/orders',
      params
    );
  }

  getOrder(id: string): Observable<Order> {
    return this.apiService.get<Order>(`/orders/${id}`);
  }

  createOrder(order: CreateOrderRequest): Observable<{ id: string }> {
    return this.apiService.post<{ id: string }>('/orders', order);
  }

  confirmOrder(id: string): Observable<void> {
    return this.apiService.post<void>(`/orders/${id}/confirm`, {});
  }

  updateOrderStatus(id: string, status: OrderStatus): Observable<Order> {
    return this.apiService.put<Order>(`/orders/${id}/status`, {
      orderId: id,
      newStatus: status,
    });
  }

  cancelOrder(id: string): Observable<Order> {
    return this.updateOrderStatus(id, OrderStatus.Cancelled);
  }

  // Get orders by customer
  getCustomerOrders(
    customerId: string,
    pageSize: number = 10
  ): Observable<OrderSummary[]> {
    return this.getOrders({
      customerId,
      pageSize,
      pageNumber: 1,
    }).pipe(map((response) => response.data));
  }

  // Get orders by status
  getOrdersByStatus(
    status: OrderStatus,
    pageSize: number = 20
  ): Observable<OrderSummary[]> {
    return this.getOrders({
      status,
      pageSize,
      pageNumber: 1,
    }).pipe(map((response) => response.data));
  }

  // Get pending orders
  getPendingOrders(): Observable<OrderSummary[]> {
    return this.getOrdersByStatus(OrderStatus.Pending);
  }

  // Get recent orders (last 7 days)
  getRecentOrders(days: number = 7): Observable<OrderSummary[]> {
    const fromDate = new Date();
    fromDate.setDate(fromDate.getDate() - days);

    return this.getOrders({
      fromDate: fromDate.toISOString().split('T')[0],
      pageSize: 50,
    }).pipe(map((response) => response.data));
  }

  // Get order statistics
  getOrderStats(): Observable<{
    totalOrders: number;
    pendingOrders: number;
    completedOrders: number;
    totalRevenue: number;
  }> {
    // This would typically be a dedicated endpoint
    return this.getOrders({ pageSize: 1000 }).pipe(
      map((response) => ({
        totalOrders: response.pagination.totalCount,
        pendingOrders: response.data.filter(
          (o) => o.status === OrderStatus.Pending
        ).length,
        completedOrders: response.data.filter(
          (o) => o.status === OrderStatus.Delivered
        ).length,
        totalRevenue: response.data.reduce(
          (sum, order) => sum + order.totalAmount,
          0
        ),
      }))
    );
  }

  // Validate order before creation
  validateOrder(order: CreateOrderRequest): string[] {
    const errors: string[] = [];

    if (!order.customerId) {
      errors.push('Customer is required');
    }

    if (!order.items || order.items.length === 0) {
      errors.push('Order must have at least one item');
    }

    if (order.items) {
      order.items.forEach((item, index) => {
        if (!item.productId) {
          errors.push(`Item ${index + 1}: Product is required`);
        }
        if (!item.quantity || item.quantity <= 0) {
          errors.push(`Item ${index + 1}: Quantity must be greater than 0`);
        }
      });
    }

    return errors;
  }
}
