import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HeroSearchComponent } from '../../../../shared/components/hero-search/hero-search.component';
import { FilterTabsComponent } from '../../../../shared/components/filter-tabs/filter-tabs.component';
import { EventsGridComponent } from '../../components/events-grid/events-grid.component';
import { UserDropdownComponent } from '../../../../shared/components/user-dropdown/user-dropdown.component';

@Component({
  selector: 'app-events-page',
  standalone: true,
  imports: [HeroSearchComponent, FilterTabsComponent, EventsGridComponent, UserDropdownComponent
  ],
  templateUrl: './events-page.component.html',
  styleUrl: './events-page.component.scss'
})
export class EventsPageComponent implements OnInit {
  selectedCategoryIds: string[] = [];
  searchKeyword: string = '';
  showFavorites: boolean = false;

  constructor(private route: ActivatedRoute) {}

  ngOnInit() {
    // Đọc query parameters từ URL mỗi khi URL thay đổi
    this.route.queryParams.subscribe(params => {
      this.searchKeyword = params['keyword'] || '';
    });
  }

  onCategoryChange(categoryIds: string[]) {
    this.selectedCategoryIds = categoryIds;
  }

  onShowFavoritesChange(showFav: boolean) {
    this.showFavorites = showFav;
  }
}
