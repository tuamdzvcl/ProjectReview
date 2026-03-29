import { TypeTickResponse } from './TypeTickResponse';

export interface EventModel {
  Id: number;
  EventID?: number;
  Title: string;
  Description: string;
  PosterUrl: string;
  Status: string;
  Location: string;
  StartDate: Date;
  EndDate: Date;
  SaleStartDate: Date;
  SaleEndDate: Date;
  UserName: string;
  CatetoryName: string;
  ListTypeTick: Array<TypeTickResponse>;
}
