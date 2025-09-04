import { Routes } from '@angular/router';
import { CustomerListPageComponent } from './pages/customer-list-page/customer-list-page.component';
import { CustomerCreatePageComponent } from './pages/customer-create-page/customer-create-page.component';
import { CustomerEditPageComponent } from './pages/customer-edit-page/customer-edit-page.component';
import { CustomerDetailPageComponent } from './pages/customer-detail-page/customer-detail-page.component';

export const customerRoutes: Routes = [
  {
    path: '',
    component: CustomerListPageComponent,
    title: 'Customers - Order Management'
  },
  {
    path: 'create',
    component: CustomerCreatePageComponent,
    title: 'Create Customer - Order Management'
  },
  {
    path: ':id',
    component: CustomerDetailPageComponent,
    title: 'Customer Details - Order Management'
  },
  {
    path: ':id/edit',
    component: CustomerEditPageComponent,
    title: 'Edit Customer - Order Management'
  }
];
