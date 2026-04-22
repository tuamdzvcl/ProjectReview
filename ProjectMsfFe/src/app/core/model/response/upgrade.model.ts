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
  TitleUpgrade: string;
  Description: string;
  status: string;
  DailyLimit: number;
  Price: number;
}
