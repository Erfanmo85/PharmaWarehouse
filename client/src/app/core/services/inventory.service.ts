import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  Batch, CreateBatchRequest, DispatchedItem, ExpiringBatch, LowStockItem,
} from '../models/inventory.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class InventoryService {
  private readonly http = inject(HttpClient);
  private readonly api = environment.apiUrl;

  getBatches(drugId: number) {
    return this.http.get<Batch[]>(`${this.api}/api/drugs/${drugId}/batches`);
  }

  createBatch(body: CreateBatchRequest) {
    return this.http.post<{ id: number }>(`${this.api}/api/batches`, body);
  }

  dispatch(drugId: number, quantity: number, note: string | null) {
    return this.http.post<DispatchedItem[]>(`${this.api}/api/stock/dispatch`, { drugId, quantity, note });
  }

  addStock(batchId: number, quantity: number) {
    return this.http.post<{ quantity: number }>(
      `${this.api}/api/stock/batches/${batchId}/add`, { quantity, note: null });
  }

  writeOff(batchId: number) {
    return this.http.post<{ writtenOff: number }>(
      `${this.api}/api/stock/batches/${batchId}/write-off`, { note: null });
  }

  lowStock() {
    return this.http.get<LowStockItem[]>(`${this.api}/api/reports/low-stock`);
  }

  expiring(days: number) {
    return this.http.get<ExpiringBatch[]>(`${this.api}/api/reports/expiring`, { params: { days } });
  }
}
