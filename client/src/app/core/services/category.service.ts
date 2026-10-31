import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Category } from '../models/inventory.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/api/categories`;

  getAll() {
    return this.http.get<Category[]>(this.url);
  }

  create(name: string) {
    return this.http.post<{ id: number }>(this.url, { name });
  }
}
