import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import { CreateProductRequest } from '../../../../shared/models/product.model';
import { ProductService } from '../../services/product.service';
import { ProductFormComponent } from '../../components/product-form/product-form.component';

@Component({
  selector: 'app-product-create-page',
  standalone: false,
  templateUrl: './product-create-page.component.html',
  styleUrls: ['./product-create-page.component.scss']
})
export class ProductCreatePageComponent implements OnInit, OnDestroy {
  loading = false;
  categories: string[] = [];
  
  private destroy$ = new Subject<void>();

  constructor(
    private productService: ProductService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
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
    this.loading = true;
    
    this.productService.createProduct(productData)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.loading = false)
      )
      .subscribe({
        next: (response) => {
          console.log('Product created successfully:', response);
          // Navigate to product detail page
          this.router.navigate(['/products', response.id]);
        },
        error: (error) => {
          console.error('Error creating product:', error);
          // Handle error - could show notification
        }
      });
  }

  onFormCancel(): void {
    this.router.navigate(['/products']);
  }
}
