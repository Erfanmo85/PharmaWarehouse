import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenKey = 'pharma_token';
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly _token = signal<string | null>(this.loadToken());

  // ۱) این متد را داخل کلاس اضافه کن
  private isExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
      return payload.exp * 1000 < Date.now();
    } catch {
      return true;
    }
  }

  // ۲) مقدار اولیه signal را این‌طور کن

  private loadToken(): string | null {
    const saved = localStorage.getItem(this.tokenKey);
    return saved && !this.isExpired(saved) ? saved : null;
  }

  readonly isLoggedIn = computed(() => !!this._token());

  get token(): string | null {
    return this._token();
  }

  login(username: string, password: string) {
    return this.http
      .post<{ token: string }>(`${environment.apiUrl}/api/auth/login`, { username, password })
      .pipe(
        tap(res => {
          localStorage.setItem(this.tokenKey, res.token);
          this._token.set(res.token);
        })
      );
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    this._token.set(null);
    this.router.navigate(['/login']);
  }
}
