using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
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

namespace CleanArchitecture.Application.Features.Courses.Commands.Unregister
{
    public sealed class RemoveStudentFromCourseCommandHandler : IRequestHandler<RemoveStudentFromCourseCommand, Response<object>>
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public RemoveStudentFromCourseCommandHandler(AppDbContext context,
                                                     IUnitOfWork unitOfWork,
                                                     IStringLocalizer<SharedResources> localizer)
        {
            this._context = context;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }

        public async Task<Response<object>> Handle(RemoveStudentFromCourseCommand request, CancellationToken cancellationToken)
        {
            // Check Is Course Exists
            var courseExists = await _context.Courses
                .AnyAsync(c => c.Id == request.CourseId, cancellationToken);

            if (!courseExists)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Course]]);


            // Check Is Student Exists
            var studentExists = await _context.Students
                .AnyAsync(s => s.Id == request.StudentId, cancellationToken);

            if(!studentExists)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Student]]);


            // Check Is Student Assigned To Course
            var studentAssigned = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.CourseId == request.CourseId
                                                && sc.StudentId == request.StudentId, cancellationToken);

            if (studentAssigned == null)
                return ResponseHandler.BadRequest<object>(_localizer[Errors.StudentNotAssigned]);


            // UnAssigned Course
            _context.StudentCourses.Remove(studentAssigned);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseHandler.Success<object>(message: _localizer[Messages.StudentRemovedSuccessfully]);
        }
    }
}
