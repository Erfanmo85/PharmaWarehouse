import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { environment } from '../../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.token;

  // فقط برای درخواست‌های API خودمان توکن بچسبان
  const request =
    token && req.url.startsWith(environment.apiUrl)
      ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
      : req;

  return next(request).pipe(
    catchError((err: HttpErrorResponse) => {
      // توکن منقضی یا نامعتبر شده: خارج شو
      if (err.status === 401 && !req.url.endsWith('/api/auth/login')) {
        auth.logout();
      }
      return throwError(() => err);
    })
  );
};
