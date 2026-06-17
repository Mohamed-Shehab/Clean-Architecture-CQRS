using AutoMapper;
using CleanArchitecture.Application.Features.Courses.Commands.Create;
using CleanArchitecture.Application.Features.Courses.Commands.Update;
using CleanArchitecture.Application.Features.Courses.DTOs;
using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Mapping
{
    public class CourseMappingProfile : Profile
    {
        public CourseMappingProfile()
        {
            CreateMap<CreateCourseCommand, Course>();
            CreateMap<UpdateCourseCommand, Course>();
        }
    }
}
