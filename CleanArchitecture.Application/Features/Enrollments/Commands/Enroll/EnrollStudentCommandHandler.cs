using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.DatabaseException;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Enrollments.Commands.Enroll
{
    public sealed class EnrollStudentCommandHandler : IRequestHandler<EnrollStudentCommand, Response<object>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IDatabaseExceptionService _databaseExceptionService;


        public EnrollStudentCommandHandler(ICourseRepository courseRepository,
                                           IStudentRepository studentRepository,
                                           IEnrollmentRepository enrollmentRepository,
                                           IUnitOfWork unitOfWork,
                                           IStringLocalizer<SharedResources> localizer,
                                           IDatabaseExceptionService databaseExceptionService)
        {
            this._courseRepository = courseRepository;
            this._studentRepository = studentRepository;
            this._enrollmentRepository = enrollmentRepository;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
            this._databaseExceptionService = databaseExceptionService;
        }


        public async Task<Response<object>> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
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
            var studentExists = await _studentRepository.AnyAsync(s => s.Id == request.StudentId, cancellationToken);

            if (!studentExists)
            {
                return ResponseHandler.NotFound<object>(
                    _localizer[Messages.NotFound, _localizer[Entities.Student]],
                    errorCode: ErrorCodes.Student.NotFound);
            }



            // Check Is Course Active
            if (!course.IsActive)
            {
                return ResponseHandler.Conflict<object>(
                    _localizer[Errors.Inactive, _localizer[Entities.Course]],
                    errorCode: ErrorCodes.Course.NotActive);
            }



            // Get Student Enrollment
            var enrollment = await _enrollmentRepository.GetEnrollmentAsync(
                    request.StudentId,
                    request.CourseId,
                    cancellationToken);


            if (enrollment != null)
            {
                switch (enrollment.Status)
                {
                    case EnrollmentStatus.Active:

                        return ResponseHandler.Conflict<object>(
                            _localizer[Errors.AlreadyEnrolled, _localizer[Entities.Student], _localizer[Entities.Course]],
                            errorCode: ErrorCodes.Enrollment.AlreadyEnrolled);

                    case EnrollmentStatus.Completed:

                        return ResponseHandler.Conflict<object>(
                            _localizer[Errors.AlreadyCompleted, _localizer[Entities.Student], _localizer[Entities.Course]],
                            errorCode: ErrorCodes.Enrollment.AlreadyCompleted);

                }
            }


            // Check Course Capacity
            var hasAvailableSeat = course.Capacity > course.ActiveEnrollmentsCount;

            if (!hasAvailableSeat)
            {
                return ResponseHandler.Conflict<object>(
                    _localizer[Errors.Full, _localizer[Entities.Course], _localizer[Entities.Students]],
                    errorCode: ErrorCodes.Enrollment.CourseFull);
            }

            if (enrollment == null)
            {
                enrollment = new Enrollment
                {
                    StudentId = request.StudentId,
                    CourseId = request.CourseId,
                    EnrolledAt = DateTime.UtcNow,
                    Status = EnrollmentStatus.Active
                };


                await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
            }
            else
            {
                enrollment.Status = EnrollmentStatus.Active;
                enrollment.EnrolledAt = DateTime.UtcNow;
                enrollment.DroppedAt = null;
            }

            course.ActiveEnrollmentsCount++;

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
            catch (DbUpdateException ex)
            {
                if (_databaseExceptionService.IsUniqueConstraintViolation(ex))
                {
                    return ResponseHandler.Conflict<object>(
                        _localizer[Errors.AlreadyEnrolled, _localizer[Entities.Student], _localizer[Entities.Course]],
                        errorCode: ErrorCodes.Enrollment.AlreadyEnrolled);
                }

                throw;
            }


            return ResponseHandler.Success<object>(message: _localizer[Messages.EnrolledSuccessfully, _localizer[Entities.Student]]);

        }
    }
}
