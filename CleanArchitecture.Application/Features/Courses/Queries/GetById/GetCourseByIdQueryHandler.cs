using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Queries.GetById
{
    public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, Response<CourseDetailsDto>>
    {
        private readonly ICourseRepository _courseRepository;
        public GetCourseByIdQueryHandler(ICourseRepository courseRepository)
        {
            this._courseRepository = courseRepository;
        }

        public async Task<Response<CourseDetailsDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetCourseDetailsAsync(request.Id, cancellationToken);

            if (course == null)
                return ResponseHandler.NotFound<CourseDetailsDto>();

            return ResponseHandler.Success(course);
        }
    }
}
