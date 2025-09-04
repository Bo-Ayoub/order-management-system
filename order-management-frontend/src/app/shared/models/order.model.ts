export interface Order {
  id: string;
  orderNumber: string;
  customerId: string;
  customerName: string;
  status: OrderStatus;
  orderDate: string;
  shippedDate?: string;
  deliveredDate?: string;
  shippingAddress?: string;
  notes?: string;
  totalAmount: number;
  currency: string;
  totalItems: number;
  orderItems: OrderItem[];
}

export interface OrderItem {
  id: string;
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  currency: string;
}

export interface CreateOrderRequest {
  customerId: string;
  items: CreateOrderItemRequest[];
  shippingAddress?: string;
  notes?: string;
}

export interface CreateOrderItemRequest {
  productId: string;
  quantity: number;
}

export enum OrderStatus {
  Pending = 'Pending',
  Confirmed = 'Confirmed',
  Processing = 'Processing',
  Shipped = 'Shipped',
  Delivered = 'Delivered',
  Cancelled = 'Cancelled',
}

export interface OrderSummary {
  id: string;
  orderNumber: string;
  customerName: string;
  status: OrderStatus;
  orderDate: string;
  totalAmount: number;
  currency: string;
  totalItems: number;
}
