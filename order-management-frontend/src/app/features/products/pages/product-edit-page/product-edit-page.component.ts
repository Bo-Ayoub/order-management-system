import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import { Product, CreateProductRequest } from '../../../../shared/models/product.model';
import { ProductService } from '../../services/product.service';
import { ProductFormComponent } from '../../components/product-form/product-form.component';

@Component({
  selector: 'app-product-edit-page',
  standalone: false,
  templateUrl: './product-edit-page.component.html',
  styleUrls: ['./product-edit-page.component.scss']
})
export class ProductEditPageComponent implements OnInit, OnDestroy {
  product: Product | null = null;
  loading = false;
  pageLoading = true;
  categories: string[] = [];
  
  private destroy$ = new Subject<void>();

  constructor(
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProduct();
    this.loadCategories();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadProduct(): void {
    const productId = this.route.snapshot.paramMap.get('id');
    if (!productId) {
      this.router.navigate(['/products']);
      return;
    }

    this.pageLoading = true;
    this.productService.getProduct(productId)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.pageLoading = false)
      )
      .subscribe({
        next: (product) => {
          this.product = product;
        },
        error: (error) => {
          console.error('Error loading product:', error);
          this.router.navigate(['/products']);
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

  onFormSubmit(productData: CreateProductRequest): void {
    if (!this.product) return;

    this.loading = true;
    
    this.productService.updateProduct(this.product.id, productData)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.loading = false)
      )
      .subscribe({
        next: () => {
          console.log('Product updated successfully');
          // Navigate to product detail page
          this.router.navigate(['/products', this.product!.id]);
        },
        error: (error) => {
          console.error('Error updating product:', error);
          // Handle error - could show notification
        }
      });
  }

  onFormCancel(): void {
    if (this.product) {
      this.router.navigate(['/products', this.product.id]);
    } else {
      this.router.navigate(['/products']);
    }
  }
}
