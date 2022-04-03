
namespace dotnetProj.Models
{
    public partial class NoIdOwnerIdTask
    {
        public string Status { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Description { get; set; }
        public string? Size { get; set; }
        public string? Course { get; set; }
        public string? DueDate { get; set; }
        public string? Details { get; set; }
    }
}
