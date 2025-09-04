import { Component, EventEmitter, Input, Output, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

import { Product, CreateProductRequest } from '../../../../shared/models/product.model';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-product-form',
  standalone: false,
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit, OnDestroy {
  @Input() product: Product | null = null;
  @Input() loading = false;
  @Input() isEditMode = false;
  @Input() categories: string[] = [];
  
  @Output() formSubmit = new EventEmitter<CreateProductRequest>();
  @Output() formCancel = new EventEmitter<void>();

  productForm: FormGroup;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private router: Router
  ) {
    this.productForm = this.createForm();
  }

  ngOnInit(): void {
    if (this.product && this.isEditMode) {
      this.populateForm();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      price: ['', [Validators.required, Validators.min(0.01)]],
      currency: ['USD', [Validators.required]],
      stockQuantity: ['', [Validators.required, Validators.min(0)]],
      category: [''],
      isActive: [true]
    });
  }

  private populateForm(): void {
    if (this.product) {
      this.productForm.patchValue({
        name: this.product.name,
        description: this.product.description || '',
        price: this.product.price,
        currency: this.product.currency,
        stockQuantity: this.product.stockQuantity,
        category: this.product.category || '',
        isActive: this.product.isActive
      });
    }
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      const formValue = this.productForm.value;
      const productData: CreateProductRequest = {
        name: formValue.name.trim(),
        description: formValue.description?.trim() || undefined,
        price: parseFloat(formValue.price),
        currency: formValue.currency,
        stockQuantity: parseInt(formValue.stockQuantity),
        category: formValue.category?.trim() || undefined
      };
      
      this.formSubmit.emit(productData);
    } else {
      this.markFormGroupTouched();
    }
  }

  onCancel(): void {
    this.formCancel.emit();
  }

  private markFormGroupTouched(): void {
    Object.keys(this.productForm.controls).forEach(key => {
      const control = this.productForm.get(key);
      control?.markAsTouched();
    });
  }

  getFieldError(fieldName: string): string {
    const field = this.productForm.get(fieldName);
    if (field?.errors && field.touched) {
      if (field.errors['required']) {
        return `${this.getFieldLabel(fieldName)} is required`;
      }
      if (field.errors['minlength']) {
        return `${this.getFieldLabel(fieldName)} must be at least ${field.errors['minlength'].requiredLength} characters`;
      }
      if (field.errors['maxlength']) {
        return `${this.getFieldLabel(fieldName)} must not exceed ${field.errors['maxlength'].requiredLength} characters`;
      }
      if (field.errors['min']) {
        return `${this.getFieldLabel(fieldName)} must be greater than ${field.errors['min'].min}`;
      }
    }
    return '';
  }

  private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      name: 'Product Name',
      description: 'Description',
      price: 'Price',
      currency: 'Currency',
      stockQuantity: 'Stock Quantity',
      category: 'Category',
      isActive: 'Active Status'
    };
    return labels[fieldName] || fieldName;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.productForm.get(fieldName);
    return !!(field?.invalid && field.touched);
  }

  isFieldValid(fieldName: string): boolean {
    const field = this.productForm.get(fieldName);
    return !!(field?.valid && field.touched);
  }

  getInputClasses(fieldName: string): string {
    const baseClasses = 'block w-full px-3 py-2 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-1 sm:text-sm transition-colors duration-200';
    
    if (this.isFieldInvalid(fieldName)) {
      return `${baseClasses} border-red-300 text-red-900 placeholder-red-300 focus:ring-red-500 focus:border-red-500`;
    } else if (this.isFieldValid(fieldName)) {
      return `${baseClasses} border-green-300 text-green-900 placeholder-green-300 focus:ring-green-500 focus:border-green-500`;
    } else {
      return `${baseClasses} border-gray-300 text-gray-900 placeholder-gray-400 focus:ring-blue-500 focus:border-blue-500`;
    }
  }

  getSelectClasses(fieldName: string): string {
    const baseClasses = 'block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-1 sm:text-sm transition-colors duration-200';
    
    if (this.isFieldInvalid(fieldName)) {
      return `${baseClasses} border-red-300 text-red-900 focus:ring-red-500 focus:border-red-500`;
    } else if (this.isFieldValid(fieldName)) {
      return `${baseClasses} border-green-300 text-green-900 focus:ring-green-500 focus:border-green-500`;
    } else {
      return `${baseClasses} border-gray-300 text-gray-900 focus:ring-blue-500 focus:border-blue-500`;
    }
  }
}
