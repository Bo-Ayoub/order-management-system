import {
  Component,
  EventEmitter,
  Input,
  Output,
  OnInit,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';

import { Customer } from '../../../../shared/models/customer.model';
import { CustomerService } from '../../../customers/services/customer.service';

@Component({
  selector: 'app-customer-selection',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './customer-selection.component.html',
  styleUrls: ['./customer-selection.component.scss'],
})
export class CustomerSelectionComponent implements OnInit, OnDestroy {
  @Input() selectedCustomer: Customer | null = null;
  @Input() loading = false;
  @Output() customerSelected = new EventEmitter<Customer>();
  @Output() customerCreated = new EventEmitter<Customer>();

  customers: Customer[] = [];
  searchTerm = '';
  showCreateForm = false;
  creatingCustomer = false;

  // New customer form
  newCustomer = {
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
  };

  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  constructor(private customerService: CustomerService) {}

  ngOnInit(): void {
    this.loadCustomers();

    // Setup search debouncing
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe((searchTerm) => {
        this.searchCustomers(searchTerm);
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCustomers(): void {
    this.customerService
      .getCustomers({ pageSize: 50 })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.customers = response.data;
        },
        error: (error) => {
          console.error('Error loading customers:', error);
        },
      });
  }

  onSearchChange(): void {
    this.searchSubject.next(this.searchTerm);
  }

  searchCustomers(searchTerm: string): void {
    if (!searchTerm.trim()) {
      this.loadCustomers();
      return;
    }

    this.customerService
      .getCustomers({
        pageSize: 50,
        searchTerm: searchTerm.trim(),
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.customers = response.data;
        },
        error: (error) => {
          console.error('Error searching customers:', error);
        },
      });
  }

  selectCustomer(customer: Customer): void {
    this.selectedCustomer = customer;
    this.customerSelected.emit(customer);
  }

  showCreateCustomerForm(): void {
    this.showCreateForm = true;
    this.resetNewCustomerForm();
  }

  hideCreateCustomerForm(): void {
    this.showCreateForm = false;
    this.resetNewCustomerForm();
  }

  createCustomer(): void {
    if (this.isNewCustomerFormValid()) {
      this.creatingCustomer = true;

      this.customerService
        .createCustomer(this.newCustomer)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (response) => {
            // Load the created customer
            this.customerService
              .getCustomer(response.id)
              .pipe(takeUntil(this.destroy$))
              .subscribe({
                next: (customer) => {
                  this.selectedCustomer = customer;
                  this.customerSelected.emit(customer);
                  this.customerCreated.emit(customer);
                  this.hideCreateCustomerForm();
                  this.creatingCustomer = false;
                },
                error: (error) => {
                  console.error('Error loading created customer:', error);
                  this.creatingCustomer = false;
                },
              });
          },
          error: (error) => {
            console.error('Error creating customer:', error);
            this.creatingCustomer = false;
          },
        });
    }
  }

  private resetNewCustomerForm(): void {
    this.newCustomer = {
      firstName: '',
      lastName: '',
      email: '',
      phoneNumber: '',
    };
  }

  isNewCustomerFormValid(): boolean {
    return !!(
      this.newCustomer.firstName &&
      this.newCustomer.lastName &&
      this.newCustomer.email &&
      this.newCustomer.phoneNumber
    );
  }

  getCustomerDisplayName(customer: Customer): string {
    return `${customer.firstName} ${customer.lastName}`;
  }

  getCustomerSubtitle(customer: Customer): string {
    return `${customer.email} • ${customer.phoneNumber || 'No phone'}`;
  }
}
