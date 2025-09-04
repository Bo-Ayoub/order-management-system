import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Components
import { OrderListComponent } from './components/order-list/order-list.component';
import { OrderFormComponent } from './components/order-form/order-form.component';
import { OrderDetailComponent } from './components/order-detail/order-detail.component';
import { ShoppingCartComponent } from './components/shopping-cart/shopping-cart.component';

// Pages
import { OrderListPageComponent } from './pages/order-list-page/order-list-page.component';
import { OrderCreatePageComponent } from './pages/order-create-page/order-create-page.component';
import { OrderEditPageComponent } from './pages/order-edit-page/order-edit-page.component';
import { OrderDetailPageComponent } from './pages/order-detail-page/order-detail-page.component';
import { ShoppingCartPageComponent } from './pages/shopping-cart-page/shopping-cart-page.component';

// Services
import { OrderService } from './services/order.service';
import { CartService } from './services/cart.service';

// Routing
import { orderRoutes } from './order.routes';

@NgModule({
  declarations: [
    // Components
    OrderListComponent,
    OrderFormComponent,
    OrderDetailComponent,
    ShoppingCartComponent,

    // Pages
    OrderListPageComponent,
    OrderCreatePageComponent,
    OrderEditPageComponent,
    OrderDetailPageComponent,
    ShoppingCartPageComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule.forChild(orderRoutes),
  ],
  providers: [],
  exports: [
    OrderListComponent,
    OrderFormComponent,
    OrderDetailComponent,
    ShoppingCartComponent,
  ],
})
export class OrderModule {}
