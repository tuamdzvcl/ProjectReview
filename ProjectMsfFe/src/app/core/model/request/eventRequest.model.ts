import { TicketTypes } from "./typeTickRequest.model";

export interface EventRequest {
  Title: string;
  Description: string;
  PosterUrl: string;
  Status: string;
  Location: string;
  StartDate: Date;
  EndDate: Date;
  SaleStartDate: Date;
  SaleEndDate: Date;
  UserID: number;
  CatetoryName: string;
  TicketTypes: Array<TicketTypes>;
}
