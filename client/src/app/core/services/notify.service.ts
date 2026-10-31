import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class NotifyService {
  private readonly snack = inject(MatSnackBar);

  success(message: string) {
    this.snack.open(message, 'بستن', { duration: 3000 });
  }

  error(message: string) {
    this.snack.open(message, 'بستن', { duration: 6000 });
  }
}
