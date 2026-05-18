import { Component, EventEmitter, Output } from '@angular/core';
import { ChildComponent } from '../child/child.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-parent',
  standalone: true,
  imports: [ChildComponent, CommonModule],
  templateUrl: './parent.component.html',
  styleUrl: './parent.component.scss',
})
export class ParentComponent {
  menu = [
    { id: 1, name: 'Trà sữa Trân Châu', price: 30000 },
    { id: 2, name: 'Trà Đào Cam Sả', price: 35000 },
    { id: 3, name: 'Lục trà dâu', price: 40000 },
  ];

  cart: string[] = [];

  addtoCart(productName: string) {
    this.cart.push(productName);
    console.log('giỏ hàng hiện tại');
  }
}
