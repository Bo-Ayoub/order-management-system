import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

import { Customer } from '../../../../shared/models/customer.model';

@Component({
  selector: 'app-customer-detail',
  standalone: false,
  templateUrl: './customer-detail.component.html',
  styleUrls: ['./customer-detail.component.scss']
})
export class CustomerDetailComponent implements OnInit, OnDestroy {
  @Input() customer: Customer | null = null;
  @Input() loading = false;

  private destroy$ = new Subject<void>();

  constructor() {}

  ngOnInit(): void {}

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  getFullName(): string {
    if (!this.customer) return '';
    return `${this.customer.firstName} ${this.customer.lastName}`;
  }

  getInitials(): string {
    if (!this.customer) return '';
    return `${this.customer.firstName.charAt(0)}${this.customer.lastName.charAt(0)}`.toUpperCase();
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatPhoneNumber(phoneNumber?: string): string {
    if (!phoneNumber) return 'Not provided';
    
    // Basic phone number formatting
    const cleaned = phoneNumber.replace(/\D/g, '');
    if (cleaned.length === 10) {
      return `(${cleaned.slice(0, 3)}) ${cleaned.slice(3, 6)}-${cleaned.slice(6)}`;
    }
    return phoneNumber;
  }

  getEmailHref(): string {
    if (!this.customer?.email) return '';
    return `mailto:${this.customer.email}`;
  }

  getPhoneHref(): string {
    if (!this.customer?.phoneNumber) return '';
    const cleaned = this.customer.phoneNumber.replace(/\D/g, '');
    return `tel:${cleaned}`;
  }
}
