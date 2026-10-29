export interface Drug {
  id: number;
  name: string;
  code: string;
  dosageForm: string;
  minStockLevel: number;
  categoryId: number;
  categoryName: string;
  totalStock: number;
  isLowStock: boolean;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}
