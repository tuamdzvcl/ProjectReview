import { Component } from '@angular/core';
import { EventsGridComponent } from '../../components/events-grid/events-grid.component';
import { UserDropdownComponent } from '../../../../shared/components/user-dropdown/user-dropdown.component';
import { FilterTabsComponent } from '../../../../shared/components/filter-tabs/filter-tabs.component';
import { HeroSearchComponent } from '../../../../shared/components/hero-search/hero-search.component';

@Component({
  selector: 'app-events-page',
  standalone: true,
  imports: [HeroSearchComponent, FilterTabsComponent, EventsGridComponent],
  templateUrl: './events-page.component.html',
  styleUrl: './events-page.component.scss',
})
export class EventsPageComponent {}
