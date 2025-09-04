import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'customers',
    loadChildren: () =>
      import('./features/customers/customer.module').then(
        (m) => m.CustomerModule
      ),
  },
  {
    path: 'products',
    loadChildren: () =>
      import('./features/products/product.module').then(
        (m) => m.ProductModule
      ),
  },
  {
    path: 'orders',
    loadChildren: () =>
      import('./features/orders/order.module').then(
        (m) => m.OrderModule
      ),
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/pages/dashboard-page/dashboard-page.component').then(
        (c) => c.DashboardPageComponent
      ),
  },
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full',
  },
];
