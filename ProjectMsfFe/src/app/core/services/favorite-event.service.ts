import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { BaseApiService } from './base-api.service';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root',
})
export class FaviriteService extends BaseApiService {
  private readonly STORAGE_KEY = 'favorite-events';

  private favoritesState = new BehaviorSubject<string[]>([]);

  public favoritesState$ = this.favoritesState.asObservable();

  private toggleTrigger$ = new Subject<{
    eventId: string;
    isFavorite: boolean;
  }>();

  constructor(http: HttpClient, private tokenService: TokenService) {
    super(http);
    this.loadFavoritesLocal();
    this.syncFavoritesFromApi();
    this.setupApiSync();
  }
  private loadFavoritesLocal(): void {
    try {
      const savedata = localStorage.getItem(this.STORAGE_KEY);
      if (savedata) {
        this.favoritesState.next(JSON.parse(savedata));
      }
    } catch (e) {
      console.error('Error loading favorites from localStorage', e);
      this.favoritesState.next([]);
      localStorage.removeItem(this.STORAGE_KEY);
    }
  }

  public clearFavorites(): void {
    this.favoritesState.next([]);
    localStorage.removeItem(this.STORAGE_KEY);
  }

  public syncFavoritesFromApi(): void {
    if (!this.tokenService.getAccessToken()) return;
    this.get<any>('UserEventFavorite/my-favorites').subscribe({
      next: (res) => {
        const responseData = res.Data;
        if (responseData) {
          const ids = responseData.map((id: any) =>
            id.toString().toLowerCase()
          );
          this.favoritesState.next(ids);
          localStorage.setItem(this.STORAGE_KEY, JSON.stringify(ids));
        }
      },
      error: (err) =>
        console.error('Lỗi lấy danh sách favorite từ Server:', err),
    });
  }

  private setupApiSync(): void {
    this.toggleTrigger$
      .pipe(debounceTime(500))
      .subscribe(({ eventId, isFavorite }) => {
        if (!this.tokenService.getAccessToken()) {
          console.warn('Chưa đăng nhập, yêu thích chỉ lưu trên LocalStorage');
          return;
        }

        this.post<any>(`UserEventFavorite/toggle/${eventId}`, {}).subscribe({
          next: (res) => {
            console.log('Đồng bộ thành công:', res.Message);
          },
          error: (err) => {
            console.error('Lỗi đồng bộ API', err);
            this.rollbackFavorite(eventId, !isFavorite);
          },
        });
      });
  }

  getFavorites(): string[] {
    return this.favoritesState.value;
  }

  isFavorite(eventId: string): boolean {
    return this.getFavorites().includes(eventId.toLowerCase());
  }

  toggleFavorite(eventId: string): boolean {
    eventId = eventId.toLowerCase();
    const currentFavorites = this.getFavorites();
    const index = currentFavorites.indexOf(eventId);
    let newArr: string[];
    let isFavorite = false;

    if (index > -1) {
      newArr = currentFavorites.filter((x) => x !== eventId);
    } else {
      newArr = [...currentFavorites, eventId];
      isFavorite = true;
    }

    this.favoritesState.next(newArr);
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(newArr));

    this.toggleTrigger$.next({ eventId, isFavorite });

    return isFavorite;
  }

  private rollbackFavorite(eventId: string, revertToIsFavorite: boolean): void {
    const currentFavorites = this.getFavorites();
    let newArr: string[];

    if (revertToIsFavorite) {
      if (!currentFavorites.includes(eventId))
        newArr = [...currentFavorites, eventId];
      else newArr = currentFavorites;
    } else {
      newArr = currentFavorites.filter((x) => x !== eventId);
    }

    this.favoritesState.next(newArr);
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(newArr));
    alert('Lỗi mạng, không thể lưu sự kiện yêu thích lúc này!');
  }
}
