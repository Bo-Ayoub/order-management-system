import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../../core/services/api.service';
import {
  Product,
  CreateProductRequest,
  ProductSummary,
} from '../../../shared/models/product.model';
import { PaginatedResponse } from '../../../shared/models/api-response.model';

export interface ProductFilters {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  category?: string;
  isActive?: boolean;
  minPrice?: number;
  maxPrice?: number;
}

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  constructor(@Inject(ApiService) private apiService: ApiService) {}

  getProducts(
    filters: ProductFilters = {}
  ): Observable<PaginatedResponse<Product>> {
    const params: any = {
      pageNumber: filters.pageNumber || 1,
      pageSize: filters.pageSize || 10,
    };

    // Add optional filters
    if (filters.searchTerm) params.searchTerm = filters.searchTerm;
    if (filters.category) params.category = filters.category;
    if (filters.isActive !== undefined) params.isActive = filters.isActive;
    if (filters.minPrice !== undefined) params.minPrice = filters.minPrice;
    if (filters.maxPrice !== undefined) params.maxPrice = filters.maxPrice;

    return this.apiService.get<PaginatedResponse<Product>>('/products', params);
  }

  getProduct(id: string): Observable<Product> {
    return this.apiService.get<Product>(`/products/${id}`);
  }

  createProduct(product: CreateProductRequest): Observable<{ id: string }> {
    return this.apiService.post<{ id: string }>('/products', product);
  }

  updateProduct(
    id: string,
    product: Partial<CreateProductRequest>
  ): Observable<void> {
    return this.apiService.put<void>(`/products/${id}`, product);
  }

  deleteProduct(id: string): Observable<void> {
    return this.apiService.delete<void>(`/products/${id}`);
  }

  // Get active products only
  getActiveProducts(
    filters: Omit<ProductFilters, 'isActive'> = {}
  ): Observable<PaginatedResponse<Product>> {
    return this.getProducts({ ...filters, isActive: true });
  }

  // Search products by name
  searchProducts(
    searchTerm: string,
    pageSize: number = 10
  ): Observable<Product[]> {
    return this.getProducts({
      searchTerm,
      pageSize,
      pageNumber: 1,
      isActive: true,
    }).pipe(map((response) => response.data));
  }

  // Get product summaries for order creation
  getProductSummaries(): Observable<ProductSummary[]> {
    return this.getActiveProducts({ pageSize: 100 }).pipe(
      map((response) =>
        response.data.map((product) => ({
          id: product.id,
          name: product.name,
          price: product.price,
          currency: product.currency,
          stockQuantity: product.stockQuantity,
          isActive: product.isActive,
        }))
      )
    );
  }

  // Get unique categories
  getCategories(): Observable<string[]> {
    return this.getProducts({ pageSize: 100 }).pipe(
      map((response) => {
        const categories = response.data
          .map((p) => p.category)
          .filter((category): category is string => !!category);
        return [...new Set(categories)].sort();
      })
    );
  }

  // Check stock availability
  checkStock(productId: string, quantity: number): Observable<boolean> {
    return this.getProduct(productId).pipe(
      map((product) => product.stockQuantity >= quantity)
    );
  }

  // Get low stock products (stock <= 10)
  getLowStockProducts(): Observable<Product[]> {
    return this.getProducts({ pageSize: 100, isActive: true }).pipe(
      map((response) =>
        response.data.filter((product) => product.stockQuantity <= 10)
      )
    );
  }
}
