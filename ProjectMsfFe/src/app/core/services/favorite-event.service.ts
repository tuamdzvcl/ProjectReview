import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FaviriteService {
  private readonly STORAGE_KEY =
    'favorite-events'; /* TODO: Define storage key (e.g. 'favorite-events') */

  // TODO: Define a BehaviorSubject to store an array of event IDs
  private favoritesState = new BehaviorSubject<string[]>([]);
  public favoritesState$ = this.favoritesState.asObservable();

  constructor() {
    this.loadFavorites();
  }

  private loadFavorites(): void {
    try {
      const savedata = localStorage.getItem(this.STORAGE_KEY);
      if (savedata) {
        this.favoritesState.next(JSON.parse(savedata));
      }
      // TODO: Read from localStorage and parse JSON, then update favoritesState
    } catch (e) {
      console.error('Error loading favorites from localStorage', e);
      this.favoritesState.next([]);
      localStorage.removeItem(this.STORAGE_KEY);

      // TODO: Handle error case (e.g. reset state, remove invalid item from localStorage)
    }
  }

  getFavorites(): string[] {
    console.log(this.favoritesState.value);
    return this.favoritesState.value;
    // TODO: Return the current array of favorite IDs from the state
  }

  isFavorite(eventId: string): boolean {
    // TODO: Return true if the eventId is in the favorites array
    return this.getFavorites().includes(eventId);
  }

  toggleFavorite(eventId: string): boolean {
    // TODO: Implement toggle logic (add if not exists, remove if exists)
    const currenFavorite = this.getFavorites();
    const index = currenFavorite.indexOf(eventId);
    let NewArr: string[];
    let isFavorite = false;

    if (index > -1) {
      NewArr = currenFavorite.filter((x) => x !== eventId);
    } else {
      NewArr = [...currenFavorite, eventId];
      isFavorite = true;
    }
    this.favoritesState.next(NewArr);
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(NewArr));

    // TODO: Update the BehaviorSubject state
    // TODO: Sync the updated array to localStorage

    return isFavorite;
  }
}
