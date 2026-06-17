using AutoMapper;
using CleanArchitecture.Application.Features.Students.Commands.Create;
using CleanArchitecture.Application.Features.Students.Commands.Update;
using CleanArchitecture.Application.Features.Students.DTOs;
using CleanArchitecture.Domain.Entities;


namespace CleanArchitecture.Application.Features.Students.Mapping
{
    public class StudentMappingProfile : Profile
    {
        public StudentMappingProfile()
        {
            CreateMap<Student, StudentDto>();
            CreateMap<CreateStudentCommand, Student>();
        }
    }
}
