/**
 * Generic count response model.
 * Maps any status key (string) to its count (number).
 * Works with any enum from BE (EventStatus, OrderStatus, UserStatus, etc.)
 */
export interface CountResponse {
  All: Number;
  Items: Record<string, number>;
}
