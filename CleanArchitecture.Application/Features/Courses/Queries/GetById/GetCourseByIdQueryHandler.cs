using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.DTOs;
using CleanArchitecture.Infrastructure.Persistence.Context;
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
        private readonly AppDbContext _context;
        public GetCourseByIdQueryHandler(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<Response<CourseDetailsDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _context.Courses
                .Where(c => c.Id == request.Id)
                .Select(c => new CourseDetailsDto
                {
                    Id = c.Id,
                    Name = c.Title,
                    CourseCapacity = c.MaxStudents
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (course == null)
                return ResponseHandler.NotFound<CourseDetailsDto>();

            return ResponseHandler.Success(course);
        }
    }
}
