import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { Product } from '../../../../shared/models/product.model';

@Component({
  selector: 'app-product-card',
  standalone: false,
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss']
})
export class ProductCardComponent {
  @Input() product: Product | null = null;
  @Input() showActions = true;
  
  @Output() editProduct = new EventEmitter<string>();
  @Output() deleteProduct = new EventEmitter<string>();
  @Output() viewProduct = new EventEmitter<string>();

  formatPrice(price: number, currency: string): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currency
    }).format(price);
  }

  getStockStatusClass(stockQuantity: number): string {
    if (stockQuantity === 0) return 'text-red-600 bg-red-100';
    if (stockQuantity <= 10) return 'text-yellow-600 bg-yellow-100';
    return 'text-green-600 bg-green-100';
  }

  getStockStatusText(stockQuantity: number): string {
    if (stockQuantity === 0) return 'Out of Stock';
    if (stockQuantity <= 10) return 'Low Stock';
    return 'In Stock';
  }

  getStatusClass(isActive: boolean): string {
    return isActive ? 'text-green-600 bg-green-100' : 'text-red-600 bg-red-100';
  }

  getStatusText(isActive: boolean): string {
    return isActive ? 'Active' : 'Inactive';
  }

  onEdit(): void {
    if (this.product) {
      this.editProduct.emit(this.product.id);
    }
  }

  onDelete(): void {
    if (this.product) {
      this.deleteProduct.emit(this.product.id);
    }
  }

  onView(): void {
    if (this.product) {
      this.viewProduct.emit(this.product.id);
    }
  }
}
