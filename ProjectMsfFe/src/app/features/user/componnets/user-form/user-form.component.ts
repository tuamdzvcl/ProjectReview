import {
  Component,
  Input,
  Output,
  EventEmitter,
  inject,
  OnChanges,
  OnInit,
  HostListener,
} from '@angular/core';
import { RolePermissionService } from '../../../../core/services/role-permission.service';
import { FormBuilder, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { CommonModule } from '@angular/common';
import { ApiErrorHandler } from '../../../../core/utils/api-error-handler.util';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    CheckboxModule,
    FormsModule,
    CommonModule,
  ],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.scss',
})
export class UserFormComponent implements OnChanges, OnInit {
  @Input() visible = false;
  @Input() data: any | null = null;

  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() submitForm = new EventEmitter<any>();

  private fb = inject(FormBuilder);

  form = this.fb.group({
    id: [null],
    firstName: ['', [Validators.required, Validators.pattern('^(?=.*[a-zA-ZÀ-ỹ])[a-zA-ZÀ-ỹ\\s]+$')]],
    lastName: ['', [Validators.required, Validators.pattern('^(?=.*[a-zA-ZÀ-ỹ])[a-zA-ZÀ-ỹ\\s]+$')]],
    email: ['', [Validators.required, Validators.email]],
    role: [[] as number[], Validators.required],
  });

  roles: any[] = [];
  private roleService = inject(RolePermissionService);

  ngOnInit() {
    this.roleService.getRolePermissions().subscribe({
      next: (res) => {
        this.roles = res.map((role: any) => ({
          label: role.RoleName,
          value: role.Id
        }));
        this.updateSelectedRoles();
      },
      error: (err) => {
        ApiErrorHandler.handleError(err, "Lỗi mất tiêu rồi")
      }
    });
  }
  selectedRoles: number[] = [];

  selectAll() {
    this.selectedRoles = this.roles.map(r => r.value);
    this.onRoleChange();
  }

  unselectAll() {
    this.selectedRoles = [];
    this.onRoleChange();
  }

  toggleRole(roleValue: number) {
    const index = this.selectedRoles.indexOf(roleValue);
    if (index > -1) {
      this.selectedRoles.splice(index, 1);
    } else {
      this.selectedRoles.push(roleValue);
    }
    this.selectedRoles = [...this.selectedRoles];
    this.onRoleChange();
  }

  onRoleChange() {
    this.form.get('role')?.setValue(this.selectedRoles as any);
  }

  private updateSelectedRoles() {
    if (!this.data?.role || this.roles.length === 0) return;

    // Ép kiểu mọi data (dù là mảng chữ, mảng số hay chuỗi đơn) về chung 1 mảng để dễ duyệt
    const roleData = Array.isArray(this.data.role) ? this.data.role : [this.data.role];

    // Quét qua danh sách Role gốc, giữ lại những Role nào trùng ID hoặc trùng Tên (tự bọc lỗi chữ Hoa thường)
    this.selectedRoles = this.roles
      .filter(r => roleData.some((item: any) => 
        item === r.value || (typeof item === 'string' && item.toLowerCase() === r.label.toLowerCase())
      ))
      .map(r => r.value);

    this.onRoleChange();
  }

  ngOnChanges() {
    if (this.data) {
      this.form.patchValue(this.data);
      this.updateSelectedRoles();
    } else {
      this.form.reset();
      this.selectedRoles = [];
      this.onRoleChange();
    }
  }
  close() {
    this.visibleChange.emit(false);
  }
  onSubmit() {
    if (this.form.invalid) return;
    this.submitForm.emit(this.form.value);
    this.close();
  }
}
