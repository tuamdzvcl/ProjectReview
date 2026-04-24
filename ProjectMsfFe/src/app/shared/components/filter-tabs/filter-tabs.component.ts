import { CommonModule } from '@angular/common';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CatetoryService } from '../../../core/services/catetory.service';
import { CatetoryResponse } from '../../../core/model/response/catetory.model';

@Component({
  selector: 'app-filter-tabs',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './filter-tabs.component.html',
  styleUrl: './filter-tabs.component.scss'
})
export class FilterTabsComponent implements OnInit {
  categories: CatetoryResponse[] = [];
  selectedCategoryIds: string[] = [];

  @Output() categoryChange = new EventEmitter<string[]>();

  constructor(private catetoryService: CatetoryService) { }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.catetoryService.GetCatetory().subscribe({
      next: (response: { Data: CatetoryResponse[] }) => {
        console.log(response.Data)
        this.categories = response.Data;
      },
      error: (err: any) => {
        console.error('Error fetching categories:', err);
      }
    });
  }

  selectCategory(id: string | null): void {
    if (id === null) {
      this.selectedCategoryIds = [];
    } else {
      const index = this.selectedCategoryIds.indexOf(id);
      if (index > -1) {
        this.selectedCategoryIds.splice(index, 1);
      } else {
        this.selectedCategoryIds.push(id);
      }
    }
    // ensure reference change for angular change detection
    this.selectedCategoryIds = [...this.selectedCategoryIds];
    this.categoryChange.emit(this.selectedCategoryIds);
  }
}
