namespace dotnetProj.TaskControllerHelper
{
	public static class TaskValidator
	{
		public static bool IsValidTask(Models.Task task)
		{
			if (task.Status == null) //mutual data memeber
			{
				return false;
			}
			if (task.Type.Equals("Chore", StringComparison.OrdinalIgnoreCase)) //check chore fields are filled
			{
				return !IsChoreFieldsNull(task) && IsHomeworkFieldsNull(task);
			}
			//check homework fields are filled
			return IsChoreFieldsNull(task) && !IsHomeworkFieldsNull(task);
		}

		public static bool IsHomeworkFieldsNull(Models.Task task)
		{
			return task.Course == null && task.DueDate == null && task.Details == null;
		}

		public static bool IsChoreFieldsNull(Models.Task task)
		{
			return task.Description == null && task.Size == null;
		}
	}
}