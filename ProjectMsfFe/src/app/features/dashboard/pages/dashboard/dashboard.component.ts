import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { AppShellComponent } from '../../../../layouts/app-shell/app-shell.component';
import { ChartModule } from 'primeng/chart';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, AppShellComponent, ChartModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent implements OnInit {
  chartData: any;
  chartOptions: any;
  selectedTab: string = 'Monthly';

  ngOnInit() {
    this.initChartOptions();
    this.fetchRevenueStats('Monthly');
  }

  /** Giả lập gọi API lấy dữ liệu doanh thu */
  fetchRevenueStats(type: string) {
    this.selectedTab = type;

    // Giả lập dữ liệu trả về từ API tùy theo loại (Tháng/Tuần/Ngày)
    const mockApiResponse = this.getMockData(type);

    this.chartData = {
      labels: mockApiResponse.labels,
      datasets: [
        {
          label: 'Revenue',
          data: mockApiResponse.data,
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
  }

  private initChartOptions() {
    this.chartOptions = {
      maintainAspectRatio: false,
      aspectRatio: 0.6,
      plugins: {
        legend: {
          display: false
        },
        tooltip: {
          mode: 'index',
          intersect: false,
        }
      },
      scales: {
        x: {
          grid: {
            display: false,
            drawBorder: false
          },
          ticks: {
            color: '#94a3b8'
          }
        },
        y: {
          grid: {
            color: '#f1f5f9',
            drawBorder: false
          },
          ticks: {
            color: '#94a3b8',
            callback: (value: any) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value)
          }
        }
      }
    };
  }

  private getMockData(type: string) {
    if (type === 'Daily') {
      return {
        labels: ['Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7', 'CN'],
        data: [1200000, 3000000, 1500000, 4500000, 2000000, 5000000, 3500000]
      };
    } else if (type === 'Weekly') {
      return {
        labels: ['Tuần 1', 'Tuần 2', 'Tuần 3', 'Tuần 4'],
        data: [12000000, 21000000, 18000000, 35000000]
      };
    } else { // Monthly
      return {
        labels: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
        data: [50000000, 70000000, 45000000, 80000000, 60000000, 95000000, 70000000, 110000000, 85000000, 120000000, 100000000, 150000000]
      };
    }
  }
}

