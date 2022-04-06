
namespace dotnetProj.Models
{
    public partial class Task
    {
        public string Id { get; set; } = null!;
        public string OwnerId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Description { get; set; }
        public string? Size { get; set; }
        public string? Course { get; set; }
        public string? DueDate { get; set; }
        public string? Details { get; set; }

        public virtual Person Owner { get; set; } = null!;
    }
}
