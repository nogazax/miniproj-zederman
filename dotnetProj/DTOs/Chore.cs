
namespace dotnetProj.Models
{
    public partial class Chore : ITask
    {
        public string? Description { get; set; }
        public string? Size { get; set; }
    }
}
