export interface PermissionItem {
  Id: number;
  PermissonsName: string;
  PermissonsDescription: string;
}

export interface RoleItem {
  Id: number;
  RoleName: string;
  Permissions: PermissionItem[];
  CreatedAt: string;
  IsSystem: boolean;
}

export interface RoleSaveEvent {
  roleName: string;
  permissionIds: number[];
  roleId: number | null;
  isEdit: boolean;
}

export interface PermissionSaveEvent {
  roleId: number;
  permissionIds: number[];
}
