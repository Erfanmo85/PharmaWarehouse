export interface Category {
  id: number;
  name: string;
}

export interface Batch {
  id: number;
  batchNumber: string;
  manufactureDate: string;
  expiryDate: string;
  quantity: number;
  purchasePrice: number;
  isExpired: boolean;
}

export interface CreateDrugRequest {
  name: string;
  code: string;
  dosageForm: string;
  minStockLevel: number;
  categoryId: number;
}

export interface CreateBatchRequest {
  drugId: number;
  batchNumber: string;
  manufactureDate: string;
  expiryDate: string;
  quantity: number;
  purchasePrice: number;
}

export interface DispatchedItem {
  batchId: number;
  batchNumber: string;
  expiryDate: string;
  quantity: number;
}

export interface LowStockItem {
  drugId: number;
  name: string;
  code: string;
  totalStock: number;
  minStockLevel: number;
}

export interface ExpiringBatch {
  batchId: number;
  drugId: number;
  drugName: string;
  batchNumber: string;
  expiryDate: string;
  quantity: number;
  daysLeft: number;
  isExpired: boolean;
}
