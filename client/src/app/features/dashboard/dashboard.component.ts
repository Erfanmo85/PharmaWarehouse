import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { ExpiringBatch, LowStockItem } from '../../core/models/inventory.model';
import { InventoryService } from '../../core/services/inventory.service';
import { NotifyService } from '../../core/services/notify.service';
import { apiErrorMessage } from '../../core/utils/api-error';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, MatCardModule, MatFormFieldModule, MatSelectModule, MatProgressBarModule],
  template: `
    <div class="head">
      <h2>داشبورد</h2>
      <mat-form-field appearance="outline" class="days">
        <mat-label>بازه بررسی انقضا</mat-label>
        <mat-select [value]="days()" (selectionChange)="changeDays($event.value)">
          @for (d of dayOptions; track d) {
            <mat-option [value]="d">{{ d }} روز آینده</mat-option>
          }
        </mat-select>
      </mat-form-field>
    </div>

    @if (loading()) {
      <mat-progress-bar mode="indeterminate" />
    }

    <div class="cards">
      <mat-card class="stat" [class.bad]="lowStock().length > 0">
        <div class="num">{{ lowStock().length }}</div>
        <div>داروی کم‌موجودی</div>
      </mat-card>
      <mat-card class="stat" [class.warn]="soonCount() > 0">
        <div class="num">{{ soonCount() }}</div>
        <div>بچ نزدیک به انقضا</div>
      </mat-card>
      <mat-card class="stat" [class.bad]="expiredCount() > 0">
        <div class="num">{{ expiredCount() }}</div>
        <div>بچ منقضی‌شده (نیازمند ضایعات)</div>
      </mat-card>
    </div>

    <h3>داروهای کم‌موجودی</h3>
    @if (lowStock().length === 0) {
      <p class="empty">همه داروها موجودی کافی دارند.</p>
    } @else {
      <div class="table-wrap">
        <table class="simple-table">
          <thead>
            <tr><th>دارو</th><th>کد</th><th>موجودی</th><th>حداقل</th></tr>
          </thead>
          <tbody>
            @for (item of lowStock(); track item.drugId) {
              <tr>
                <td><a [routerLink]="['/drugs', item.drugId]">{{ item.name }}</a></td>
                <td>{{ item.code }}</td>
                <td>{{ item.totalStock }}</td>
                <td>{{ item.minStockLevel }}</td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    }

    <h3>بچ‌های نزدیک به انقضا یا منقضی‌شده</h3>
    @if (expiring().length === 0) {
      <p class="empty">بچ نزدیک به انقضایی وجود ندارد.</p>
    } @else {
      <div class="table-wrap">
        <table class="simple-table">
          <thead>
            <tr><th>دارو</th><th>شماره بچ</th><th>تاریخ انقضا</th><th>باقی‌مانده</th><th>تعداد</th><th>وضعیت</th></tr>
          </thead>
          <tbody>
            @for (b of expiring(); track b.batchId) {
              <tr>
                <td><a [routerLink]="['/drugs', b.drugId]">{{ b.drugName }}</a></td>
                <td>{{ b.batchNumber }}</td>
                <td>{{ b.expiryDate }}</td>
                <td>{{ b.isExpired ? '-' : b.daysLeft + ' روز' }}</td>
                <td>{{ b.quantity }}</td>
                <td>
                  <span class="chip" [class.danger]="b.isExpired" [class.warn]="!b.isExpired">
                    {{ b.isExpired ? 'منقضی' : 'نزدیک انقضا' }}
                  </span>
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    }
  `,
  styles: [`
    .head { display: flex; align-items: center; justify-content: space-between; flex-wrap: wrap; gap: 12px; }
    .days { width: 220px; }
    .cards { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 16px; margin: 16px 0 24px; }
    .stat { padding: 16px; text-align: center; }
    .stat.bad { background: #fdecea; }
    .stat.warn { background: #fff4e0; }
    .num { font-size: 32px; font-weight: 700; }
    .empty { color: #666; }
    .table-wrap { overflow-x: auto; }
  `],
})
export class DashboardComponent implements OnInit {
  private readonly inventory = inject(InventoryService);
  private readonly notify = inject(NotifyService);

  readonly dayOptions = [30, 60, 90, 180];

  readonly lowStock = signal<LowStockItem[]>([]);
  readonly expiring = signal<ExpiringBatch[]>([]);
  readonly loading = signal(false);
  readonly days = signal(30);

  readonly expiredCount = computed(() => this.expiring().filter(b => b.isExpired).length);
  readonly soonCount = computed(() => this.expiring().filter(b => !b.isExpired).length);

  ngOnInit() {
    this.load();
  }

  changeDays(value: number) {
    this.days.set(value);
    this.load();
  }

  load() {
    this.loading.set(true);

    forkJoin({
      low: this.inventory.lowStock(),
      exp: this.inventory.expiring(this.days()),
    }).subscribe({
      next: ({ low, exp }) => {
        this.lowStock.set(low);
        this.expiring.set(exp);
        this.loading.set(false);
      },
      error: err => {
        this.notify.error(apiErrorMessage(err));
        this.loading.set(false);
      },
    });
  }
}
