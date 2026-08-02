using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Courses.Commands.Delete
{
    public sealed class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, Response<object>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public DeleteCourseCommandHandler(ICourseRepository courseRepository,
                                          IUnitOfWork unitOfWork, 
                                          IStringLocalizer<SharedResources> localizer)
        {
            this._courseRepository = courseRepository;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }

        public async Task<Response<object>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);

            if (course == null)
            {
                return ResponseHandler.NotFound<object>(
                    _localizer[Messages.NotFound, _localizer[Entities.Course]],
                    errorCode: ErrorCodes.Course.NotFound);
            }

            var hasStudents = course.ActiveEnrollmentsCount > 0;

            if (hasStudents)
            {
                return ResponseHandler.Conflict<object>(
                    _localizer[Errors.DeletionNotAllowed, _localizer[Entities.Course]],
                    errorCode: ErrorCodes.Course.HasActiveEnrollments);
            }

            // Mark course as deleted
            course.IsDeleted = true;
            course.DeletedAt = DateTime.UtcNow;

            course.IsActive = false;


            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                var courseStillExists = await _courseRepository.AnyAsync(
                    c => c.Id == request.Id,
                    cancellationToken);

                if (!courseStillExists)
                {
                    return ResponseHandler.NotFound<object>(
                        _localizer[Messages.NotFound, _localizer[Entities.Course]],
                        errorCode: ErrorCodes.Course.NotFound);
                }


                return ResponseHandler.Conflict<object>(
                    _localizer[Errors.ConcurrencyConflict],
                    errorCode: ErrorCodes.Common.ConcurrencyConflict);
            }


            return ResponseHandler.Success<object>(message: _localizer[Messages.DeletedSuccessfully, _localizer[Entities.Course]]);
        }
    }
}
