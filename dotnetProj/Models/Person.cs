
namespace dotnetProj.Models
{
    public partial class Person
    {
        public Person()
        {
            Tasks = new HashSet<Task>();
        }

        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string FavoriteProgrammingLanguage { get; set; } = null!;
        public string Id { get; set; } = null!;

        public virtual ICollection<Task> Tasks { get; set; }
    }
}
