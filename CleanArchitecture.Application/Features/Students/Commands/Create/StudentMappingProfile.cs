using AutoMapper;
using CleanArchitecture.Domain.Entities;


namespace CleanArchitecture.Application.Features.Students.Commands.Create
{
    public class StudentMappingProfile : Profile
    {
        public StudentMappingProfile()
        {
            CreateMap<CreateStudentCommand, Student>();
        }
    }
}
