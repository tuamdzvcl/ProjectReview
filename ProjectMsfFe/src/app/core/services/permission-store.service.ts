import { Injectable, signal, computed } from '@angular/core';
import { UserService } from './user.service';
import { firstValueFrom } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PermissionStoreService {
  private _permissions = signal<string[]>([]);
  private _loaded = signal(false);

  readonly permissions = this._permissions.asReadonly();

  readonly loaded = this._loaded.asReadonly();

  constructor(private userService: UserService) {}
  async loadPermissions(): Promise<void> {
    try {
      const user = await firstValueFrom(this.userService.GetUserbyid());
      this._permissions.set(
        (user.Permissions ?? []).map((p) => p.toUpperCase())
      );
      this._loaded.set(true);
    } catch (err) {
      console.error('Failed to load permissions:', err);
      this._permissions.set([]);
      this._loaded.set(true);
    }
  }

  hasPermission(permission: string): boolean {
    return this._permissions().includes(permission.toUpperCase());
  }
  hasAnyPermission(permissions: string[]): boolean {
    const current = this._permissions();
    return permissions.some((p) => current.includes(p.toUpperCase()));
  }

  clear(): void {
    this._permissions.set([]);
    this._loaded.set(false);
  }
}
