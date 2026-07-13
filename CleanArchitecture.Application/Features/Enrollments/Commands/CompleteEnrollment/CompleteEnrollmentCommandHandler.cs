using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Enrollments.Commands.CompleteEnrollment
{
    public sealed class CompleteEnrollmentCommandHandler : IRequestHandler<CompleteEnrollmentCommand, Response<object>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CompleteEnrollmentCommandHandler(ICourseRepository courseRepository,
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


        public async Task<Response<object>> Handle(CompleteEnrollmentCommand request, CancellationToken cancellationToken)
        {
            // Check Is Course Exists
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

            if (course == null)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Course]]);


            // Check Is Student Exists
            var studentExists = await _studentRepository.AnyAsync(s => s.Id == request.StudentId, cancellationToken);

            if (!studentExists)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Student]]);


            // Get Student Enrollment
            var enrollment = await _enrollmentRepository.GetEnrollmentAsync(
                    request.StudentId,
                    request.CourseId,
                    cancellationToken);

            if (enrollment == null || enrollment.Status == EnrollmentStatus.Dropped)
                return ResponseHandler.BadRequest<object>(_localizer[Errors.NotEnrolled, _localizer[Entities.Student], _localizer[Entities.Course]]);

            if (enrollment.Status == EnrollmentStatus.Completed)
                return ResponseHandler.BadRequest<object>(_localizer[Errors.AlreadyCompleted, _localizer[Entities.Student], _localizer[Entities.Course]]);


            // Complete Enrollment
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompletedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return ResponseHandler.Success<object>(message: _localizer[Messages.CompletedSuccessfully, _localizer[Entities.Student], _localizer[Entities.Course]]);

        }
    }
}
