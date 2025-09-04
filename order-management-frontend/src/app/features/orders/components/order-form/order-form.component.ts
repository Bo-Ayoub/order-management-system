import {
  Component,
  EventEmitter,
  Input,
  Output,
  OnInit,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
  FormArray,
} from '@angular/forms';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';

import {
  CreateOrderRequest,
  OrderStatus,
} from '../../../../shared/models/order.model';
import { Customer } from '../../../../shared/models/customer.model';
import { Product } from '../../../../shared/models/product.model';
import { CustomerService } from '../../../customers/services/customer.service';
import { ProductService } from '../../../products/services/product.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-order-form',
  standalone: false,
  templateUrl: './order-form.component.html',
  styleUrls: ['./order-form.component.scss'],
})
export class OrderFormComponent implements OnInit, OnDestroy {
  @Input() order: CreateOrderRequest | null = null;
  @Input() loading = false;
  @Output() save = new EventEmitter<CreateOrderRequest>();
  @Output() cancel = new EventEmitter<void>();

  orderForm: FormGroup;
  customers: Customer[] = [];
  products: Product[] = [];
  filteredCustomers: Customer[] = [];
  filteredProducts: Product[] = [];
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private customerService: CustomerService,
    private productService: ProductService,
    private cartService: CartService
  ) {
    this.orderForm = this.createForm();
  }

  ngOnInit(): void {
    this.loadCustomers();
    this.loadProducts();
    this.setupFormSubscriptions();

    if (this.order) {
      this.populateForm();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      customerId: ['', Validators.required],
      shippingAddress: [''],
      notes: [''],
      items: this.fb.array([]),
    });
  }

  private setupFormSubscriptions(): void {
    // Watch for customer search
    this.orderForm
      .get('customerId')
      ?.valueChanges.pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe((value) => {
        this.filterCustomers(value);
      });
  }

  private loadCustomers(): void {
    this.customerService
      .getCustomers({ pageSize: 1000 })
      .pipe(takeUntil(this.destroy$))
      .subscribe((response) => {
        this.customers = response.data;
        this.filteredCustomers = this.customers;
      });
  }

  private loadProducts(): void {
    this.productService
      .getProducts({ pageSize: 1000 })
      .pipe(takeUntil(this.destroy$))
      .subscribe((response) => {
        this.products = response.data;
        this.filteredProducts = this.products;
      });
  }

  private populateForm(): void {
    if (this.order) {
      this.orderForm.patchValue({
        customerId: this.order.customerId,
        shippingAddress: this.order.shippingAddress || '',
        notes: this.order.notes || '',
      });

      // Clear existing items
      const itemsArray = this.orderForm.get('items') as FormArray;
      itemsArray.clear();

      // Add order items
      this.order.items.forEach((item) => {
        this.addOrderItem(item.productId, item.quantity);
      });
    }
  }

  private filterCustomers(searchTerm: string): void {
    if (!searchTerm) {
      this.filteredCustomers = this.customers;
      return;
    }

    this.filteredCustomers = this.customers.filter(
      (customer) =>
        customer.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        customer.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        customer.email.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }

  filterProducts(searchTerm: string): void {
    if (!searchTerm) {
      this.filteredProducts = this.products;
      return;
    }

    this.filteredProducts = this.products.filter(
      (product) =>
        product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        (product.description &&
          product.description.toLowerCase().includes(searchTerm.toLowerCase()))
    );
  }

  get itemsArray(): FormArray {
    return this.orderForm.get('items') as FormArray;
  }

  addOrderItem(productId: string = '', quantity: number = 1): void {
    const itemForm = this.fb.group({
      productId: [productId, Validators.required],
      quantity: [quantity, [Validators.required, Validators.min(1)]],
    });

    this.itemsArray.push(itemForm);
  }

  removeOrderItem(index: number): void {
    this.itemsArray.removeAt(index);
  }

  addFromCart(): void {
    this.cartService
      .getCart()
      .pipe(takeUntil(this.destroy$))
      .subscribe((cart) => {
        // Clear existing items
        this.itemsArray.clear();

        // Add cart items
        cart.items.forEach((item) => {
          this.addOrderItem(item.product.id, item.quantity);
        });
      });
  }

  getProductName(productId: string): string {
    const product = this.products.find((p) => p.id === productId);
    return product ? product.name : 'Unknown Product';
  }

  getProductPrice(productId: string): number {
    const product = this.products.find((p) => p.id === productId);
    return product ? product.price : 0;
  }

  getProductCurrency(productId: string): string {
    const product = this.products.find((p) => p.id === productId);
    return product ? product.currency : 'USD';
  }

  getItemTotal(productId: string, quantity: number): number {
    return this.getProductPrice(productId) * quantity;
  }

  getOrderTotal(): number {
    let total = 0;
    this.itemsArray.controls.forEach((control) => {
      const productId = control.get('productId')?.value;
      const quantity = control.get('quantity')?.value || 0;
      total += this.getItemTotal(productId, quantity);
    });
    return total;
  }

  formatPrice(price: number, currency: string): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currency,
    }).format(price);
  }

  onSubmit(): void {
    if (this.orderForm.valid) {
      const formValue = this.orderForm.value;
      const orderRequest: CreateOrderRequest = {
        customerId: formValue.customerId,
        items: formValue.items.map((item: any) => ({
          productId: item.productId,
          quantity: item.quantity,
        })),
        shippingAddress: formValue.shippingAddress || undefined,
        notes: formValue.notes || undefined,
      };

      this.save.emit(orderRequest);
    } else {
      this.markFormGroupTouched();
    }
  }

  onCancel(): void {
    this.cancel.emit();
  }

  private markFormGroupTouched(): void {
    Object.keys(this.orderForm.controls).forEach((key) => {
      const control = this.orderForm.get(key);
      control?.markAsTouched();

      if (control instanceof FormArray) {
        control.controls.forEach((arrayControl) => {
          Object.keys(arrayControl.value).forEach((arrayKey) => {
            const arrayFormControl = arrayControl.get(arrayKey);
            arrayFormControl?.markAsTouched();
          });
        });
      }
    });
  }

  getInputClasses(control: any): string {
    const baseClasses =
      'block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm';

    if (control.invalid && control.touched) {
      return (
        baseClasses +
        ' border-red-300 text-red-900 placeholder-red-300 focus:ring-red-500 focus:border-red-500'
      );
    }

    return baseClasses + ' border-gray-300';
  }

  getSelectClasses(control: any): string {
    const baseClasses =
      'block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm';

    if (control.invalid && control.touched) {
      return (
        baseClasses +
        ' border-red-300 text-red-900 focus:ring-red-500 focus:border-red-500'
      );
    }

    return baseClasses + ' border-gray-300';
  }
}
