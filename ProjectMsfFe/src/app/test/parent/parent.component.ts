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

  cart: any[] = [];

  addtoCart(product: any) {
    const ex = this.cart.find((x) => x.name === product.name);
    if (ex) {
      ex.quantity++;
    }
    else {
      this.cart.push({ name: product.name, price: product.price, quantity: 1 });
    }
    console.log('giỏ hàng hiện tại');
  }
  getTotalPrice() {
    let pricesum = 0;
    for (let i of this.cart) {
      pricesum += (i.price * i.quantity)
    }
    return pricesum;
  }
}
