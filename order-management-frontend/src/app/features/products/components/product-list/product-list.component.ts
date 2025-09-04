import {
  Component,
  EventEmitter,
  Input,
  Output,
  OnInit,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';

import {
  Product,
  ProductSummary,
} from '../../../../shared/models/product.model';
import { PaginatedResponse } from '../../../../shared/models/api-response.model';
import { ProductService, ProductFilters } from '../../services/product.service';
import { CartService } from '../../../orders/services/cart.service';

@Component({
  selector: 'app-product-list',
  standalone: false,
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class ProductListComponent implements OnInit, OnDestroy {
  @Input() products: Product[] = [];
  @Input() pagination: PaginatedResponse<Product>['pagination'] | null = null;
  @Input() loading = false;
  @Input() searchTerm = '';
  @Input() selectedCategory = '';
  @Input() categories: string[] = [];

  @Output() searchChange = new EventEmitter<string>();
  @Output() categoryChange = new EventEmitter<string>();
  @Output() pageChange = new EventEmitter<number>();
  @Output() deleteProduct = new EventEmitter<string>();
  @Output() refresh = new EventEmitter<void>();
  @Output() addToCart = new EventEmitter<{
    productId: string;
    quantity: number;
  }>();

  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  constructor(
    private productService: ProductService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    // Setup search debouncing
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe((searchTerm) => {
        this.searchChange.emit(searchTerm);
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSearchChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchTerm = target.value;
    this.searchSubject.next(this.searchTerm);
  }

  onCategoryChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    this.selectedCategory = target.value;
    this.categoryChange.emit(this.selectedCategory);
  }

  onPageChange(page: number): void {
    this.pageChange.emit(page);
  }

  onDeleteProduct(productId: string): void {
    if (confirm('Are you sure you want to delete this product?')) {
      this.deleteProduct.emit(productId);
    }
  }

  onRefresh(): void {
    this.refresh.emit();
  }

  onAddToCart(productId: string, quantity: number = 1): void {
    this.addToCart.emit({ productId, quantity });
  }

  canAddToCart(product: Product): boolean {
    return product.isActive && product.stockQuantity > 0;
  }

  formatPrice(price: number, currency: string): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currency,
    }).format(price);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
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

  getPageNumbers(): number[] {
    if (!this.pagination) return [];

    const currentPage = this.pagination.pageNumber;
    const totalPages = this.pagination.totalPages;
    const pages: number[] = [];

    // Show up to 5 page numbers
    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, startPage + 4);

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    return pages;
  }

  // Expose Math to template
  Math = Math;
}
