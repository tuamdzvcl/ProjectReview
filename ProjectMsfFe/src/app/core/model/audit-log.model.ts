export interface AuditLog {
    Id: number;
    UserId: string | null;
    Username: string | null;
    Method: string;
    Path: string;
    StatusCode: number;
    IpAddress: string | null;
    Timestamp: string;
    Note: string | null;
}

export interface AuditLogResponse {
    TotalRecords: number;
    PageIndex: number;
    PageSize: number;
    Items: AuditLog[];
}
