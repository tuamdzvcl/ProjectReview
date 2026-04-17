export interface UpgradeResponse {
  id: number;
  titleUpgrade: string;
  description: string;
  status: string;
  dailyLimit: number;
  price: number;
  createdAt: string;
  updatedAt: string;
  isDailyPackage: boolean;
}

export interface UpgradeRequest {
  titleUpgrade: string;
  description: string;
  status: string;
  dailyLimit: number;
  price: number;
}
