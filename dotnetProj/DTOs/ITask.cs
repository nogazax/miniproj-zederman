namespace dotnetProj.Models
{
	public class ITask
	{
		public string Id { get; set; } = null!;
		public string OwnerId { get; set; } = null!;
		public string Status { get; set; } = null!;
		public string Type { get; set; } = null!;

	}
}