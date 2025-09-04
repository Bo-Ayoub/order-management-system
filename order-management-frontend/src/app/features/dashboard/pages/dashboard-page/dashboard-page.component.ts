import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard-page.component.html',
  styleUrls: ['./dashboard-page.component.scss']
})
export class DashboardPageComponent {
  stats = [
    {
      title: 'Total Customers',
      value: '1,234',
      change: '+12%',
      changeType: 'positive',
      icon: 'users',
      color: 'blue'
    },
    {
      title: 'Total Products',
      value: '567',
      change: '+8%',
      changeType: 'positive',
      icon: 'package',
      color: 'green'
    },
    {
      title: 'Total Orders',
      value: '2,890',
      change: '+15%',
      changeType: 'positive',
      icon: 'shopping-cart',
      color: 'purple'
    },
    {
      title: 'Revenue',
      value: '$45,678',
      change: '+23%',
      changeType: 'positive',
      icon: 'dollar-sign',
      color: 'yellow'
    }
  ];

  recentActivities = [
    { action: 'New customer registered', user: 'John Doe', time: '2 minutes ago' },
    { action: 'Product updated', user: 'Jane Smith', time: '5 minutes ago' },
    { action: 'Order completed', user: 'Mike Johnson', time: '10 minutes ago' },
    { action: 'Customer updated', user: 'Sarah Wilson', time: '15 minutes ago' }
  ];

  getIconPath(icon: string): string {
    const icons: { [key: string]: string } = {
      users: 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z',
      package: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4',
      'shopping-cart': 'M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 13m0 0l-2.5 5M7 13l2.5 5m6-5v6a2 2 0 11-4 0v-6m4 0V9a2 2 0 00-4 0v4.01',
      'dollar-sign': 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1'
    };
    return icons[icon] || icons['users'];
  }
}
