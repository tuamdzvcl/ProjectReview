export interface PromotionResponse {
  Id: number;
  Description: string;
  Code: string;
  DiscountAmount: number | null;
  AmountLimit : number | null;
  DiscountValue: number;
  DiscountType: string;
  StartDate: string;
  EndDate: string;
  IsActive: boolean;
  UsageLimit: number | null;
  UsedCount: number;
  CreatedAt?: string;
  UpdatedAt?: string;
}

export interface PromotionRequest {
  Code: string;
  DiscountAmount: number | null;
  DiscountValue: number;
  AmountLimit:number |null;
  DiscountType: string;
  StartDate: string;
  EndDate: string;
  IsActive: boolean;
  UsageLimit: number | null;
}
