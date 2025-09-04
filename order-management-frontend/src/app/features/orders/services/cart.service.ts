import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, combineLatest } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { ProductService } from '../../products/services/product.service';
import { Product } from '../../../shared/models/product.model';

export interface CartItem {
  product: Product;
  quantity: number;
  subtotal: number;
}

export interface Cart {
  items: CartItem[];
  totalItems: number;
  totalAmount: number;
  currency: string;
}

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private cartItems$ = new BehaviorSubject<CartItem[]>([]);
  private readonly STORAGE_KEY = 'order_cart';

  constructor(private productService: ProductService) {
    // Load cart from localStorage on service initialization
    this.loadCartFromStorage();
  }

  // Get current cart as observable
  getCart(): Observable<Cart> {
    return this.cartItems$.pipe(
      map((items) => this.calculateCartTotals(items))
    );
  }

  // Get cart items
  getCartItems(): Observable<CartItem[]> {
    return this.cartItems$.asObservable();
  }

  // Add item to cart
  addToCart(productId: string, quantity: number = 1): Observable<boolean> {
    return this.productService.getProduct(productId).pipe(
      tap((product) => {
        const currentItems = this.cartItems$.value;
        const existingItemIndex = currentItems.findIndex(
          (item) => item.product.id === productId
        );

        if (existingItemIndex >= 0) {
          // Update existing item
          const updatedItems = [...currentItems];
          const newQuantity =
            updatedItems[existingItemIndex].quantity + quantity;

          // Check stock availability
          if (newQuantity > product.stockQuantity) {
            throw new Error(
              `Only ${product.stockQuantity} items available in stock`
            );
          }

          updatedItems[existingItemIndex] = {
            ...updatedItems[existingItemIndex],
            quantity: newQuantity,
            subtotal: newQuantity * product.price,
          };

          this.updateCart(updatedItems);
        } else {
          // Add new item
          if (quantity > product.stockQuantity) {
            throw new Error(
              `Only ${product.stockQuantity} items available in stock`
            );
          }

          const newItem: CartItem = {
            product,
            quantity,
            subtotal: quantity * product.price,
          };

          this.updateCart([...currentItems, newItem]);
        }
      }),
      map(() => true)
    );
  }

  // Update item quantity
  updateQuantity(productId: string, quantity: number): void {
    const currentItems = this.cartItems$.value;
    const itemIndex = currentItems.findIndex(
      (item) => item.product.id === productId
    );

    if (itemIndex >= 0) {
      if (quantity <= 0) {
        this.removeFromCart(productId);
      } else {
        const updatedItems = [...currentItems];
        const product = updatedItems[itemIndex].product;

        // Check stock availability
        if (quantity > product.stockQuantity) {
          throw new Error(
            `Only ${product.stockQuantity} items available in stock`
          );
        }

        updatedItems[itemIndex] = {
          ...updatedItems[itemIndex],
          quantity,
          subtotal: quantity * product.price,
        };

        this.updateCart(updatedItems);
      }
    }
  }

  // Remove item from cart
  removeFromCart(productId: string): void {
    const currentItems = this.cartItems$.value;
    const filteredItems = currentItems.filter(
      (item) => item.product.id !== productId
    );
    this.updateCart(filteredItems);
  }

  // Clear entire cart
  clearCart(): void {
    this.updateCart([]);
  }

  // Get cart item count
  getItemCount(): Observable<number> {
    return this.cartItems$.pipe(
      map((items) => items.reduce((total, item) => total + item.quantity, 0))
    );
  }

  // Get cart total amount
  getTotalAmount(): Observable<number> {
    return this.cartItems$.pipe(
      map((items) => items.reduce((total, item) => total + item.subtotal, 0))
    );
  }

  // Check if product is in cart
  isInCart(productId: string): Observable<boolean> {
    return this.cartItems$.pipe(
      map((items) => items.some((item) => item.product.id === productId))
    );
  }

  // Get specific cart item
  getCartItem(productId: string): Observable<CartItem | undefined> {
    return this.cartItems$.pipe(
      map((items) => items.find((item) => item.product.id === productId))
    );
  }

  // Validate cart before checkout
  validateCart(): Observable<string[]> {
    return this.cartItems$.pipe(
      map((items) => {
        const errors: string[] = [];

        if (items.length === 0) {
          errors.push('Cart is empty');
          return errors;
        }

        // Validate each item
        items.forEach((item) => {
          if (item.quantity > item.product.stockQuantity) {
            errors.push(
              `${item.product.name}: Only ${item.product.stockQuantity} items available (you have ${item.quantity})`
            );
          }

          if (!item.product.isActive) {
            errors.push(`${item.product.name} is no longer available`);
          }

          if (item.quantity <= 0) {
            errors.push(`${item.product.name}: Invalid quantity`);
          }
        });

        return errors;
      })
    );
  }

  // Refresh cart items (check stock availability, prices, etc.)
  refreshCart(): Observable<Cart> {
    const currentItems = this.cartItems$.value;

    if (currentItems.length === 0) {
      return this.getCart();
    }

    // Get updated product information
    const productRequests = currentItems.map((item) =>
      this.productService.getProduct(item.product.id)
    );

    return combineLatest(productRequests).pipe(
      tap((updatedProducts) => {
        const refreshedItems: CartItem[] = [];

        updatedProducts.forEach((product, index) => {
          const currentItem = currentItems[index];

          // Check if product is still active and in stock
          if (product.isActive && product.stockQuantity > 0) {
            const availableQuantity = Math.min(
              currentItem.quantity,
              product.stockQuantity
            );

            refreshedItems.push({
              product,
              quantity: availableQuantity,
              subtotal: availableQuantity * product.price,
            });
          }
        });

        this.updateCart(refreshedItems);
      }),
      map((updatedProducts) => this.calculateCartTotals(this.cartItems$.value))
    );
  }

  // Convert cart to order request format
  toOrderRequest(customerId: string, shippingAddress?: string, notes?: string) {
    const items = this.cartItems$.value;

    return {
      customerId,
      items: items.map((item) => ({
        productId: item.product.id,
        quantity: item.quantity,
      })),
      shippingAddress,
      notes,
    };
  }

  // Private helper methods
  private updateCart(items: CartItem[]): void {
    this.cartItems$.next(items);
    this.saveCartToStorage(items);
  }

  private calculateCartTotals(items: CartItem[]): Cart {
    const totalItems = items.reduce((total, item) => total + item.quantity, 0);
    const totalAmount = items.reduce((total, item) => total + item.subtotal, 0);
    const currency = items.length > 0 ? items[0].product.currency : 'USD';

    return {
      items,
      totalItems,
      totalAmount,
      currency,
    };
  }

  private saveCartToStorage(items: CartItem[]): void {
    try {
      const cartData = items.map((item) => ({
        productId: item.product.id,
        quantity: item.quantity,
      }));
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(cartData));
    } catch (error) {
      console.warn('Failed to save cart to localStorage:', error);
    }
  }

  private loadCartFromStorage(): void {
    try {
      const savedCart = localStorage.getItem(this.STORAGE_KEY);
      if (savedCart) {
        const cartData = JSON.parse(savedCart);

        // Restore cart items by fetching current product data
        if (Array.isArray(cartData) && cartData.length > 0) {
          const productRequests = cartData.map((item: any) =>
            this.productService.getProduct(item.productId)
          );

          combineLatest(productRequests).subscribe({
            next: (products) => {
              const restoredItems: CartItem[] = [];

              products.forEach((product, index) => {
                const savedItem = cartData[index];

                if (product && product.isActive && savedItem.quantity > 0) {
                  const availableQuantity = Math.min(
                    savedItem.quantity,
                    product.stockQuantity
                  );

                  if (availableQuantity > 0) {
                    restoredItems.push({
                      product,
                      quantity: availableQuantity,
                      subtotal: availableQuantity * product.price,
                    });
                  }
                }
              });

              this.cartItems$.next(restoredItems);
            },
            error: (error) => {
              console.warn('Failed to restore cart from storage:', error);
              this.clearCart();
            },
          });
        }
      }
    } catch (error) {
      console.warn('Failed to load cart from localStorage:', error);
    }
  }
}
