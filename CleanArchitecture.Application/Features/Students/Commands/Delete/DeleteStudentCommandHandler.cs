using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.Identity;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Students.Commands.Delete
{
    public sealed class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Response<object>>
    {
        private readonly IIdentityService _identityService;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public DeleteStudentCommandHandler(IIdentityService identityService,
                                           IStudentRepository studentRepository,
                                           IUnitOfWork unitOfWork,
                                           IStringLocalizer<SharedResources> localizer)
        {
            this._identityService = identityService;
            this._studentRepository = studentRepository;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }

        public async Task<Response<object>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);

            if (student == null)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Student]]);

            // Delete the associated user from the identity system
            var deleteUserResult = await _identityService.DeleteUserAsync(student.UserId, cancellationToken);

            if(!deleteUserResult.Succeeded)
                return ResponseHandler.BadRequest<object>(_localizer[Messages.DeletionFailed, _localizer[Entities.User]], deleteUserResult.Errors);

            // Delete the student
            _studentRepository.Delete(student);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseHandler.Success<object>(message: _localizer[Messages.DeletedSuccessfully, _localizer[Entities.Student]]);
        }
    }
}
