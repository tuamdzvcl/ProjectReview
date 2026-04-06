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
  selectedCategoryId: number | null = null;

  @Output() categoryChange = new EventEmitter<number | null>();

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

  selectCategory(id: number | null): void {
    console.log(id)
    this.selectedCategoryId = id;
    this.categoryChange.emit(id);
  }
}
