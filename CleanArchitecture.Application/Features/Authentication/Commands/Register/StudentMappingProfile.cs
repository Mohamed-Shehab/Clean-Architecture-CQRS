using AutoMapper;
using CleanArchitecture.Domain.Entities;


namespace CleanArchitecture.Application.Features.Authentication.Commands.Register
{
    public class StudentMappingProfile : Profile
    {
        public StudentMappingProfile()
        {
            CreateMap<RegisterCommand, Student>();
        }
    }
}
