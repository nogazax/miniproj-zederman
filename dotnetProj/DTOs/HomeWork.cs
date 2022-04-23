namespace dotnetProj.Models
{
    public partial class HomeWork : ITask
    {
        public string? Course { get; set; }
        public string? DueDate { get; set; }
        public string? Details { get; set; }

    }
}
