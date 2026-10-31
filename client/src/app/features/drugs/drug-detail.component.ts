import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { dosageLabel } from '../../core/models/dosage-forms';
import { Drug } from '../../core/models/drug.model';
import { Batch } from '../../core/models/inventory.model';
import { DrugService } from '../../core/services/drug.service';
import { InventoryService } from '../../core/services/inventory.service';
import { NotifyService } from '../../core/services/notify.service';
import { apiErrorMessage } from '../../core/utils/api-error';

@Component({
  selector: 'app-drug-detail',
  imports: [ReactiveFormsModule, RouterLink, MatButtonModule, MatFormFieldModule, MatInputModule],
  template: `
    <a mat-button routerLink="/drugs">بازگشت به لیست</a>

    @if (drug(); as d) {
      <div class="head">
        <div>
          <h2>{{ d.name }}</h2>
          <span class="muted">کد: {{ d.code }}</span>
        </div>
        <span class="chip" [class.danger]="d.isLowStock">
          {{ d.isLowStock ? 'کمبود موجودی' : 'موجودی کافی' }}
        </span>
      </div>

      <div class="info">
        <div><span class="muted">دسته‌بندی</span><b>{{ d.categoryName }}</b></div>
        <div><span class="muted">شکل دارویی</span><b>{{ label(d.dosageForm) }}</b></div>
        <div><span class="muted">موجودی کل</span><b>{{ d.totalStock }}</b></div>
        <div><span class="muted">حداقل موجودی</span><b>{{ d.minStockLevel }}</b></div>
      </div>

      <div class="head">
        <h3>بچ‌ها (مرتب‌شده بر اساس نزدیک‌ترین انقضا)</h3>
        <button mat-stroked-button type="button" (click)="showForm.set(!showForm())">
          {{ showForm() ? 'بستن فرم' : 'ثبت بچ جدید' }}
        </button>
      </div>

      @if (showForm()) {
        <form [formGroup]="form" (ngSubmit)="createBatch()" class="batch-form">
          <mat-form-field appearance="outline">
            <mat-label>شماره بچ</mat-label>
            <input matInput formControlName="batchNumber" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>تاریخ تولید</mat-label>
            <input matInput type="date" formControlName="manufactureDate" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>تاریخ انقضا</mat-label>
            <input matInput type="date" formControlName="expiryDate" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>تعداد</mat-label>
            <input matInput type="number" min="1" formControlName="quantity" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>قیمت خرید (واحد)</mat-label>
            <input matInput type="number" min="0" formControlName="purchasePrice" />
          </mat-form-field>
          <button mat-flat-button type="submit" [disabled]="form.invalid || saving()">ثبت بچ</button>
        </form>
      }

      @if (batches().length === 0) {
        <p class="muted">هنوز بچی برای این دارو ثبت نشده است.</p>
      } @else {
        <div class="table-wrap">
          <table class="simple-table">
            <thead>
              <tr>
                <th>شماره بچ</th><th>تاریخ تولید</th><th>تاریخ انقضا</th>
                <th>تعداد</th><th>قیمت خرید</th><th>وضعیت</th><th>عملیات</th>
              </tr>
            </thead>
            <tbody>
              @for (b of batches(); track b.id) {
                <tr>
                  <td>{{ b.batchNumber }}</td>
                  <td>{{ b.manufactureDate }}</td>
                  <td>{{ b.expiryDate }}</td>
                  <td>{{ b.quantity }}</td>
                  <td>{{ b.purchasePrice }}</td>
                  <td>
                    <span class="chip" [class.danger]="b.isExpired">{{ b.isExpired ? 'منقضی' : 'معتبر' }}</span>
                  </td>
                  <td class="actions">
                    @if (!b.isExpired) {
                      <input #qty type="number" min="1" placeholder="تعداد" class="qty" />
                      <button mat-button type="button" (click)="addStock(b.id, +qty.value); qty.value = ''">افزودن</button>
                    }
                    @if (b.quantity > 0) {
                      <button mat-button type="button" (click)="writeOff(b)">ضایعات</button>
                    }
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }
    }
  `,
  styles: [`
    .head { display: flex; align-items: center; justify-content: space-between; flex-wrap: wrap; gap: 12px; margin-top: 8px; }
    .muted { color: #666; font-size: 13px; }
    .info { display: grid; grid-template-columns: repeat(auto-fit, minmax(160px, 1fr)); gap: 12px; margin: 16px 0; }
    .info div { display: flex; flex-direction: column; gap: 2px; }
    .batch-form { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 8px 16px; align-items: start; margin: 12px 0; }
    .table-wrap { overflow-x: auto; }
    .actions { white-space: nowrap; }
    .qty { width: 70px; padding: 4px; margin-inline-end: 4px; }
  `],
})
export class DrugDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  private readonly drugService = inject(DrugService);
  private readonly inventory = inject(InventoryService);
  private readonly notify = inject(NotifyService);

  readonly drugId = Number(this.route.snapshot.paramMap.get('id'));
  readonly label = dosageLabel;

  readonly drug = signal<Drug | null>(null);
  readonly batches = signal<Batch[]>([]);
  readonly showForm = signal(false);
  readonly saving = signal(false);

  readonly form = this.fb.nonNullable.group({
    batchNumber: ['', [Validators.required, Validators.maxLength(50)]],
    manufactureDate: ['', Validators.required],
    expiryDate: ['', Validators.required],
    quantity: [1, [Validators.required, Validators.min(1)]],
    purchasePrice: [0, [Validators.required, Validators.min(0)]],
  });

  ngOnInit() {
    this.reload();
  }

  reload() {
    forkJoin({
      drug: this.drugService.getById(this.drugId),
      batches: this.inventory.getBatches(this.drugId),
    }).subscribe({
      next: ({ drug, batches }) => {
        this.drug.set(drug);
        this.batches.set(batches);
      },
      error: err => this.notify.error(apiErrorMessage(err)),
    });
  }

  createBatch() {
    if (this.form.invalid) return;
    this.saving.set(true);

    this.inventory.createBatch({ drugId: this.drugId, ...this.form.getRawValue() }).subscribe({
      next: () => {
        this.notify.success('بچ ثبت شد.');
        this.form.reset({ quantity: 1, purchasePrice: 0 });
        this.showForm.set(false);
        this.saving.set(false);
        this.reload();
      },
      error: err => {
        this.notify.error(apiErrorMessage(err));
        this.saving.set(false);
      },
    });
  }

  addStock(batchId: number, quantity: number) {
    if (!quantity || quantity <= 0) {
      this.notify.error('تعداد را درست وارد کن.');
      return;
    }

    this.inventory.addStock(batchId, quantity).subscribe({
      next: () => {
        this.notify.success('موجودی اضافه شد.');
        this.reload();
      },
      error: err => this.notify.error(apiErrorMessage(err)),
    });
  }

  writeOff(batch: Batch) {
    if (!confirm(`کل موجودی بچ ${batch.batchNumber} (${batch.quantity} عدد) به عنوان ضایعات ثبت شود؟`)) {
      return;
    }

    this.inventory.writeOff(batch.id).subscribe({
      next: res => {
        this.notify.success(`${res.writtenOff} عدد به عنوان ضایعات ثبت شد.`);
        this.reload();
      },
      error: err => this.notify.error(apiErrorMessage(err)),
    });
  }
}
