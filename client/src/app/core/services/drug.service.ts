import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Drug, PagedResult } from '../models/drug.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class DrugService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/api/drugs`;

  getDrugs(search: string, page: number, pageSize: number) {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);

    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<PagedResult<Drug>>(this.url, { params });
  }
}
