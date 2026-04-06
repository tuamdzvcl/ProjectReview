import { OrderItemRequest } from './orderItemRequest.model';
import { UserOrder } from './userOrder.model';

export interface CreateOrderRequest {
  User: UserOrder;
  Items: OrderItemRequest[];
}