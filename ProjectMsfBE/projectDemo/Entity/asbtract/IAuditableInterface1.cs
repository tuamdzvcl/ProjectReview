namespace EventTick.Model.asbtract
{
    public interface IAuditableInterface1
    {
        DateTimeOffset? CreatedDate { get; set; }
        string? CreatedBy { get; set; }
        DateTimeOffset? UpdatedDate { get; set; }
        string? UpdatedBy { get; set; }
        bool? IsDeleted { get; set; }
    }
}
