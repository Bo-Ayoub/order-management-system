import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface Notification {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message: string;
  duration?: number;
  timestamp: Date;
}

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private notificationsSubject = new BehaviorSubject<Notification[]>([]);
  public notifications$: Observable<Notification[]> =
    this.notificationsSubject.asObservable();

  constructor() {}

  showSuccess(message: string, title: string = 'Success'): void {
    this.addNotification({
      id: this.generateId(),
      type: 'success',
      title,
      message,
      duration: 5000,
      timestamp: new Date(),
    });
  }

  showError(message: string, title: string = 'Error'): void {
    this.addNotification({
      id: this.generateId(),
      type: 'error',
      title,
      message,
      duration: 7000,
      timestamp: new Date(),
    });
  }

  showWarning(message: string, title: string = 'Warning'): void {
    this.addNotification({
      id: this.generateId(),
      type: 'warning',
      title,
      message,
      duration: 6000,
      timestamp: new Date(),
    });
  }

  showInfo(message: string, title: string = 'Info'): void {
    this.addNotification({
      id: this.generateId(),
      type: 'info',
      title,
      message,
      duration: 5000,
      timestamp: new Date(),
    });
  }

  removeNotification(id: string): void {
    const currentNotifications = this.notificationsSubject.value;
    const updatedNotifications = currentNotifications.filter(
      (n) => n.id !== id
    );
    this.notificationsSubject.next(updatedNotifications);
  }

  clearAllNotifications(): void {
    this.notificationsSubject.next([]);
  }

  private addNotification(notification: Notification): void {
    const currentNotifications = this.notificationsSubject.value;
    const updatedNotifications = [...currentNotifications, notification];
    this.notificationsSubject.next(updatedNotifications);

    // Auto-remove notification after duration
    if (notification.duration) {
      setTimeout(() => {
        this.removeNotification(notification.id);
      }, notification.duration);
    }
  }

  private generateId(): string {
    return Math.random().toString(36).substr(2, 9);
  }
}
