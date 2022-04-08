using dotnetProj.Models;

namespace dotnetProj.PeopleControllerHelper
{
	public static class PeopleValidator
	{
		public static bool PersonExists(string id, MyDatabaseContext context)
		{
			return context.People.Any(e => e.Id == id);
		}
	}
}