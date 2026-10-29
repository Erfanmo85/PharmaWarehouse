import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  template: `
    <div class="wrap">
      <mat-card class="card">
        <mat-card-header>
          <mat-card-title>ورود به سامانه انبار دارو</mat-card-title>
        </mat-card-header>

        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="submit()">
            <mat-form-field appearance="outline">
              <mat-label>نام کاربری</mat-label>
              <input matInput formControlName="username" autocomplete="username" />
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>رمز عبور</mat-label>
              <input matInput type="password" formControlName="password" autocomplete="current-password" />
            </mat-form-field>

            @if (error()) {
              <p class="error">{{ error() }}</p>
            }

            <button mat-flat-button type="submit" [disabled]="form.invalid || loading()">
              {{ loading() ? 'در حال ورود...' : 'ورود' }}
            </button>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .wrap { min-height: 100vh; display: grid; place-items: center; padding: 16px; }
    .card { width: 100%; max-width: 380px; padding: 16px; }
    form { display: flex; flex-direction: column; gap: 8px; margin-top: 16px; }
    .error { color: #b3261e; margin: 0; }
  `],
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly form = this.fb.nonNullable.group({
    username: ['', Validators.required],
    password: ['', Validators.required],
  });

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  submit() {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.error.set(null);

    const { username, password } = this.form.getRawValue();

    this.auth.login(username, password).subscribe({
      next: () => this.router.navigate(['/drugs']),
      error: () => {
        this.error.set('نام کاربری یا رمز عبور اشتباه است.');
        this.loading.set(false);
      },
    });
  }
}
