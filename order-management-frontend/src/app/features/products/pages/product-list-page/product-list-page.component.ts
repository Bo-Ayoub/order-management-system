import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import { Product } from '../../../../shared/models/product.model';
import { PaginatedResponse } from '../../../../shared/models/api-response.model';
import { ProductService, ProductFilters } from '../../services/product.service';
import { ProductListComponent } from '../../components/product-list/product-list.component';

@Component({
  selector: 'app-product-list-page',
  standalone: false,
  templateUrl: './product-list-page.component.html',
  styleUrls: ['./product-list-page.component.scss']
})
export class ProductListPageComponent implements OnInit, OnDestroy {
  products: Product[] = [];
  pagination: PaginatedResponse<Product>['pagination'] | null = null;
  loading = false;
  searchTerm = '';
  selectedCategory = '';
  categories: string[] = [];
  
  private destroy$ = new Subject<void>();
  private currentFilters: ProductFilters = {
    pageNumber: 1,
    pageSize: 10,
    searchTerm: '',
    category: '',
    isActive: true
  };

  constructor(
    private productService: ProductService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadProducts(): void {
    this.loading = true;
    this.productService.getProducts(this.currentFilters)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.loading = false)
      )
      .subscribe({
        next: (response) => {
          this.products = response.data;
          this.pagination = response.pagination;
        },
        error: (error) => {
          console.error('Error loading products:', error);
          // Handle error - could show notification
        }
      });
  }

  loadCategories(): void {
    this.productService.getCategories()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (categories) => {
          this.categories = categories;
        },
        error: (error) => {
          console.error('Error loading categories:', error);
        }
      });
  }

  onSearchChange(searchTerm: string): void {
    this.searchTerm = searchTerm;
    this.currentFilters = {
      ...this.currentFilters,
      searchTerm,
      pageNumber: 1 // Reset to first page when searching
    };
    this.loadProducts();
  }

  onCategoryChange(category: string): void {
    this.selectedCategory = category;
    this.currentFilters = {
      ...this.currentFilters,
      category: category || undefined,
      pageNumber: 1 // Reset to first page when filtering
    };
    this.loadProducts();
  }

  onPageChange(page: number): void {
    this.currentFilters = {
      ...this.currentFilters,
      pageNumber: page
    };
    this.loadProducts();
  }

  onDeleteProduct(productId: string): void {
    this.productService.deleteProduct(productId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          // Remove product from local array
          this.products = this.products.filter(p => p.id !== productId);
          // Show success notification
          console.log('Product deleted successfully');
        },
        error: (error) => {
          console.error('Error deleting product:', error);
          // Handle error - could show notification
        }
      });
  }

  onRefresh(): void {
    this.loadProducts();
  }

  onCreateProduct(): void {
    this.router.navigate(['/products/create']);
  }
}
