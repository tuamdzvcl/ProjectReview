import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChartModule } from 'primeng/chart';
import { DatePicker } from 'primeng/datepicker';
import { ReportService } from '../../../../core/services/report.service';
import { ReportResponse } from '../../../../core/model/response/report.model';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { TokenService } from '../../../../core/services/token.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ChartModule, DatePicker, FormsModule, VndCurrencyPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent implements OnInit {
  private reportService = inject(ReportService);
  private tokenService = inject(TokenService);
  private cdr = inject(ChangeDetectorRef);

  chartData: any;
  chartOptions: any;
  selectedTab: string = 'Monthly';
  selectedDataType: string = 'Revenue'; // 'Revenue', 'Tickets', 'PlatformRevenue', 'Packages'
  isAdmin: boolean = false;
  private cachedChartList: any[] = [];

  // Summary data from API (Defaults to 0 to prevent "NaN" or layout break)
  totalRevenue: number = 0;
  totalOrders: number = 0;
  totalTickets: number = 0;
  totalViews: number = 0;
  growthRevenue: number = 0;
  growthOrders: number = 0;
  growthTickets: number = 0;
  growthViews: number = 0;

  // Date range for picker
  rangeDates: Date[] | undefined;

  ngOnInit() {
    this.isAdmin = this.tokenService.getRole()?.toUpperCase() === 'ADMIN';
    if (this.isAdmin) {
      this.selectedDataType = 'PlatformRevenue';
    }
    this.initChartOptions();

    // Mặc định chọn tháng hiện tại
    const now = new Date();
    const firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
    const lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0);
    this.rangeDates = [firstDay, lastDay];

    this.fetchRevenueStats('Monthly');
  }

  onDateChange() {
    // Tự động tải lại dữ liệu khi chọn khoảng ngày mới
    if (this.rangeDates && this.rangeDates[0] && this.rangeDates[1]) {
      this.fetchRevenueStats(this.selectedTab);
    }
  }

  onDataTypeChange() {
    this.fetchRevenueStats(this.selectedTab);
  }

  fetchRevenueStats(type: string) {
    this.selectedTab = type;
    const groupBy = type.toLowerCase();

    let fromDate: string | undefined;
    let toDate: string | undefined;

    if (this.rangeDates && this.rangeDates[0] && this.rangeDates[1]) {
      fromDate = this.formatDateToLocalISO(this.rangeDates[0]);
      toDate = this.formatDateToLocalISO(this.rangeDates[1]);
    }

    let request$;
    if (this.selectedDataType === 'PlatformRevenue') {
      request$ = this.reportService.GetPlatformRevenueReport(fromDate, toDate, groupBy);
    } else if (this.selectedDataType === 'Packages') {
      request$ = this.reportService.GetUpgradesReport(fromDate, toDate, groupBy);
    } else {
      request$ = this.reportService.GetRevenueReport(fromDate, toDate, groupBy);
    }

    request$.subscribe({
      next: (data: ReportResponse) => {
        const summary = data?.Summary || {};
        this.totalRevenue = summary.TotalRevenue ?? 0;
        this.totalOrders = summary.TotalOrders ?? 0;
        this.totalTickets = summary.TotalTickets ?? 0;
        this.totalViews = summary.TotalViews ?? 0;
        this.growthRevenue = summary.GrowthRevenue ?? 0;
        this.growthOrders = summary.GrowthOrders ?? 0;
        this.growthTickets = summary.GrowthTickets ?? 0;
        this.growthViews = summary.GrowthViews ?? 0;

        this.cachedChartList = data?.Chart || [];
        this.updateChartData();

        this.cdr.markForCheck();
      },
      error: (err) => {
        let msg = 'Lỗi khi tải dữ liệu báo cáo.';
        if (err.error?.Message) {
          msg = err.error.Message;
        }
        alert(msg);
        this.resetStats();
        this.cdr.markForCheck();
      }
    });
  }

  updateChartData() {
    const isRevenue = this.selectedDataType === 'Revenue' || this.selectedDataType === 'PlatformRevenue' || this.selectedDataType === 'Packages';

    let label = 'Doanh thu';
    if (this.selectedDataType === 'Tickets') label = 'Vé đã bán';
    if (this.selectedDataType === 'Packages') label = 'Doanh thu gói';

    let color = '#81E979';
    if (this.selectedDataType === 'Tickets') color = '#3B82F6';
    if (this.selectedDataType === 'Packages') color = '#A855F7';

    let borderColor = '#6cc04a';
    if (this.selectedDataType === 'Tickets') borderColor = '#2563EB';
    if (this.selectedDataType === 'Packages') borderColor = '#9333EA';

    this.chartData = {
      labels: this.cachedChartList.map((c) => c.Label || ''),
      datasets: [
        {
          label: label,
          data: this.cachedChartList.map((c) => (this.selectedDataType === 'Tickets') ? (c.Tickets ?? 0) : (c.Revenue ?? 0)),
          backgroundColor: color,
          borderColor: borderColor,
          borderWidth: 1,
          borderRadius: 6,
          barThickness: 32,
        }
      ]
    };
    this.cdr.markForCheck();
  }

  private formatDateToLocalISO(date: Date): string {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
  }

  private resetStats() {
    this.totalRevenue = 0;
    this.totalOrders = 0;
    this.totalTickets = 0;
    this.totalViews = 0;
    this.growthRevenue = 0;
    this.growthOrders = 0;
    this.growthTickets = 0;
    this.growthViews = 0;
    this.chartData = { labels: [], datasets: [] };
  }

  formatGrowth(value: number): string {
    const val = value ?? 0;
    const sign = val >= 0 ? '+' : '';
    return `${sign}${val.toFixed(2)}%`;
  }

  formatCurrency(value: number): string {
    const val = value ?? 0;
    return new Intl.NumberFormat('vi-VN').format(val) + 'đ';
  }

  private initChartOptions() {
    this.chartOptions = {
      maintainAspectRatio: false,
      aspectRatio: 0.6,
      plugins: {
        legend: { display: false },
        tooltip: {
          mode: 'index',
          intersect: false,
          callbacks: {
            label: (context: any) => {
              let label = context.dataset.label || '';
              if (label) label += ': ';
              const val = context.parsed.y;
              if (val !== null) {
                if (this.selectedDataType === 'Revenue') {
                  label += new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
                } else {
                  label += val + ' vé';
                }
              }
              return label;
            }
          }
        }
      },
      scales: {
        x: {
          grid: { display: false, drawBorder: false },
          ticks: { color: '#94a3b8' }
        },
        y: {
          grid: { color: '#f1f5f9', drawBorder: false },
          ticks: {
            color: '#94a3b8',
            callback: (value: any) => {
              if (this.selectedDataType === 'Revenue') {
                return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(value);
              }
              return value;
            }
          }
        }
      }
    };
  }
}
