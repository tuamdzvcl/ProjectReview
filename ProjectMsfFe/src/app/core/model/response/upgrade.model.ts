export interface UpgradeResponse {
  Id: number;
  TitleUpgrade: string;
  Description: string;
  status: string;
  DailyLimit: number;
  Price: number;
  CreatedAt: string;
  UpdatedAt: string;
  isDailyPackage: boolean;
}

export interface UpgradeRequest {
  titleUpgrade: string;
  description: string;
  status: string;
  dailyLimit: number;
  price: number;
}
