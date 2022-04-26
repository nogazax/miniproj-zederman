using dotnetProj.Models;

namespace dotnetProj.PeopleControllerHelper
{
	public static class PeopleValidator
	{
		public static bool PersonExists(string id, SqlContext context)
		{
			return context.People.Any(e => e.Id == id);
		}
	}
}