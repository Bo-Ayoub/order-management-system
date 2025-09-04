import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import {
  Order,
  CreateOrderRequest,
} from '../../../../shared/models/order.model';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order-edit-page',
  standalone: false,
  templateUrl: './order-edit-page.component.html',
  styleUrls: ['./order-edit-page.component.scss'],
})
export class OrderEditPageComponent implements OnInit, OnDestroy {
  order: Order | null = null;
  loading = false;
  pageLoading = false;
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

    this.pageLoading = true;
    this.orderService
      .getOrder(orderId)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => (this.pageLoading = false))
      )
      .subscribe({
        next: (order) => {
          this.order = order;
        },
        error: (error) => {
          console.error('Failed to load order:', error);
        },
      });
  }

  onSave(orderRequest: CreateOrderRequest): void {
    this.loading = true;

    // Note: This would typically be an updateOrder method
    // For now, we'll use createOrder as a placeholder
    this.orderService
      .createOrder(orderRequest)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => (this.loading = false))
      )
      .subscribe({
        next: (response) => {
          console.log('Order updated:', response);
          // Navigate to order detail page
        },
        error: (error) => {
          console.error('Failed to update order:', error);
        },
      });
  }

  onCancel(): void {
    // Navigate back to order detail
  }

  getOrderForForm(): CreateOrderRequest | null {
    if (!this.order) return null;

    return {
      customerId: this.order.customerId,
      items: this.order.orderItems.map((item) => ({
        productId: item.productId,
        quantity: item.quantity,
      })),
      shippingAddress: this.order.shippingAddress,
      notes: this.order.notes,
    };
  }
}
