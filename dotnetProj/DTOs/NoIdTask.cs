namespace dotnetProj.Models
{
    public partial class NoIdTask
    {
        public string? OwnerId { get; set; } = null!;
        public string? Status { get; set; } = null!;
        public string? Type { get; set; } = null!;
        public string? Description { get; set; }
        public string? Size { get; set; }
        public string? Course { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Details { get; set; }
    }
}
