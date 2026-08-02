using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Enrollments.Commands.Unenroll
{
    public sealed class UnenrollStudentCommandHandler : IRequestHandler<UnenrollStudentCommand, Response<object>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public UnenrollStudentCommandHandler(ICourseRepository courseRepository,
                                             IStudentRepository studentRepository,
                                             IEnrollmentRepository enrollmentRepository,
                                             IUnitOfWork unitOfWork,
                                             IStringLocalizer<SharedResources> localizer)
        {
            this._courseRepository = courseRepository;
            this._studentRepository = studentRepository;
            this._enrollmentRepository = enrollmentRepository;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }

        public async Task<Response<object>> Handle(UnenrollStudentCommand request, CancellationToken cancellationToken)
        {
            // Check Is Course Exists
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

            if (course == null)
            {
                return ResponseHandler.NotFound<object>(
                    _localizer[Messages.NotFound, _localizer[Entities.Course]],
                    errorCode: ErrorCodes.Course.NotFound);
            }


            // Check Is Student Exists
            var studentExists = await _studentRepository
                .AnyAsync(s => s.Id == request.StudentId, cancellationToken);

            if (!studentExists)
            {
                return ResponseHandler.NotFound<object>(
                    _localizer[Messages.NotFound, _localizer[Entities.Student]],
                    errorCode: ErrorCodes.Student.NotFound);
            }


            // Check Is Student Enrolled in the Course
            var enrollment = await _enrollmentRepository.GetEnrollmentAsync(request.StudentId, request.CourseId, cancellationToken);

            if (enrollment == null)
            {
                return ResponseHandler.Conflict<object>(
                    _localizer[Errors.NotEnrolled, _localizer[Entities.Student], _localizer[Entities.Course]],
                    errorCode: ErrorCodes.Enrollment.NotEnrolled);
            }


            switch (enrollment.Status)
            {
                case EnrollmentStatus.Dropped:

                    return ResponseHandler.Conflict<object>(
                        _localizer[Errors.NotEnrolled, _localizer[Entities.Student], _localizer[Entities.Course]],
                        errorCode: ErrorCodes.Enrollment.NotEnrolled);

                case EnrollmentStatus.Completed:

                    return ResponseHandler.Conflict<object>(
                        _localizer[Errors.AlreadyCompleted, _localizer[Entities.Student], _localizer[Entities.Course]],
                        errorCode: ErrorCodes.Enrollment.AlreadyCompleted);
            }


            // Mark enrollment as dropped
            enrollment.Status = EnrollmentStatus.Dropped;
            enrollment.DroppedAt = DateTime.UtcNow;

            course.ActiveEnrollmentsCount = Math.Max(0, course.ActiveEnrollmentsCount - 1);

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                var courseStillExists = await _courseRepository.AnyAsync(
                    c => c.Id == request.CourseId,
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


            return ResponseHandler.Success<object>(message: _localizer[Messages.UnenrolledSuccessfully, _localizer[Entities.Student]]);
        }
    }
}
