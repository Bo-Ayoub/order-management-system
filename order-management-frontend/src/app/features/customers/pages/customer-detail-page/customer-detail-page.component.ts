import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import { Customer } from '../../../../shared/models/customer.model';
import { CustomerService } from '../../services/customer.service';
import { CustomerDetailComponent } from '../../components/customer-detail/customer-detail.component';

@Component({
  selector: 'app-customer-detail-page',
  standalone: false,
  templateUrl: './customer-detail-page.component.html',
  styleUrls: ['./customer-detail-page.component.scss'],
})
export class CustomerDetailPageComponent implements OnInit, OnDestroy {
  customer: Customer | null = null;
  loading = false;

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

    this.loading = true;
    this.customerService
      .getCustomer(customerId)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => (this.loading = false))
      )
      .subscribe({
        next: (customer) => {
          this.customer = customer;
        },
        error: (error) => {
          console.error('Error loading customer:', error);
          this.customer = null;
        },
      });
  }

  onEditCustomer(): void {
    if (this.customer) {
      this.router.navigate(['/customers', this.customer.id, 'edit']);
    }
  }

  onBackToList(): void {
    this.router.navigate(['/customers']);
  }
}
