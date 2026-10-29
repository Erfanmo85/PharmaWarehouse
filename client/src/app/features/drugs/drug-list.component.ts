import { Component, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { Drug } from '../../core/models/drug.model';
import { DrugService } from '../../core/services/drug.service';

const FORM_LABELS: Record<string, string> = {
  Tablet: 'قرص',
  Capsule: 'کپسول',
  Syrup: 'شربت',
  Injection: 'آمپول',
  Ointment: 'پماد',
};

@Component({
  selector: 'app-drug-list',
  imports: [MatTableModule, MatPaginatorModule, MatFormFieldModule, MatInputModule, MatProgressBarModule],
  template: `
    <h2>لیست داروها</h2>

    <mat-form-field appearance="outline" class="search">
      <mat-label>جستجو (نام یا کد)</mat-label>
      <input matInput (input)="onSearch($any($event.target).value)" />
    </mat-form-field>

    @if (loading()) {
      <mat-progress-bar mode="indeterminate" />
    }

    @if (error()) {
      <p class="error">{{ error() }}</p>
    }

    <div class="table-wrap">
      <table mat-table [dataSource]="drugs()">
        <ng-container matColumnDef="code">
          <th mat-header-cell *matHeaderCellDef>کد</th>
          <td mat-cell *matCellDef="let d">{{ d.code }}</td>
        </ng-container>

        <ng-container matColumnDef="name">
          <th mat-header-cell *matHeaderCellDef>نام دارو</th>
          <td mat-cell *matCellDef="let d">{{ d.name }}</td>
        </ng-container>

        <ng-container matColumnDef="categoryName">
          <th mat-header-cell *matHeaderCellDef>دسته‌بندی</th>
          <td mat-cell *matCellDef="let d">{{ d.categoryName }}</td>
        </ng-container>

        <ng-container matColumnDef="dosageForm">
          <th mat-header-cell *matHeaderCellDef>شکل دارویی</th>
          <td mat-cell *matCellDef="let d">{{ label(d.dosageForm) }}</td>
        </ng-container>

        <ng-container matColumnDef="totalStock">
          <th mat-header-cell *matHeaderCellDef>موجودی</th>
          <td mat-cell *matCellDef="let d">{{ d.totalStock }}</td>
        </ng-container>

        <ng-container matColumnDef="minStockLevel">
          <th mat-header-cell *matHeaderCellDef>حداقل</th>
          <td mat-cell *matCellDef="let d">{{ d.minStockLevel }}</td>
        </ng-container>

        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>وضعیت</th>
          <td mat-cell *matCellDef="let d">
            <span class="chip" [class.low]="d.isLowStock">
              {{ d.isLowStock ? 'کمبود' : 'کافی' }}
            </span>
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="columns"></tr>
        <tr mat-row *matRowDef="let row; columns: columns"></tr>

        <tr class="mat-row" *matNoDataRow>
          <td class="empty" [attr.colspan]="columns.length">دارویی پیدا نشد.</td>
        </tr>
      </table>
    </div>

    <mat-paginator
      [length]="total()"
      [pageSize]="pageSize"
      [pageIndex]="pageIndex"
      [pageSizeOptions]="[5, 10, 25]"
      (page)="onPage($event)"
      showFirstLastButtons />
  `,
  styles: [`
    .search { width: 100%; max-width: 360px; }
    .table-wrap { overflow-x: auto; }
    table { width: 100%; }
    .empty { text-align: center; padding: 24px; }
    .error { color: #b3261e; }
    .chip { padding: 2px 10px; border-radius: 12px; background: #e6f4ea; color: #1e6e3a; font-size: 13px; }
    .chip.low { background: #fdecea; color: #b3261e; }
  `],
})
export class DrugListComponent implements OnInit {
  private readonly drugService = inject(DrugService);

  readonly columns = ['code', 'name', 'categoryName', 'dosageForm', 'totalStock', 'minStockLevel', 'status'];

  readonly drugs = signal<Drug[]>([]);
  readonly total = signal(0);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  search = '';
  pageIndex = 0;
  pageSize = 10;

  private readonly search$ = new Subject<string>();

  constructor() {
    this.search$
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(value => {
        this.search = value;
        this.pageIndex = 0;
        this.load();
      });
  }

  ngOnInit() {
    this.load();
  }

  label(form: string) {
    return FORM_LABELS[form] ?? form;
  }

  onSearch(value: string) {
    this.search$.next(value.trim());
  }

  onPage(e: PageEvent) {
    this.pageIndex = e.pageIndex;
    this.pageSize = e.pageSize;
    this.load();
  }

  load() {
    this.loading.set(true);
    this.error.set(null);

    this.drugService.getDrugs(this.search, this.pageIndex + 1, this.pageSize).subscribe({
      next: res => {
        this.drugs.set(res.items);
        this.total.set(res.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('دریافت لیست داروها با خطا مواجه شد.');
        this.loading.set(false);
      },
    });
  }
}
