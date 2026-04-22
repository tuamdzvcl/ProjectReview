export interface TicketInfo {
  TicketName: string;
  Quantity: number;
  Price: number;
}

export interface EventInfo {
  EventTitle: string;
  Tickets: TicketInfo[];
}

export interface UserInEvent {
  ID: string;
  UserName: string;
  Email: string;
  FirstName: string;
  LastName: string;
  Avarta?: string;
  Events: EventInfo[];
}
