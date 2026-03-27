import { Component } from '@angular/core';
import { HeroSearchComponent } from '../../components/hero-search/hero-search.component';
import { FilterTabsComponent } from '../../components/filter-tabs/filter-tabs.component';
import { EventsGridComponent } from '../../components/events-grid/events-grid.component';
import { UserDropdownComponent } from '../../../../shared/components/user-dropdown/user-dropdown.component';

@Component({
  selector: 'app-events-page',
  standalone: true,
  imports: [HeroSearchComponent,FilterTabsComponent,EventsGridComponent,UserDropdownComponent
  ],
  templateUrl: './events-page.component.html',
  styleUrl: './events-page.component.scss'
})
export class EventsPageComponent {
  
}

