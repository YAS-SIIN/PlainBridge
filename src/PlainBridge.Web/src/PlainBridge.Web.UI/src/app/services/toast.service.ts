import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private defaultConfig: MatSnackBarConfig = {
    duration: 4000,
    horizontalPosition: 'right',
    verticalPosition: 'top'
  };

  constructor(private snackBar: MatSnackBar) {}

  success(message: string, action?: string): void {
    this.snackBar.open(message, action || 'Close', {
      ...this.defaultConfig,
      panelClass: ['toast-success']
    });
  }

  error(message: string, action?: string): void {
    this.snackBar.open(message, action || 'Close', {
      ...this.defaultConfig,
      duration: 6000, // Keep error messages longer
      panelClass: ['toast-error']
    });
  }

  warning(message: string, action?: string): void {
    this.snackBar.open(message, action || 'Close', {
      ...this.defaultConfig,
      panelClass: ['toast-warning']
    });
  }

  info(message: string, action?: string): void {
    this.snackBar.open(message, action || 'Close', {
      ...this.defaultConfig,
      panelClass: ['toast-info']
    });
  }
}
