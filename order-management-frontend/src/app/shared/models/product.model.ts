export interface Product {
  id: string;
  name: string;
  description?: string;
  price: number;
  currency: string;
  stockQuantity: number;
  category?: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateProductRequest {
  name: string;
  description?: string;
  price: number;
  currency: string;
  stockQuantity: number;
  category?: string;
}

export interface ProductSummary {
  id: string;
  name: string;
  price: number;
  currency: string;
  stockQuantity: number;
  isActive: boolean;
}
