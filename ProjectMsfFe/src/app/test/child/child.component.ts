import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-child',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './child.component.html',
  styleUrl: './child.component.scss',
})
export class ChildComponent {
  @Input() prodouct: any;

  @Output() onBuy = new EventEmitter<any>();

  buyProdouct() {
    this.onBuy.emit(this.prodouct);
    console.log('đã thêm');
  }
}
