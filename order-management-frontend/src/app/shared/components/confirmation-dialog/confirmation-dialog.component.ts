import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface ConfirmationDialogData {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  type?: 'warning' | 'danger' | 'info' | 'success';
}

@Component({
  selector: 'app-confirmation-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirmation-dialog.component.html',
  styleUrls: ['./confirmation-dialog.component.scss']
})
export class ConfirmationDialogComponent implements OnInit {
  @Input() isOpen = false;
  @Input() data: ConfirmationDialogData = {
    title: 'Confirm Action',
    message: 'Are you sure you want to proceed?',
    confirmText: 'Confirm',
    cancelText: 'Cancel',
    type: 'warning'
  };
  
  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();
  @Output() close = new EventEmitter<void>();

  constructor() {}

  ngOnInit(): void {}

  onConfirm(): void {
    this.confirm.emit();
    this.closeDialog();
  }

  onCancel(): void {
    this.cancel.emit();
    this.closeDialog();
  }

  onClose(): void {
    this.close.emit();
    this.closeDialog();
  }

  onBackdropClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.onClose();
    }
  }

  private closeDialog(): void {
    this.isOpen = false;
  }

  getIconPath(): string {
    const icons = {
      warning: 'M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z',
      danger: 'M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
      info: 'M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
      success: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z'
    };
    return icons[this.data.type || 'warning'];
  }

  getIconClasses(): string {
    const classes = {
      warning: 'text-yellow-600 bg-yellow-100',
      danger: 'text-red-600 bg-red-100',
      info: 'text-blue-600 bg-blue-100',
      success: 'text-green-600 bg-green-100'
    };
    return classes[this.data.type || 'warning'];
  }

  getButtonClasses(): string {
    const classes = {
      warning: 'bg-yellow-600 hover:bg-yellow-700 focus:ring-yellow-500',
      danger: 'bg-red-600 hover:bg-red-700 focus:ring-red-500',
      info: 'bg-blue-600 hover:bg-blue-700 focus:ring-blue-500',
      success: 'bg-green-600 hover:bg-green-700 focus:ring-green-500'
    };
    return classes[this.data.type || 'warning'];
  }
}
