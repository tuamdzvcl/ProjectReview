import { UserResponse } from './user.model';
import { EventModel } from './event.model';

export interface UserEventsResponse {
  User: UserResponse;
  Events: EventModel[];
}
