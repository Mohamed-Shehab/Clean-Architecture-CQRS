using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using MediatR;
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

        public EnrollStudentCommandHandler(ICourseRepository courseRepository,
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

        public async Task<Response<object>> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
        {
            // Check Is Course Exists
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

            if(course == null)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Course]]);


            // Check Is Student Exists
            var studentExists = await _studentRepository.AnyAsync(s => s.Id == request.StudentId, cancellationToken);

            if(!studentExists)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Student]]);



            // Check Is Course Active
            if (!course.IsActive)
                return ResponseHandler.BadRequest<object>(_localizer[Errors.Inactive, _localizer[Entities.Course]]);    



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

                        return ResponseHandler.Conflict<object>(_localizer[Errors.AlreadyEnrolled, _localizer[Entities.Student], _localizer[Entities.Course]]);

                    case EnrollmentStatus.Completed:

                        return ResponseHandler.Conflict<object>(_localizer[Errors.AlreadyCompleted, _localizer[Entities.Student], _localizer[Entities.Course]]);

                }
            }


            // Check Course Capacity
            var hasAvailableSeat = await _enrollmentRepository.HasAvailableSeatAsync(request.CourseId, course.Capacity, cancellationToken);

            if (!hasAvailableSeat)
                return ResponseHandler.Conflict<object>(_localizer[Errors.Full, _localizer[Entities.Course], _localizer[Entities.Students]]);


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


            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return ResponseHandler.Success<object>(message: _localizer[Messages.EnrolledSuccessfully, _localizer[Entities.Student]]);

        }
    }
}
