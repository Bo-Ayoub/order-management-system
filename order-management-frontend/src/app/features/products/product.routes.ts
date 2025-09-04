import { Routes } from '@angular/router';
import { ProductListPageComponent } from './pages/product-list-page/product-list-page.component';
import { ProductCreatePageComponent } from './pages/product-create-page/product-create-page.component';
import { ProductEditPageComponent } from './pages/product-edit-page/product-edit-page.component';
import { ProductDetailPageComponent } from './pages/product-detail-page/product-detail-page.component';

export const productRoutes: Routes = [
  {
    path: '',
    component: ProductListPageComponent,
    title: 'Products - Order Management'
  },
  {
    path: 'create',
    component: ProductCreatePageComponent,
    title: 'Create Product - Order Management'
  },
  {
    path: ':id',
    component: ProductDetailPageComponent,
    title: 'Product Details - Order Management'
  },
  {
    path: ':id/edit',
    component: ProductEditPageComponent,
    title: 'Edit Product - Order Management'
  }
];
