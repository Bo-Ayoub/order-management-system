import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Components
import { CustomerListComponent } from './components/customer-list/customer-list.component';
import { CustomerFormComponent } from './components/customer-form/customer-form.component';
import { CustomerDetailComponent } from './components/customer-detail/customer-detail.component';

// Pages
import { CustomerListPageComponent } from './pages/customer-list-page/customer-list-page.component';
import { CustomerCreatePageComponent } from './pages/customer-create-page/customer-create-page.component';
import { CustomerEditPageComponent } from './pages/customer-edit-page/customer-edit-page.component';
import { CustomerDetailPageComponent } from './pages/customer-detail-page/customer-detail-page.component';

// Services
import { CustomerService } from './services/customer.service';

// Routing
import { customerRoutes } from './customer.routes';

@NgModule({
  declarations: [
    // Components
    CustomerListComponent,
    CustomerFormComponent,
    CustomerDetailComponent,

    // Pages
    CustomerListPageComponent,
    CustomerCreatePageComponent,
    CustomerEditPageComponent,
    CustomerDetailPageComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule.forChild(customerRoutes),
  ],
  providers: [],
  exports: [
    CustomerListComponent,
    CustomerFormComponent,
    CustomerDetailComponent,
  ],
})
export class CustomerModule {}
