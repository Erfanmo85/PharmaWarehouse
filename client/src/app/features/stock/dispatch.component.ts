import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { Drug } from '../../core/models/drug.model';
import { DispatchedItem } from '../../core/models/inventory.model';
import { DrugService } from '../../core/services/drug.service';
import { InventoryService } from '../../core/services/inventory.service';
import { NotifyService } from '../../core/services/notify.service';
import { apiErrorMessage } from '../../core/utils/api-error';

@Component({
  selector: 'app-dispatch',
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule],
  template: `
    <h2>خروج کالا</h2>
    <p class="muted">سیستم به‌صورت خودکار از بچی که زودتر منقضی می‌شود کم می‌کند (FEFO).</p>

    <form [formGroup]="form" (ngSubmit)="submit()" class="form">
      <mat-form-field appearance="outline">
        <mat-label>دارو</mat-label>
        <mat-select formControlName="drugId">
          @for (d of drugs(); track d.id) {
            <mat-option [value]="d.id">{{ d.name }} ({{ d.code }}) | موجودی: {{ d.totalStock }}</mat-option>
          }
        </mat-select>
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>تعداد</mat-label>
        <input matInput type="number" min="1" formControlName="quantity" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>توضیح (اختیاری)</mat-label>
        <input matInput formControlName="note" />
      </mat-form-field>

      <button mat-flat-button type="submit" [disabled]="form.invalid || loading()">
        {{ loading() ? 'در حال ثبت...' : 'ثبت خروج' }}
      </button>
    </form>

    @if (result().length > 0) {
      <h3>بچ‌های برداشت‌شده</h3>
      <div class="table-wrap">
        <table class="simple-table">
          <thead>
            <tr><th>شماره بچ</th><th>تاریخ انقضا</th><th>تعداد برداشت‌شده</th></tr>
          </thead>
          <tbody>
            @for (item of result(); track item.batchId) {
              <tr>
                <td>{{ item.batchNumber }}</td>
                <td>{{ item.expiryDate }}</td>
                <td>{{ item.quantity }}</td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    }
  `,
  styles: [`
    .muted { color: #666; }
    .form { display: flex; flex-direction: column; gap: 4px; max-width: 460px; }
    .table-wrap { overflow-x: auto; }
  `],
})
export class DispatchComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly drugService = inject(DrugService);
  private readonly inventory = inject(InventoryService);
  private readonly notify = inject(NotifyService);

  readonly drugs = signal<Drug[]>([]);
  readonly result = signal<DispatchedItem[]>([]);
  readonly loading = signal(false);

  readonly form = this.fb.nonNullable.group({
    drugId: [0, [Validators.required, Validators.min(1)]],
    quantity: [1, [Validators.required, Validators.min(1)]],
    note: [''],
  });

  ngOnInit() {
    this.loadDrugs();
  }

  private loadDrugs() {
    this.drugService.getDrugs('', 1, 100).subscribe({
      next: res => this.drugs.set(res.items),
      error: err => this.notify.error(apiErrorMessage(err)),
    });
  }

  submit() {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.result.set([]);

    const { drugId, quantity, note } = this.form.getRawValue();

    this.inventory.dispatch(drugId, quantity, note.trim() || null).subscribe({
      next: items => {
        this.result.set(items);
        this.notify.success('خروج کالا ثبت شد.');
        this.form.patchValue({ quantity: 1, note: '' });
        this.loading.set(false);
        this.loadDrugs();   // موجودی‌های جدید را در لیست نشان بده
      },
      error: err => {
        this.notify.error(apiErrorMessage(err));
        this.loading.set(false);
      },
    });
  }
}
