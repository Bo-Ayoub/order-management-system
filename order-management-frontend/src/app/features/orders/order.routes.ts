import { Routes } from '@angular/router';
import { OrderListPageComponent } from './pages/order-list-page/order-list-page.component';
import { OrderCreatePageComponent } from './pages/order-create-page/order-create-page.component';
import { OrderEditPageComponent } from './pages/order-edit-page/order-edit-page.component';
import { OrderDetailPageComponent } from './pages/order-detail-page/order-detail-page.component';
import { ShoppingCartPageComponent } from './pages/shopping-cart-page/shopping-cart-page.component';

export const orderRoutes: Routes = [
  {
    path: '',
    component: OrderListPageComponent,
    title: 'Orders - Order Management'
  },
  {
    path: 'create',
    component: OrderCreatePageComponent,
    title: 'Create Order - Order Management'
  },
  {
    path: 'cart',
    component: ShoppingCartPageComponent,
    title: 'Shopping Cart - Order Management'
  },
  {
    path: ':id',
    component: OrderDetailPageComponent,
    title: 'Order Details - Order Management'
  },
  {
    path: ':id/edit',
    component: OrderEditPageComponent,
    title: 'Edit Order - Order Management'
  }
];
