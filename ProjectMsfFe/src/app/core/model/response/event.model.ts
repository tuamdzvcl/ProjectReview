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
  UserID: string;
  CatetoryId: number;
  CatetoryName: string;
  ListTypeTick: Array<TypeTickResponse>;
  Reason?: string;
  isfaslse?: boolean;
}
