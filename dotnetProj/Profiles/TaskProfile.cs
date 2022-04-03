using AutoMapper;
using dotnetProj.Models;

namespace dotnetProj.Profiles
{
    public class TaskProfile : Profile
    {
		public TaskProfile()
		{
			CreateMap<Models.Task, NoIdOwnerIdTask>();
			CreateMap<NoIdOwnerIdTask, Models.Task>();
		}
	}
}
