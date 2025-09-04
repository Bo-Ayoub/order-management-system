import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface Notification {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message: string;
  duration?: number;
}

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private notifications$ = new BehaviorSubject<Notification[]>([]);

  getNotifications(): Observable<Notification[]> {
    return this.notifications$.asObservable();
  }

  success(title: string, message: string, duration = 5000) {
    this.addNotification('success', title, message, duration);
  }

  error(title: string, message: string, duration = 8000) {
    this.addNotification('error', title, message, duration);
  }

  warning(title: string, message: string, duration = 6000) {
    this.addNotification('warning', title, message, duration);
  }

  info(title: string, message: string, duration = 5000) {
    this.addNotification('info', title, message, duration);
  }

  private addNotification(
    type: Notification['type'],
    title: string,
    message: string,
    duration: number
  ) {
    const notification: Notification = {
      id: Math.random().toString(36).substr(2, 9),
      type,
      title,
      message,
      duration,
    };

    const currentNotifications = this.notifications$.value;
    this.notifications$.next([...currentNotifications, notification]);

    if (duration > 0) {
      setTimeout(() => {
        this.removeNotification(notification.id);
      }, duration);
    }
  }

  removeNotification(id: string) {
    const currentNotifications = this.notifications$.value;
    const filteredNotifications = currentNotifications.filter(
      (n) => n.id !== id
    );
    this.notifications$.next(filteredNotifications);
  }

  clearAll() {
    this.notifications$.next([]);
  }
}
