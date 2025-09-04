export interface Customer {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  createdAt: string;
}

export interface CreateCustomerRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
}

export interface CustomerSummary {
  id: string;
  fullName: string;
  email: string;
  totalOrders: number;
}
