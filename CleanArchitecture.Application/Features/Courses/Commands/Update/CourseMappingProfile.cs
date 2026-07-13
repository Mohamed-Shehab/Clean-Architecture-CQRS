using AutoMapper;
using CleanArchitecture.Application.Features.Courses.Commands.Update;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Features.Courses.Mapping
{
    public class CourseMappingProfile : Profile
    {
        public CourseMappingProfile()
        {
            CreateMap<UpdateCourseCommand, Course>()
                .ForMember(c => c.Id, opt => opt.Ignore());
        }
    }
}
