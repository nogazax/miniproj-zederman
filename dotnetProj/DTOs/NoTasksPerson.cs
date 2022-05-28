
namespace dotnetProj.Models
{
    public partial class NoTasksPerson
    {
        public string? Id { get; set; } = null!;
        public string? Emails { get; set; } = null!;
        public string? Name { get; set; } = null!;
        public string? FavoriteProgrammingLanguage { get; set; } = null!;
        public int? ActiveTaskCount { get; set; } = 0!;
    }
}
