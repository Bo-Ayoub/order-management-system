import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { Order, OrderStatus } from '../../../../shared/models/order.model';
import { OrderService } from '../../services/order.service';
import { NotificationService } from '../../../../shared/services/notification.service';

@Component({
  selector: 'app-order-detail',
  standalone: false,
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss'],
})
export class OrderDetailComponent implements OnInit {
  @Input() order: Order | null = null;
  @Input() loading = false;
  @Output() statusUpdate = new EventEmitter<{
    orderId: string;
    status: OrderStatus;
  }>();
  @Output() edit = new EventEmitter<string>();
  @Output() delete = new EventEmitter<string>();
  @Output() statusUpdated = new EventEmitter<OrderStatus>();

  orderStatuses = Object.values(OrderStatus);
  updatingStatus = false;

  constructor(
    private orderService: OrderService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {}

  // onStatusUpdate(newStatus: OrderStatus): void {
  //   if (this.order) {
  //     this.statusUpdate.emit({
  //       orderId: this.order.id,
  //       status: newStatus,
  //     });
  //   }
  // }

  onStatusUpdate(event: Event): void {
    const target = event.target as HTMLSelectElement;
    if (target?.value) {
      const newStatus = target.value as OrderStatus;
      this.updateOrderStatus(newStatus);
    }
  }

  private updateOrderStatus(status: OrderStatus): void {
    if (this.order && !this.updatingStatus) {
      this.updatingStatus = true;

      this.orderService.updateOrderStatus(this.order.id, status).subscribe({
        next: (updatedOrder) => {
          this.notificationService.showSuccess(
            `Order status updated to ${status}`
          );
          this.statusUpdated.emit(status);
          this.statusUpdate.emit({
            orderId: this.order!.id,
            status: status,
          });
          this.updatingStatus = false;
        },
        error: (error) => {
          this.notificationService.showError(
            `Failed to update order status: ${error.message || 'Unknown error'}`
          );
          this.updatingStatus = false;
        },
      });
    }
  }

  onEdit(): void {
    if (this.order) {
      this.edit.emit(this.order.id);
    }
  }

  onDelete(): void {
    if (this.order) {
      this.delete.emit(this.order.id);
    }
  }

  formatPrice(price: number, currency: string): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currency,
    }).format(price);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  getStatusClass(status: OrderStatus): string {
    const statusClasses: { [key in OrderStatus]: string } = {
      [OrderStatus.Pending]: 'text-yellow-600 bg-yellow-100',
      [OrderStatus.Confirmed]: 'text-blue-600 bg-blue-100',
      [OrderStatus.Processing]: 'text-purple-600 bg-purple-100',
      [OrderStatus.Shipped]: 'text-indigo-600 bg-indigo-100',
      [OrderStatus.Delivered]: 'text-green-600 bg-green-100',
      [OrderStatus.Cancelled]: 'text-red-600 bg-red-100',
    };
    return statusClasses[status] || 'text-gray-600 bg-gray-100';
  }

  canUpdateStatus(): boolean {
    if (!this.order) return false;
    return (
      this.order.status !== OrderStatus.Delivered &&
      this.order.status !== OrderStatus.Cancelled
    );
  }

  canEdit(): boolean {
    if (!this.order) return false;
    return this.order.status === OrderStatus.Pending;
  }

  canDelete(): boolean {
    if (!this.order) return false;
    return this.order.status === OrderStatus.Pending;
  }
}
