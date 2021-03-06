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
			CreateMap<Models.Task, Chore>();
			CreateMap<Chore, Models.Task>();
			CreateMap<Models.Task, HomeWork>();
			CreateMap<HomeWork, Models.Task>();
		}
	}
}
