import { Injectable } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';

@Injectable()
export class PersianPaginatorIntl extends MatPaginatorIntl {
  override itemsPerPageLabel = 'تعداد در صفحه';
  override nextPageLabel = 'صفحه بعد';
  override previousPageLabel = 'صفحه قبل';
  override firstPageLabel = 'صفحه اول';
  override lastPageLabel = 'صفحه آخر';

  override getRangeLabel = (page: number, pageSize: number, length: number) => {
    if (length === 0) return '۰ از ۰';
    const start = page * pageSize + 1;
    const end = Math.min((page + 1) * pageSize, length);
    return `${start} – ${end} از ${length}`;
  };
}
