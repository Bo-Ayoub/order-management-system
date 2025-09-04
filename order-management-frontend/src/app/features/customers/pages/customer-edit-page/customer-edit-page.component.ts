import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import {
  Customer,
  CreateCustomerRequest,
} from '../../../../shared/models/customer.model';
import { CustomerService } from '../../services/customer.service';
import { CustomerFormComponent } from '../../components/customer-form/customer-form.component';

@Component({
  selector: 'app-customer-edit-page',
  standalone: false,
  templateUrl: './customer-edit-page.component.html',
  styleUrls: ['./customer-edit-page.component.scss'],
})
export class CustomerEditPageComponent implements OnInit, OnDestroy {
  customer: Customer | null = null;
  loading = false;
  pageLoading = true;

  private destroy$ = new Subject<void>();

  constructor(
    private customerService: CustomerService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCustomer();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCustomer(): void {
    const customerId = this.route.snapshot.paramMap.get('id');
    if (!customerId) {
      this.router.navigate(['/customers']);
      return;
    }

    this.pageLoading = true;
    this.customerService
      .getCustomer(customerId)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => (this.pageLoading = false))
      )
      .subscribe({
        next: (customer) => {
          this.customer = customer;
        },
        error: (error) => {
          console.error('Error loading customer:', error);
          this.router.navigate(['/customers']);
        },
      });
  }

  onFormSubmit(customerData: CreateCustomerRequest): void {
    if (!this.customer) return;

    this.loading = true;

    this.customerService
      .updateCustomer(this.customer.id, customerData)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => (this.loading = false))
      )
      .subscribe({
        next: () => {
          console.log('Customer updated successfully');
          // Navigate to customer detail page
          this.router.navigate(['/customers', this.customer!.id]);
        },
        error: (error) => {
          console.error('Error updating customer:', error);
          // Handle error - could show notification
        },
      });
  }

  onFormCancel(): void {
    if (this.customer) {
      this.router.navigate(['/customers', this.customer.id]);
    } else {
      this.router.navigate(['/customers']);
    }
  }
}
