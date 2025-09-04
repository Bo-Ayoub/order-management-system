import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Components
import { ProductListComponent } from './components/product-list/product-list.component';
import { ProductFormComponent } from './components/product-form/product-form.component';
import { ProductDetailComponent } from './components/product-detail/product-detail.component';
import { ProductCardComponent } from './components/product-card/product-card.component';

// Pages
import { ProductListPageComponent } from './pages/product-list-page/product-list-page.component';
import { ProductCreatePageComponent } from './pages/product-create-page/product-create-page.component';
import { ProductEditPageComponent } from './pages/product-edit-page/product-edit-page.component';
import { ProductDetailPageComponent } from './pages/product-detail-page/product-detail-page.component';

// Services
import { ProductService } from './services/product.service';

// Routing
import { productRoutes } from './product.routes';

@NgModule({
  declarations: [
    // Components
    ProductListComponent,
    ProductFormComponent,
    ProductDetailComponent,
    ProductCardComponent,
    
    // Pages
    ProductListPageComponent,
    ProductCreatePageComponent,
    ProductEditPageComponent,
    ProductDetailPageComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule.forChild(productRoutes),
  ],
  providers: [],
  exports: [
    ProductListComponent,
    ProductFormComponent,
    ProductDetailComponent,
    ProductCardComponent,
  ],
})
export class ProductModule {}
