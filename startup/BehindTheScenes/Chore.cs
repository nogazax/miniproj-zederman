

namespace BehindTheScenes
{
	public class Chore : Task
	{
		public string Description { get; set; }
		public Size size { get; set; }

		public enum Size
		{
			Small,
			Medium,
			Large,
		}
	}
}
