export interface RevenueSummary {
  TotalRevenue: number;
  TotalOrders: number;
  TotalTickets: number;
  TotalViews: number;
  GrowthRevenue: number;
  GrowthOrders: number;
  GrowthTickets: number;
  GrowthViews: number;
}

export interface RevenueChart {
  Time: string;
  Label: string;
  Revenue: number;
}

export interface RevenueMeta {
  FromDate: string;
  ToDate: string;
  GroupBy: string;
}

export interface ReportResponse {
  Summary: RevenueSummary;
  Chart: RevenueChart[];
  Meta: RevenueMeta;
}
