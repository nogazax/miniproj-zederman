namespace BehindTheScenes
{
	public abstract class Task
	{
		public string Id { get; set; }

		public string OwnerId{ get; set; }

		public Status status{ get; set; }

		public enum Status
		{
			Active,
			Done
		}

		public bool SetOwner(string newOwnerId)
		{
			throw new NotImplementedException ();
		}

		public bool SetStatus(Status newStatus)
		{
			throw new NotImplementedException();
		}
	}
}