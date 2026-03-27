namespace EventTick.Model.asbtract
{
    public interface IAuditableInterface1
    {
        DateTime? CreatedDate { get; set; }
        string? CreatedBy { get; set; }
        DateTime? UpdatedDate { get; set; }
        string? UpdatedBy { get; set; }
        bool? IsDeleted { get; set; }

    }
}
