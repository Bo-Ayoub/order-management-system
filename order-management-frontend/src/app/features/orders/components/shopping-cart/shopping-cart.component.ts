import { Component, EventEmitter, Input, Output, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

import { CartService, Cart, CartItem } from '../../services/cart.service';
import { Product } from '../../../../shared/models/product.model';

@Component({
  selector: 'app-shopping-cart',
  standalone: false,
  templateUrl: './shopping-cart.component.html',
  styleUrls: ['./shopping-cart.component.scss']
})
export class ShoppingCartComponent implements OnInit, OnDestroy {
  @Input() showHeader = true;
  @Input() showCheckoutButton = true;
  @Input() compact = false;
  
  @Output() checkout = new EventEmitter<void>();
  @Output() itemUpdated = new EventEmitter<CartItem>();
  @Output() itemRemoved = new EventEmitter<string>();

  cart: Cart = { items: [], totalItems: 0, totalAmount: 0, currency: 'USD' };
  loading = false;
  private destroy$ = new Subject<void>();

  constructor(private cartService: CartService) {}

  ngOnInit(): void {
    this.cartService.getCart()
      .pipe(takeUntil(this.destroy$))
      .subscribe(cart => {
        this.cart = cart;
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  updateQuantity(productId: string, quantity: number): void {
    try {
      this.cartService.updateQuantity(productId, quantity);
      this.itemUpdated.emit(this.cart.items.find(item => item.product.id === productId));
    } catch (error) {
      console.error('Failed to update quantity:', error);
      // You might want to show a notification here
    }
  }

  removeItem(productId: string): void {
    this.cartService.removeFromCart(productId);
    this.itemRemoved.emit(productId);
  }

  clearCart(): void {
    this.cartService.clearCart();
  }

  onCheckout(): void {
    this.checkout.emit();
  }

  formatPrice(price: number, currency: string): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currency
    }).format(price);
  }

  getStockStatus(product: Product): { status: string; message: string; class: string } {
    if (product.stockQuantity === 0) {
      return {
        status: 'out-of-stock',
        message: 'Out of stock',
        class: 'text-red-600 bg-red-100'
      };
    } else if (product.stockQuantity < 10) {
      return {
        status: 'low-stock',
        message: `Only ${product.stockQuantity} left`,
        class: 'text-yellow-600 bg-yellow-100'
      };
    } else {
      return {
        status: 'in-stock',
        message: 'In stock',
        class: 'text-green-600 bg-green-100'
      };
    }
  }

  isProductAvailable(product: Product): boolean {
    return product.isActive && product.stockQuantity > 0;
  }

  getMaxQuantity(product: Product): number {
    return Math.min(product.stockQuantity, 99);
  }
}
