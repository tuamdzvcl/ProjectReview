import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DatePickerModule } from 'primeng/datepicker';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';

export interface FilterData {
  keyword: string;
  startDate: Date | null;
  endDate: Date | null;
  status: any | null;
}

@Component({
  selector: 'app-data-filter',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DatePickerModule,
    DropdownModule,
    InputTextModule,
    ButtonModule
  ],
  templateUrl: './data-filter.component.html',
  styleUrls: ['./data-filter.component.scss']
})
export class DataFilterComponent {
  @Input() placeholder: string = 'Tìm kiếm...';
  @Input() statusOptions: any[] = [];
  @Input() statusPlaceholder: string = 'Trạng thái';

  @Output() filterChange = new EventEmitter<FilterData>();
  @Output() reset = new EventEmitter<void>();
  filter: FilterData = {
    keyword: '',
    startDate: null,
    endDate: null,
    status: null
  };

  onFilter(): void {
    this.filterChange.emit({ ...this.filter });
  }

  onReset(): void {
    this.filter = {
      keyword: '',
      startDate: null,
      endDate: null,
      status: null
    };
    this.reset.emit();
    this.onFilter();
  }
}
