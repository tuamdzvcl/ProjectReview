import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FaviriteService {
  private readonly STORAGE_KEY = 'favorite-events';
  private favoritesState = new BehaviorSubject<string[]>([]);
  public favoritesState$ = this.favoritesState.asObservable();

  constructor() {
    this.loadFavorites();
  }

  private loadFavorites(): void {
    try {
      const savedData = localStorage.getItem(this.STORAGE_KEY);
      if (savedData) {
        this.favoritesState.next(JSON.parse(savedData));
      }
    } catch (e) {
      console.error('Error loading favorites from localStorage', e);
      this.favoritesState.next([]);
      localStorage.removeItem(this.STORAGE_KEY);
    }
  }

  getFavorites(): string[] {
    return this.favoritesState.value;
  }

  isFavorite(eventId: string): boolean {
    return this.getFavorites().includes(eventId);
  }

  toggleFavorite(eventId: string): boolean {
    const currentFavorites = this.getFavorites();
    const index = currentFavorites.indexOf(eventId);
    let newFavorites: string[];
    let isNowFavorite = false;

    if (index > -1) {
      // Remove from favorites
      newFavorites = currentFavorites.filter(id => id !== eventId);
    } else {
      // Add to favorites
      newFavorites = [...currentFavorites, eventId];
      isNowFavorite = true;
    }

    this.favoritesState.next(newFavorites);
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(newFavorites));
    
    return isNowFavorite;
  }
}
