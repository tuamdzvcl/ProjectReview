import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AppShellComponent } from '../../../../layouts/app-shell/app-shell.component';
import { ChartModule } from 'primeng/chart';
import { DatePicker } from 'primeng/datepicker';
import { ReportService } from '../../../../core/services/report.service';
import { ReportResponse } from '../../../../core/model/response/report.model';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, AppShellComponent, ChartModule, DatePicker, FormsModule, VndCurrencyPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent implements OnInit {
  private reportService = inject(ReportService);
  private cdr = inject(ChangeDetectorRef);

  chartData: any;
  chartOptions: any;
  selectedTab: string = 'Monthly';

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
    this.initChartOptions();

    // Default range: current month
    const now = new Date();
    const firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
    const lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0);
    this.rangeDates = [firstDay, lastDay];

    this.fetchRevenueStats('Monthly');
  }

  onDateChange() {
    // Only refresh when both start and end dates are picked
    if (this.rangeDates && this.rangeDates[0] && this.rangeDates[1]) {
      this.fetchRevenueStats(this.selectedTab);
    }
  }

  fetchRevenueStats(type: string) {
    this.selectedTab = type;
    const groupBy = type.toLowerCase();

    let fromDate: string | undefined;
    let toDate: string | undefined;

    if (this.rangeDates && this.rangeDates[0] && this.rangeDates[1]) {
      fromDate = this.rangeDates[0].toISOString();
      toDate = this.rangeDates[1].toISOString();
    }

    this.reportService.GetRevenueReport(fromDate, toDate, groupBy).subscribe({
      next: (data: ReportResponse) => {
        // Safe mapping with defaults to 0
        const summary = data?.Summary || {};
        this.totalRevenue = summary.TotalRevenue ?? 0;
        this.totalOrders = summary.TotalOrders ?? 0;
        this.totalTickets = summary.TotalTickets ?? 0;
        this.totalViews = summary.TotalViews ?? 0;
        this.growthRevenue = summary.GrowthRevenue ?? 0;
        this.growthOrders = summary.GrowthOrders ?? 0;
        this.growthTickets = summary.GrowthTickets ?? 0;
        this.growthViews = summary.GrowthViews ?? 0;

        // Chart data
        const chartList = data?.Chart || [];
        this.chartData = {
          labels: chartList.map((c) => c.Label || ''),
          datasets: [
            {
              label: 'Doanh thu',
              data: chartList.map((c) => c.Revenue ?? 0),
              fill: true,
              borderColor: '#6cc04a',
              tension: 0.4,
              backgroundColor: 'rgba(108, 192, 74, 0.1)',
              pointBackgroundColor: '#6cc04a',
              pointBorderColor: '#fff',
              pointHoverBackgroundColor: '#fff',
              pointHoverBorderColor: '#6cc04a'
            }
          ]
        };

        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Lỗi khi tải báo cáo doanh thu:', err);
        // Reset to 0 on error
        this.resetStats();
        this.cdr.markForCheck();
      }
    });
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
              if (context.parsed.y !== null) {
                label += new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(context.parsed.y);
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
            callback: (value: any) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value)
          }
        }
      }
    };
  }
}
