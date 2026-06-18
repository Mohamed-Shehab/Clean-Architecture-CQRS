using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
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
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentCourseRepository _studentCourseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public RemoveStudentFromCourseCommandHandler(ICourseRepository courseRepository,
                                                     IStudentRepository studentRepository,
                                                     IStudentCourseRepository studentCourseRepository,
                                                     IUnitOfWork unitOfWork,
                                                     IStringLocalizer<SharedResources> localizer)
        {
            this._courseRepository = courseRepository;
            this._studentRepository = studentRepository;
            this._studentCourseRepository = studentCourseRepository;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }

        public async Task<Response<object>> Handle(RemoveStudentFromCourseCommand request, CancellationToken cancellationToken)
        {
            // Check Is Course Exists
            var courseExists = await _courseRepository
                .AnyAsync(c => c.Id == request.CourseId, cancellationToken);

            if (!courseExists)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Course]]);


            // Check Is Student Exists
            var studentExists = await _studentRepository
                .AnyAsync(s => s.Id == request.StudentId, cancellationToken);

            if(!studentExists)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Student]]);


            // Check Is Student Assigned To Course
            var assignment = await _studentCourseRepository.GetAssignmentAsync(request.StudentId, request.CourseId, cancellationToken);
                

            if (assignment == null)
                return ResponseHandler.BadRequest<object>(_localizer[Errors.StudentNotAssigned]);


            // UnAssigned Course
            _studentCourseRepository.Delete(assignment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseHandler.Success<object>(message: _localizer[Messages.StudentRemovedSuccessfully]);
        }
    }
}
