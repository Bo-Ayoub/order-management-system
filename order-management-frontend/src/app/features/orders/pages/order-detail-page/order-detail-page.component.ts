import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import { Order, OrderStatus } from '../../../../shared/models/order.model';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order-detail-page',
  standalone: false,
  templateUrl: './order-detail-page.component.html',
  styleUrls: ['./order-detail-page.component.scss']
})
export class OrderDetailPageComponent implements OnInit, OnDestroy {
  order: Order | null = null;
  loading = false;
  private destroy$ = new Subject<void>();

  constructor(
    private orderService: OrderService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadOrder();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadOrder(): void {
    const orderId = this.route.snapshot.paramMap.get('id');
    if (!orderId) return;

    this.loading = true;
    this.orderService.getOrder(orderId)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.loading = false)
      )
      .subscribe({
        next: (order) => {
          this.order = order;
        },
        error: (error) => {
          console.error('Failed to load order:', error);
        }
      });
  }

  onStatusUpdate(event: { orderId: string; status: OrderStatus }): void {
    this.orderService.updateOrderStatus(event.orderId, event.status)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          console.log('Order status updated');
          this.loadOrder(); // Reload order to get updated data
        },
        error: (error) => {
          console.error('Failed to update order status:', error);
        }
      });
  }

  onEdit(orderId: string): void {
    // Navigate to edit page
    console.log('Edit order:', orderId);
  }

  onDelete(orderId: string): void {
    // Handle delete
    console.log('Delete order:', orderId);
  }
}
