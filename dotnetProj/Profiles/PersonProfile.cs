using AutoMapper;
using dotnetProj.Models;

namespace dotnetProj.Profiles
{
    public class PersonProfile : Profile
    {
		public PersonProfile()
		{
			CreateMap<Person, NoIdPerson>();
			CreateMap<NoIdPerson, Person>();
			CreateMap<Person, NoTasksPerson>();
		}
	}
}
