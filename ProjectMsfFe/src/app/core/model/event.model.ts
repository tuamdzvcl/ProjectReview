import { TypeTickResponse } from './TypeTickResponse';

export interface EventModel {
  EventID: number;
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
  ListTypeTick: Array<TypeTickResponse>;
}
