import { HttpErrorResponse } from '@angular/common/http';

export function apiErrorMessage(err: unknown): string {
  if (err instanceof HttpErrorResponse) {
    if (err.status === 0) {
      return 'ارتباط با سرور برقرار نشد.';
    }

    const body = err.error;

    if (body?.errors) {
      const messages = Object.values(body.errors as Record<string, string[]>).flat();
      if (messages.length) return messages.join(' | ');
    }

    if (body?.title) return body.title;

    console.error('API error:', err);
    return `خطای پیش‌بینی‌نشده رخ داد (کد ${err.status}).`;
  }

  console.error('Unexpected error:', err);
  return 'خطای پیش‌بینی‌نشده رخ داد.';
}
