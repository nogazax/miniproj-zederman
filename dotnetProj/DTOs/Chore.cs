
namespace dotnetProj.Models
{
    public partial class Chore : ITask
    {
 
        public string Type { get; set; } = null!;

        public string? Description { get; set; }
        public string? Size { get; set; }
    }
}
