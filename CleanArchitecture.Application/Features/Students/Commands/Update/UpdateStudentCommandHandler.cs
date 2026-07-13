using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.Identity;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Students.Commands.Update
{
    public sealed class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Response<object>>
    {
        private readonly IIdentityService _identityService;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public UpdateStudentCommandHandler(IIdentityService identityService,
                       IStudentRepository studentRepository, 
                       IUnitOfWork unitOfWork,
                       IStringLocalizer<SharedResources> localizer)
        {
            this._identityService = identityService;
            this._studentRepository = studentRepository;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }

        public async Task<Response<object>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            // Validate existence
            var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);

            if(student == null)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Student]]);


            // Update Identity User
            var updateUserResult = await _identityService.UpdateUserAsync(student.UserId,
                                                                request.FirstName,
                                                                request.LastName,
                                                                request.PhoneNumber,
                                                                cancellationToken);

            if(!updateUserResult.Succeeded)
                return ResponseHandler.Conflict<object>(_localizer[Messages.UpdateFailed, _localizer[Entities.User]], errors: updateUserResult.Errors);


            // Update Student Entity
            student.DateOfBirth = request.DateOfBirth;
            student.Address = request.Address;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseHandler.Success<object>(message: _localizer[Messages.UpdatedSuccessfully, _localizer[Entities.Student]]);
        }
    }
}
