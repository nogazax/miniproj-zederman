
namespace dotnetProj.Models
{
    public partial class Chore : ITask
    {
        public string Id { get; set; } = null!;
        public string OwnerId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Description { get; set; }
        public string? Size { get; set; }
    }
}
