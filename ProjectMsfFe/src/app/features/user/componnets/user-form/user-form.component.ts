import {
  Component,
  Input,
  Output,
  EventEmitter,
  inject,
  OnChanges,
  HostListener,
} from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown'; 
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    DropdownModule,
    CommonModule,
  ],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.scss',
})
export class UserFormComponent implements OnChanges {
  @Input() visible = false;
  @Input() data: any | null = null;

  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() submitForm = new EventEmitter<any>();

  private fb = inject(FormBuilder);

  form = this.fb.group({
    id: [null],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    role: [null, Validators.required],
  });

  roles = [
    { label: 'Admin', value: 'admin' },
    { label: 'Organizer', value: 'organizer' },
    { label: 'Customer', value: 'customer' },
  ];
  ngOnChanges() {
    if (this.data) {
      this.form.patchValue(this.data);
    } else {
      this.form.reset();
    }
  }
  close() {
    this.visibleChange.emit(false);
  }
  onSubmit() {
    if (this.form.invalid) return;
    console.log('test');

    this.submitForm.emit(this.form.value);
    this.close();
  }
}
