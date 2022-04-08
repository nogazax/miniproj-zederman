namespace dotnetProj.Models
{
    public partial class HomeWork : ITask
    {
        public string Id { get; set; } = null!;
        public string OwnerId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Course { get; set; }
        public string? DueDate { get; set; }
        public string? Details { get; set; }

    }
}
