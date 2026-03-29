import { Component, inject } from '@angular/core';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { CommonModule, NgTemplateOutlet } from '@angular/common';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import Swal from 'sweetalert2';
import { AppShellComponent } from '../../../../layouts/app-shell/app-shell.component';
import { UserFormComponent } from '../../componnets/user-form/user-form.component';
import { UserService } from '../../../../core/services/user.service';
import { UserResponse } from '../../../../core/model/user.model';
import { UserUpdata } from '../../../../core/model/update/userupdate.model';
import { UserRequest } from '../../../../core/model/request/userRequest.model';
@Component({
  selector: 'app-user',
  standalone: true,
  imports: [
    AppShellComponent,
    TableModule,
    ButtonModule,
    CommonModule,
    ConfirmPopupModule,
    ToastModule,
    UserFormComponent,
  ],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss',
  providers: [MessageService, ConfirmationService],
})
export class UserComponent {
  private userService = inject(UserService);

  count = 4;
  users: UserResponse[] = [];
  totalRecords = 0;
  first = 0;
  rows = 10;
  pageIndex = 1;
  keyword = '';
  selectedRole = '';

  totalAll = 0;
  totalOrganizer = 0;
  totalCustomer = 0;

  //luôn load
  ngOnInit(): void {
    this.loadUsers();
    this.setTotals;
  }
  loadUsers() {
    this.userService
      .GetUsers(this.pageIndex, this.rows, '', this.selectedRole)
      .subscribe({
        next: (res) => {
          console.log(res);
          this.setTotals(res.items);
          console.log('User');
          this.users = res.items;
          this.totalRecords = res.totalRecords;
        },
      });
  }

  setTotals(users: any[]) {
    this.totalAll = users.length;

    this.totalOrganizer = users.filter(
      (u) => (u.role || u.RoleName[0]) === 'organizer'
    ).length;

    this.totalCustomer = users.filter(
      (u) => (u.role || u.RoleName[0]) === 'customer'
    ).length;
  }

  pageChange(event: any) {
    this.first = event.first;
    this.rows = event.rows;
    this.pageIndex = event.first / event.rows + 1;
    this.loadUsers();
  }
  isFirstPage(): boolean {
    return this.first === 0;
  }

  isLastPage(): boolean {
    return this.first + this.rows >= this.totalRecords;
  }

  next() {
    if (this.isLastPage()) return;
    this.first += this.rows;
    this.pageIndex++;
    this.loadUsers();
  }

  prev() {
    if (this.isFirstPage()) return;
    this.first -= this.rows;
    this.pageIndex--;
    this.loadUsers();
  }
  reset() {
    console.log('test');
  }

  filterRole(role: string) {
    this.selectedRole = role;
    this.pageIndex = 1;
    this.first = 0;
    this.loadUsers();
  }
  onEdit(users: any) {
    this.selectedUser = {
      id: users?.ID ?? users?.ID ?? null,
      firstName: users?.FirstName ?? users?.firstName ?? '',
      lastName: users?.LastName ?? users?.lastName ?? '',
      email: users?.Email ?? users?.email ?? '',
      role: Array.isArray(users?.RoleName)
        ? users.RoleName[0]
        : users?.RoleName ?? users?.role ?? null,
    };
    this.display = true;
  }
  onDelete(users: any) {
    Swal.fire({
      title: 'Are you sure?',
      text: "You won't be able to revert this!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, delete it!',
    }).then((result) => {
      if (result.isConfirmed) {
        this.userService.deleteUser(users.ID).subscribe({
          next: () => {
            Swal.fire({
              title: 'Deleted!',
              text: 'User has been deleted.',
              icon: 'success',
            });
            this.loadUsers();
          },
          error: (err) => {
            Swal.fire({
              title: 'Error!',
              text: err.message,
              icon: 'error',
            });
          },
        });
      }
    });
  }

  display = false;
  selectedUser: any = null;

  onSave() {
    console.log('được rồi ');
    this.selectedUser = null;
    this.display = true;
  }

  handleSubmit(data: any) {
    if (data.id) {
      const payload = this.mapToUpdatePayload(data);

      this.userService.UpdateUser(data.id, payload).subscribe({
        next: () => {
          Swal.fire({
            title: 'Update thành công',
            icon: 'success',
            draggable: true,
          });
          this.display = false;
          this.loadUsers();
        },
        error: (err) => {
          console.error('Update thất bại', err);
        },
      });
    } else {
      const payload = this.mapToCreatePayload(data);
      console.log('Payload to send:', payload);

      this.userService.CreateUser(payload).subscribe({
        next: () => {
          Swal.fire({
            title: 'Create thành công',
            icon: 'success',
            draggable: true,
          });
          this.display = false;
          this.loadUsers();
        },
        error: (err) => {
          console.error('Create thất bại', err.error.errors);
          Swal.fire({
            title: 'Error!',
            text: err.message || 'Create user failed',
            icon: 'error',
          });
        },
      });
    }
  }
  private mapToUpdatePayload(data: any): UserUpdata {
    return {
      FirstName: data.firstName,
      LastName: data.lastName,
      AvataUrl: data.avataUrl ?? '',
      RoleName: data.role,
      UpdateAt: new Date().toISOString(),
      UpdateBy: 'admin',
    };
  }

  private mapToCreatePayload(data: any): UserRequest {
    return {
      Email: data.email,
      Username: data.firstName + ' ' + data.lastName,
      FirstName: data.firstName,
      LastName: data.lastName,
      RoleName: [data.role],
      AvataUrl: data.avataUrl ?? '',
    };
  }
}
