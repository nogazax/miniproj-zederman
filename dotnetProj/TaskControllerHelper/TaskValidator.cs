namespace dotnetProj.TaskControllerHelper
{
	public static class TaskValidator
	{
		public static bool IsValidTask(Models.Task task)
		{
			if (task.Type.Equals("Chore", StringComparison.OrdinalIgnoreCase)) //check chore fields are filled
			{
				return !IsChoreFieldsNull(task) && IsHomeworkFieldsNull(task);
			}
			//check homework fields are filled
			return IsChoreFieldsNull(task) && !IsHomeworkFieldsNull(task);
		}

		public static bool IsHomeworkFieldsNull(Models.Task task)
		{
			return string.IsNullOrEmpty(task.Course) || string.IsNullOrEmpty(task.DueDate) || string.IsNullOrEmpty(task.Details);
		}

		public static bool IsChoreFieldsNull(Models.Task task)
		{
			return string.IsNullOrEmpty(task.Description) || string.IsNullOrEmpty(task.Size);
		}
	}
}