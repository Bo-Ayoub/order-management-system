import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import { CreateCustomerRequest } from '../../../../shared/models/customer.model';
import { CustomerService } from '../../services/customer.service';
import { CustomerFormComponent } from '../../components/customer-form/customer-form.component';

@Component({
  selector: 'app-customer-create-page',
  standalone: false,
  templateUrl: './customer-create-page.component.html',
  styleUrls: ['./customer-create-page.component.scss'],
})
export class CustomerCreatePageComponent implements OnInit, OnDestroy {
  loading = false;

  private destroy$ = new Subject<void>();

  constructor(
    private customerService: CustomerService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onFormSubmit(customerData: CreateCustomerRequest): void {
    this.loading = true;

    this.customerService
      .createCustomer(customerData)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => (this.loading = false))
      )
      .subscribe({
        next: (response) => {
          console.log('Customer created successfully:', response);
          // Navigate to customer detail page
          this.router.navigate(['/customers', response.id]);
        },
        error: (error) => {
          console.error('Error creating customer:', error);
          // Handle error - could show notification
        },
      });
  }

  onFormCancel(): void {
    this.router.navigate(['/customers']);
  }
}
