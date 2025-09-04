import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

import { CartService, Cart } from '../../services/cart.service';

@Component({
  selector: 'app-shopping-cart-page',
  standalone: false,
  templateUrl: './shopping-cart-page.component.html',
  styleUrls: ['./shopping-cart-page.component.scss']
})
export class ShoppingCartPageComponent implements OnInit, OnDestroy {
  cart: Cart = { items: [], totalItems: 0, totalAmount: 0, currency: 'USD' };
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

  onCheckout(): void {
    // Navigate to order creation page
    // This would typically be handled by the router
    console.log('Proceeding to checkout...');
  }

  onItemUpdated(): void {
    // Item was updated, cart will automatically refresh
  }

  onItemRemoved(): void {
    // Item was removed, cart will automatically refresh
  }
}
