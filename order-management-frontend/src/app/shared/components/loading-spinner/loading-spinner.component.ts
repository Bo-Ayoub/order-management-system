import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loading-spinner.component.html',
  styleUrls: ['./loading-spinner.component.scss']
})
export class LoadingSpinnerComponent {
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  @Input() color: 'primary' | 'secondary' | 'white' = 'primary';
  @Input() overlay: boolean = false;
  @Input() message: string = 'Loading...';

  getSizeClasses(): string {
    const sizeMap = {
      small: 'h-4 w-4',
      medium: 'h-8 w-8',
      large: 'h-12 w-12'
    };
    return sizeMap[this.size];
  }

  getColorClasses(): string {
    const colorMap = {
      primary: 'border-blue-600',
      secondary: 'border-gray-600',
      white: 'border-white'
    };
    return colorMap[this.color];
  }

  getTextColorClasses(): string {
    const colorMap = {
      primary: 'text-blue-600',
      secondary: 'text-gray-600',
      white: 'text-white'
    };
    return colorMap[this.color];
  }
}