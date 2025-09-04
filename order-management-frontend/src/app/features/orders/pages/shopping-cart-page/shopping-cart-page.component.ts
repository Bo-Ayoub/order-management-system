import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';

import { CartService, Cart } from '../../services/cart.service';
import { CustomerSelectionComponent } from '../../components/customer-selection/customer-selection.component';
import { Customer } from '../../../../shared/models/customer.model';
import { OrderService } from '../../services/order.service';
import { NotificationService } from '../../../../shared/services/notification.service';

@Component({
  selector: 'app-shopping-cart-page',
  standalone: false,
  templateUrl: './shopping-cart-page.component.html',
  styleUrls: ['./shopping-cart-page.component.scss'],
})
export class ShoppingCartPageComponent implements OnInit, OnDestroy {
  cart: Cart = { items: [], totalItems: 0, totalAmount: 0, currency: 'USD' };
  selectedCustomer: Customer | null = null;
  currentStep: 'cart' | 'customer' | 'shipping' | 'review' | 'confirmation' =
    'cart';
  creatingOrder = false;

  // Shipping information
  shippingAddress = {
    street: '',
    city: '',
    state: '',
    zipCode: '',
    country: '',
  };

  // Order notes
  orderNotes = '';

  private destroy$ = new Subject<void>();

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.cartService
      .getCart()
      .pipe(takeUntil(this.destroy$))
      .subscribe((cart) => {
        this.cart = cart;
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onCheckout(): void {
    if (this.cart.items.length === 0) {
      this.notificationService.showWarning(
        'Your cart is empty. Add some products before checkout.'
      );
      return;
    }
    this.currentStep = 'customer';
  }

  onCustomerSelected(customer: Customer): void {
    this.selectedCustomer = customer;
    this.currentStep = 'shipping';
  }

  onCustomerCreated(customer: Customer): void {
    this.selectedCustomer = customer;
    this.currentStep = 'shipping';
  }

  onShippingComplete(): void {
    this.currentStep = 'review';
  }

  onBackToCart(): void {
    this.currentStep = 'cart';
  }

  onBackToCustomer(): void {
    this.currentStep = 'customer';
  }

  onBackToShipping(): void {
    this.currentStep = 'shipping';
  }

  onPlaceOrder(): void {
    if (!this.selectedCustomer) {
      this.notificationService.showError(
        'Please select a customer before placing the order.'
      );
      return;
    }

    if (!this.isShippingAddressValid()) {
      this.notificationService.showError(
        'Please fill in all required shipping address fields.'
      );
      return;
    }

    this.creatingOrder = true;

    const orderData = {
      customerId: this.selectedCustomer.id,
      items: this.cart.items.map((item) => ({
        productId: item.product.id,
        quantity: item.quantity,
      })),
      shippingAddress: `${this.shippingAddress.street}, ${this.shippingAddress.city}, ${this.shippingAddress.state} ${this.shippingAddress.zipCode}, ${this.shippingAddress.country}`,
      notes: this.orderNotes,
    };

    this.orderService
      .createOrder(orderData)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.notificationService.showSuccess('Order placed successfully!');
          this.cartService.clearCart();
          this.currentStep = 'confirmation';
          this.creatingOrder = false;
        },
        error: (error) => {
          this.notificationService.showError(
            `Failed to place order: ${error.message || 'Unknown error'}`
          );
          this.creatingOrder = false;
        },
      });
  }

  onStartNewOrder(): void {
    this.currentStep = 'cart';
    this.selectedCustomer = null;
    this.shippingAddress = {
      street: '',
      city: '',
      state: '',
      zipCode: '',
      country: '',
    };
    this.orderNotes = '';
  }

  onItemUpdated(): void {
    // Item was updated, cart will automatically refresh
  }

  onItemRemoved(): void {
    // Item was removed, cart will automatically refresh
  }

  private isShippingAddressValid(): boolean {
    return !!(
      this.shippingAddress.street &&
      this.shippingAddress.city &&
      this.shippingAddress.state &&
      this.shippingAddress.zipCode &&
      this.shippingAddress.country
    );
  }

  getStepTitle(): string {
    const titles = {
      cart: 'Shopping Cart',
      customer: 'Select Customer',
      shipping: 'Shipping Information',
      review: 'Review Order',
      confirmation: 'Order Confirmation',
    };
    return titles[this.currentStep];
  }

  getStepDescription(): string {
    const descriptions = {
      cart: 'Review your items and proceed to checkout',
      customer: 'Choose an existing customer or create a new one',
      shipping: 'Enter shipping address and delivery preferences',
      review: 'Review your order details before placing it',
      confirmation: 'Your order has been placed successfully',
    };
    return descriptions[this.currentStep];
  }

  getStepClass(step: string, index: number): string {
    const stepOrder = [
      'cart',
      'customer',
      'shipping',
      'review',
      'confirmation',
    ];
    const currentIndex = stepOrder.indexOf(this.currentStep);
    const stepIndex = stepOrder.indexOf(step);

    if (stepIndex < currentIndex) {
      return 'flex items-center'; // Completed step
    } else if (stepIndex === currentIndex) {
      return 'flex items-center'; // Current step
    } else {
      return 'flex items-center'; // Future step
    }
  }

  getStepCircleClass(step: string, index: number): string {
    const stepOrder = [
      'cart',
      'customer',
      'shipping',
      'review',
      'confirmation',
    ];
    const currentIndex = stepOrder.indexOf(this.currentStep);
    const stepIndex = stepOrder.indexOf(step);

    if (stepIndex < currentIndex) {
      return 'flex items-center justify-center w-8 h-8 rounded-full bg-green-600 text-white'; // Completed
    } else if (stepIndex === currentIndex) {
      return 'flex items-center justify-center w-8 h-8 rounded-full bg-blue-600 text-white'; // Current
    } else {
      return 'flex items-center justify-center w-8 h-8 rounded-full bg-gray-300 text-gray-600'; // Future
    }
  }

  getStepNumber(step: string, index: number): number | null {
    const stepOrder = [
      'cart',
      'customer',
      'shipping',
      'review',
      'confirmation',
    ];
    const currentIndex = stepOrder.indexOf(this.currentStep);
    const stepIndex = stepOrder.indexOf(step);

    if (stepIndex < currentIndex) {
      return null; // Show checkmark for completed steps
    } else {
      return stepIndex + 1; // Show number for current and future steps
    }
  }

  getStepLabelClass(step: string, index: number): string {
    const stepOrder = [
      'cart',
      'customer',
      'shipping',
      'review',
      'confirmation',
    ];
    const currentIndex = stepOrder.indexOf(this.currentStep);
    const stepIndex = stepOrder.indexOf(step);

    if (stepIndex < currentIndex) {
      return 'text-green-600'; // Completed
    } else if (stepIndex === currentIndex) {
      return 'text-blue-600'; // Current
    } else {
      return 'text-gray-500'; // Future
    }
  }

  getStepLabel(step: string): string {
    const labels = {
      cart: 'Cart',
      customer: 'Customer',
      shipping: 'Shipping',
      review: 'Review',
      confirmation: 'Confirmation',
    };
    return labels[step as keyof typeof labels];
  }

  formatPrice(price: number, currency: string): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currency,
    }).format(price);
  }
}
