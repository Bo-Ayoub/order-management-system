import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import { CreateOrderRequest } from '../../../../shared/models/order.model';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order-create-page',
  standalone: false,
  templateUrl: './order-create-page.component.html',
  styleUrls: ['./order-create-page.component.scss']
})
export class OrderCreatePageComponent implements OnInit, OnDestroy {
  loading = false;
  private destroy$ = new Subject<void>();

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {}

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSave(orderRequest: CreateOrderRequest): void {
    this.loading = true;
    
    this.orderService.createOrder(orderRequest)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.loading = false)
      )
      .subscribe({
        next: (response) => {
          console.log('Order created:', response);
          // Navigate to order detail page
          // this.router.navigate(['/orders', response.id]);
        },
        error: (error) => {
          console.error('Failed to create order:', error);
          // You might want to show a notification here
        }
      });
  }

  onCancel(): void {
    // Navigate back to orders list
    // this.router.navigate(['/orders']);
  }
}
