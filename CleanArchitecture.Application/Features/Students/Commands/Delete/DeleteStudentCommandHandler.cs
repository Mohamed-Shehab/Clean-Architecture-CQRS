using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.Identity;
using CleanArchitecture.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Students.Commands.Delete
{
    public sealed class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Response<object>>
    {
        private readonly IIdentityService _identityService;
        private readonly IStudentRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public DeleteStudentCommandHandler(IIdentityService identityService,
                                           IStudentRepository studentRepository,
                                           IEnrollmentRepository enrollmentRepository,
                                           IUnitOfWork unitOfWork,
                                           IStringLocalizer<SharedResources> localizer)
        {
            this._identityService = identityService;
            this._studentRepository = studentRepository;
            this._enrollmentRepository = enrollmentRepository;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }

        public async Task<Response<object>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);

            if (student == null)
            { 
                return ResponseHandler.NotFound<object>(
                    _localizer[Messages.NotFound, _localizer[Entities.Student]],
                    errorCode: ErrorCodes.Student.NotFound);
            }


            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Get student active enrollments
                var activeEnrollments = await _enrollmentRepository.GetStudentActiveEnrollmentsAsync(student.Id, cancellationToken);


                // Drop all active enrollments and decrease the enrolled students count for each course
                foreach (var enrollment in activeEnrollments)
                {
                    enrollment.Status = EnrollmentStatus.Dropped;
                    enrollment.DroppedAt = DateTime.UtcNow;


                    enrollment.Course.ActiveEnrollmentsCount = Math.Max(0, enrollment.Course.ActiveEnrollmentsCount - 1);
                }



                // Mark user as deleted
                var deleteUserResult = await _identityService.DeleteUserAsync(student.UserId, cancellationToken);

                if (!deleteUserResult.Succeeded)
                {
                    return ResponseHandler.Conflict<object>(
                        _localizer[Messages.DeletionFailed, _localizer[Entities.User]], 
                        errorCode: ErrorCodes.Identity.UserDeletionFailed,
                        errors: deleteUserResult.Errors);
                }


                // Mark student as deleted
                student.IsDeleted = true;


                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return ResponseHandler.Conflict<object>(
                        _localizer[Errors.ConcurrencyConflict],
                        errorCode: ErrorCodes.Common.ConcurrencyConflict);
                }


                return ResponseHandler.Success<object>(message: _localizer[Messages.DeletedSuccessfully, _localizer[Entities.Student]]);
            },
            cancellationToken: cancellationToken);
            
        }
    }
}
