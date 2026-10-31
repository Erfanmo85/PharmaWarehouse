import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { DOSAGE_FORMS } from '../../core/models/dosage-forms';
import { Category } from '../../core/models/inventory.model';
import { CategoryService } from '../../core/services/category.service';
import { DrugService } from '../../core/services/drug.service';
import { NotifyService } from '../../core/services/notify.service';
import { apiErrorMessage } from '../../core/utils/api-error';

@Component({
  selector: 'app-drug-form',
  imports: [ReactiveFormsModule, RouterLink, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule],
  template: `
    <h2>ثبت داروی جدید</h2>

    <form [formGroup]="form" (ngSubmit)="submit()" class="form">
      <mat-form-field appearance="outline">
        <mat-label>نام دارو</mat-label>
        <input matInput formControlName="name" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>کد دارو</mat-label>
        <input matInput formControlName="code" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>شکل دارویی</mat-label>
        <mat-select formControlName="dosageForm">
          @for (f of forms; track f.value) {
            <mat-option [value]="f.value">{{ f.label }}</mat-option>
          }
        </mat-select>
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>حداقل موجودی</mat-label>
        <input matInput type="number" min="0" formControlName="minStockLevel" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>دسته‌بندی</mat-label>
        <mat-select formControlName="categoryId">
          @for (c of categories(); track c.id) {
            <mat-option [value]="c.id">{{ c.name }}</mat-option>
          }
        </mat-select>
      </mat-form-field>

      <div class="category-add">
        <mat-form-field appearance="outline" class="grow">
          <mat-label>دسته‌بندی جدید (اختیاری)</mat-label>
          <input matInput [formControl]="newCategory" />
        </mat-form-field>
        <button mat-stroked-button type="button" (click)="addCategory()">افزودن دسته‌بندی</button>
      </div>

      <div class="actions">
        <button mat-flat-button type="submit" [disabled]="form.invalid || saving()">ثبت دارو</button>
        <a mat-button routerLink="/drugs">انصراف</a>
      </div>
    </form>
  `,
  styles: [`
    .form { display: flex; flex-direction: column; gap: 4px; max-width: 460px; }
    .category-add { display: flex; gap: 12px; align-items: flex-start; }
    .grow { flex: 1; }
    .actions { display: flex; gap: 12px; margin-top: 8px; }
  `],
})
export class DrugFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly drugService = inject(DrugService);
  private readonly categoryService = inject(CategoryService);
  private readonly notify = inject(NotifyService);
  private readonly router = inject(Router);

  readonly forms = DOSAGE_FORMS;
  readonly categories = signal<Category[]>([]);
  readonly saving = signal(false);

  readonly newCategory = new FormControl('', { nonNullable: true });

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    code: ['', [Validators.required, Validators.maxLength(50)]],
    dosageForm: ['Tablet', Validators.required],
    minStockLevel: [0, [Validators.required, Validators.min(0)]],
    categoryId: [0, [Validators.required, Validators.min(1)]],
  });

  ngOnInit() {
    this.loadCategories();
  }

  private loadCategories(selectId?: number) {
    this.categoryService.getAll().subscribe({
      next: list => {
        this.categories.set(list);
        if (selectId) this.form.patchValue({ categoryId: selectId });
      },
      error: err => this.notify.error(apiErrorMessage(err)),
    });
  }

  addCategory() {
    const name = this.newCategory.value.trim();
    if (!name) return;

    this.categoryService.create(name).subscribe({
      next: res => {
        this.newCategory.reset();
        this.notify.success('دسته‌بندی اضافه شد.');
        this.loadCategories(res.id);
      },
      error: err => this.notify.error(apiErrorMessage(err)),
    });
  }

  submit() {
    if (this.form.invalid) return;
    this.saving.set(true);

    this.drugService.create(this.form.getRawValue()).subscribe({
      next: res => {
        this.notify.success('دارو ثبت شد.');
        this.router.navigate(['/drugs', res.id]);
      },
      error: err => {
        this.notify.error(apiErrorMessage(err));
        this.saving.set(false);
      },
    });
  }
}
