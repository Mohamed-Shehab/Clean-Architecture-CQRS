using AutoMapper;
using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.DTOs;
using CleanArchitecture.Domain.Interfaces.Repositories;
using CleanArchitecture.Infrastructure.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Queries.Get
{
    public sealed class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, Response<List<CourseDto>>>
    {
        private readonly AppDbContext _context;
        public GetCoursesQueryHandler(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<Response<List<CourseDto>>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
        {
            // Pagination
            request.Pagination.Normalize();

            // Base query
            var query = _context.Courses.AsQueryable();

            // Total count BEFORE pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Get paged data with projection
            var courses = await query
                .OrderBy(c => c.Id)
                .Skip((request.Pagination.PageNumber - 1) * request.Pagination.PageSize)
                .Take(request.Pagination.PageSize)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Title,
                    StudentsCount = c.StudentCourses.Count()
                })
                .ToListAsync(cancellationToken);
                
            // Not Found
            if (!courses.Any())
                return ResponseHandler.NotFound<List<CourseDto>>();

            // Success Response
            return ResponseHandler.SuccessPaged(
                courses,
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                totalCount
            );
        }
    }
}
