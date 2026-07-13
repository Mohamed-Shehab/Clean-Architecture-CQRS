using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Domain.Enums;
using MediatR;
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
            var courseExists = await _courseRepository
                .AnyAsync(c => c.Id == request.CourseId, cancellationToken);

            if (!courseExists)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Course]]);


            // Check Is Student Exists
            var studentExists = await _studentRepository
                .AnyAsync(s => s.Id == request.StudentId, cancellationToken);

            if (!studentExists)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Student]]);


            // Check Is Student Enrolled in the Course
            var enrollment = await _enrollmentRepository.GetEnrollmentAsync(request.StudentId, request.CourseId, cancellationToken);
                
            if (enrollment == null)
                return ResponseHandler.Conflict<object>(_localizer[Errors.NotEnrolled, _localizer[Entities.Student], _localizer[Entities.Course]]);

            switch (enrollment.Status)
            {
                case EnrollmentStatus.Dropped:

                    return ResponseHandler.Conflict<object>(_localizer[Errors.NotEnrolled, _localizer[Entities.Student], _localizer[Entities.Course]]);

                case EnrollmentStatus.Completed:

                    return ResponseHandler.Conflict<object>(_localizer[Errors.AlreadyCompleted, _localizer[Entities.Student], _localizer[Entities.Course]]);
            }


            // Mark enrollment as dropped
            enrollment.Status = EnrollmentStatus.Dropped; //_enrollmentRepository.Delete(enrollment);

            enrollment.DroppedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return ResponseHandler.Success<object>(message: _localizer[Messages.UnenrolledSuccessfully, _localizer[Entities.Student]]);
        }
    }
}
