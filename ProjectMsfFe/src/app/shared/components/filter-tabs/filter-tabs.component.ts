import { FaviriteService } from './../../../core/services/favorite-event.service';
import { CommonModule } from '@angular/common';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CatetoryService } from '../../../core/services/catetory.service';
import { CatetoryResponse } from '../../../core/model/response/catetory.model';

@Component({
  selector: 'app-filter-tabs',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './filter-tabs.component.html',
  styleUrl: './filter-tabs.component.scss',
})
export class FilterTabsComponent implements OnInit {
  categories: CatetoryResponse[] = [];
  selectedCategoryIds: string[] = [];

  @Output() categoryChange = new EventEmitter<string[]>();
  @Output() filterFavorites = new EventEmitter<boolean>();
  
  showFavorites: boolean = false;

  constructor(
    private catetoryService: CatetoryService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.catetoryService.GetCatetory().subscribe({
      next: (response: { Data: CatetoryResponse[] }) => {
        this.categories = response.Data;
      },
      error: (err: any) => {
        console.error('Error fetching categories:', err);
      },
    });
  }

  selectCategory(id: string | null): void {
    if (this.showFavorites) {
      this.showFavorites = false;
      this.filterFavorites.emit(this.showFavorites);
    }

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

  favorites() {
    this.showFavorites = !this.showFavorites;
    if (this.showFavorites) {
      this.selectedCategoryIds = [];
      this.categoryChange.emit(this.selectedCategoryIds);
    }
    this.filterFavorites.emit(this.showFavorites);
  }
}
