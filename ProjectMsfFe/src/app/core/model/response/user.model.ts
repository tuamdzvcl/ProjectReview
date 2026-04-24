export interface UserResponse {
    Id: string
    Email: string,
    Username: string,
    LastName: string
    FirstName: string
    RoleName: Array<string>
    AvatarUrl: string
    Permissions?: string[]
}