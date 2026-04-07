import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppShellComponent } from "../../../../layouts/app-shell/app-shell.component";

@Component({
  selector: 'app-support',
  standalone: true,
  imports: [CommonModule, AppShellComponent],
  templateUrl: './support.component.html',
  styleUrl: './support.component.scss',
})
export class SupportComponent {}
