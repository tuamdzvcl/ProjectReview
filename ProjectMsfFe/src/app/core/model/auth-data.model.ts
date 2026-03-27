import { UserResponse } from "./user.model";

export interface AuthData {
  AccessToken: string;
  RefreshToken: string;
  ExpiredAt: string;
  User : UserResponse
}