# Order Management Frontend

A comprehensive Angular-based frontend application for managing orders, products, and customers in an e-commerce system. This application provides a modern, responsive interface for order management operations with real-time cart functionality and seamless backend integration.

## 🚀 Features

### 📦 Product Management

- **Product Catalog**: Browse and manage product inventory
- **Product CRUD Operations**: Create, read, update, and delete products
- **Stock Management**: Real-time stock quantity tracking
- **Product Categories**: Organize products by categories
- **Product Search & Filtering**: Advanced search and filtering capabilities
- **Product Status Management**: Active/inactive product states

### 👥 Customer Management

- **Customer Database**: Complete customer information management
- **Customer CRUD Operations**: Full lifecycle customer management
- **Customer Profiles**: Detailed customer information and order history
- **Customer Search**: Find customers quickly with search functionality

### 🛒 Order Management

- **Shopping Cart**: Advanced cart functionality with persistent storage
- **Order Creation**: Create orders with customer selection and product management
- **Order Tracking**: Track order status through various stages (Pending, Confirmed, Processing, Shipped, Delivered, Cancelled)
- **Order History**: Complete order history and details
- **Order Status Updates**: Real-time order status management
- **Order Validation**: Comprehensive validation before checkout

### 🛍️ Shopping Cart Features

- **Persistent Cart**: Cart data persists across browser sessions using localStorage
- **Real-time Stock Validation**: Automatic stock availability checking
- **Quantity Management**: Add, update, and remove items with quantity controls
- **Price Calculations**: Automatic subtotal and total calculations
- **Cart Validation**: Pre-checkout validation for stock availability and product status
- **Cart Refresh**: Automatic cart refresh with updated product information

### 📊 Dashboard & Analytics

- **Overview Dashboard**: Centralized view of key metrics and statistics
- **Order Analytics**: Visual representation of order data using Chart.js
- **Real-time Updates**: Live data updates and notifications

### 🎨 User Interface

- **Responsive Design**: Mobile-first responsive design using Tailwind CSS
- **Modern UI Components**: Clean, intuitive interface with Angular Material components
- **Loading States**: Smooth loading indicators and user feedback
- **Error Handling**: Comprehensive error handling with user-friendly messages
- **Notifications**: Toast notifications for user actions and system feedback

### 🔧 Technical Features

- **Angular 18**: Latest Angular framework with standalone components
- **State Management**: NgRx for predictable state management
- **HTTP Interceptors**: Authentication, error handling, and loading interceptors
- **Lazy Loading**: Optimized performance with lazy-loaded feature modules
- **TypeScript**: Full type safety and enhanced development experience
- **Server-Side Rendering (SSR)**: Enhanced SEO and performance

## 🛠️ Technology Stack

- **Frontend Framework**: Angular 18.2.0
- **UI Library**: Angular Material & Tailwind CSS
- **State Management**: NgRx Store & Effects
- **Charts**: Chart.js for data visualization
- **Icons**: Lucide Angular
- **Date Handling**: date-fns
- **Build Tool**: Angular CLI
- **Package Manager**: npm

## 🚀 Getting Started

### Prerequisites

- Node.js (version 18 or higher)
- npm (version 9 or higher)
- Angular CLI (version 18.2.20)

### Installation

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd order-management-frontend
   ```

2. **Install dependencies**

   ```bash
   npm install
   ```

3. **Start the development server**
   ```bash
   npm start
   # or
   ng serve
   ```

### ⚠️ Important: Port Configuration

**The application MUST run on port 4200** as the backend CORS configuration only allows requests from `http://localhost:4200`. Any other port will result in CORS errors and prevent the application from communicating with the backend API.

The application will be available at: `http://localhost:4200/`

## 📁 Project Structure

```
src/
├── app/
│   ├── core/                    # Core functionality
│   │   ├── guards/             # Route guards
│   │   ├── interceptors/       # HTTP interceptors
│   │   └── services/           # Core services
│   ├── features/               # Feature modules
│   │   ├── customers/          # Customer management
│   │   ├── products/           # Product management
│   │   ├── orders/             # Order management
│   │   └── dashboard/          # Dashboard
│   ├── layout/                 # Layout components
│   │   ├── header/
│   │   ├── sidebar/
│   │   └── footer/
│   └── shared/                 # Shared components and utilities
│       ├── components/         # Reusable components
│       ├── models/            # TypeScript interfaces
│       ├── pipes/             # Custom pipes
│       └── services/          # Shared services
```

## 🔧 Development Commands

### Development Server

```bash
ng serve
# Runs on http://localhost:4200 (MANDATORY PORT)
```

### Build

```bash
ng build
# Production build artifacts in dist/
```

### Build with Watch Mode

```bash
ng build --watch --configuration development
```

### Testing

```bash
ng test
# Unit tests via Karma
```

### Code Generation

```bash
ng generate component component-name
ng generate service service-name
ng generate module module-name
```

## 🌐 API Integration

The frontend integrates with a backend API that requires CORS configuration for `http://localhost:4200`. Ensure your backend is configured to allow requests from this specific origin.

## 📱 Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

**Note**: This application is designed to work specifically with the Hahn Order Management Backend API and requires the backend to be running for full functionality.
