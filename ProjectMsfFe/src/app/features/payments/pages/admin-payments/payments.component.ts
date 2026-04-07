import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppShellComponent } from '../../../../layouts/app-shell/app-shell.component';

@Component({
  selector: 'app-payments',
  standalone: true,
  imports: [CommonModule, AppShellComponent],
  templateUrl: './payments.component.html',
  styleUrl: './payments.component.scss'
})
export class PaymentsComponent {
}
