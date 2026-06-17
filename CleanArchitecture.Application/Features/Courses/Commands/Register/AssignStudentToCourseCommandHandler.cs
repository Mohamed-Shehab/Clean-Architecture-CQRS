using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Infrastructure.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Commands.Register
{
    public sealed class AssignStudentToCourseCommandHandler : IRequestHandler<AssignStudentToCourseCommand, Response<object>>
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public AssignStudentToCourseCommandHandler(AppDbContext context,
                                                   IUnitOfWork unitOfWork,
                                                   IStringLocalizer<SharedResources> localizer)
        {
            this._context = context;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }

        public async Task<Response<object>> Handle(AssignStudentToCourseCommand request, CancellationToken cancellationToken)
        {
            // Check Is Course Exists
            var course = await _context.Courses.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

            if(course == null)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Course]]);


            // Check Is Student Exists
            var studentExists = await _context.Students
                .AnyAsync(s => s.Id == request.StudentId, cancellationToken);

            if(!studentExists)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Student]]);


            // Check Is Student Already Assigned To Course
            var alreadyAssigned = await _context.StudentCourses
                .AnyAsync(sc => sc.CourseId == request.CourseId 
                                     && sc.StudentId == request.StudentId, cancellationToken);

            if (alreadyAssigned)
                return ResponseHandler.BadRequest<object>(_localizer[Errors.StudentAlreadyAssigned]);


            // Check Course Capacity
            var studentsCount = await _context.StudentCourses.
                CountAsync(sc => sc.CourseId == request.CourseId, cancellationToken);
            
            if (studentsCount >= course.MaxStudents)
                return ResponseHandler.BadRequest<object>(_localizer[Errors.CourseIsFull]);


            // Assign Student To Course
            StudentCourse studentCourse = new StudentCourse
            {
                CourseId = request.CourseId,
                StudentId = request.StudentId,
                EnrolledAt = DateTime.UtcNow
            };

            await _context.StudentCourses.AddAsync(studentCourse, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseHandler.Success<object>(message: _localizer[Messages.StudentAssignedSuccessfully]);

        }
    }
}
