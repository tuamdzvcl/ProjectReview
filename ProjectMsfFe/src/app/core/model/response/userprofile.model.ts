import { EventModel } from "./event.model";

export interface UserProfile {
    User: UserResponseProfile;
    Events: EventModel[];
}

export interface UserResponseProfile {
    LastName: string;
    FirstName: string;
    AvatarUrl: string;
}