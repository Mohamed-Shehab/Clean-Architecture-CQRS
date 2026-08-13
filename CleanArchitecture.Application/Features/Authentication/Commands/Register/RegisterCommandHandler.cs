using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.Identity;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.Register
{
    public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Response<object>>
    {
        private readonly IIdentityService _identityService;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public RegisterCommandHandler(IIdentityService identityService,
                                      IStudentRepository studentRepository,
                                      IUnitOfWork unitOfWork,
                                      IMapper mapper,
                                      IStringLocalizer<SharedResources> localizer)
        {
            this._identityService = identityService;
            this._studentRepository = studentRepository;
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._localizer = localizer;
        }


        public async Task<Response<object>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Check if a user with the same email already exists
            var isEmailUsed = await _identityService.IsEmailUsedAsync(request.Email, cancellationToken);

            if (isEmailUsed)
            {
                return ResponseHandler.Conflict<object>(
                    _localizer[Errors.AlreadyUsed, _localizer[Fields.Email]],
                    errorCode: ErrorCodes.Student.EmailAlreadyUsed);
            }


            // Check if a user with the same phone number already exists
            var isPhoneNumberUsed = await _identityService.IsPhoneNumberUsedAsync(request.PhoneNumber, cancellationToken);

            if (isPhoneNumberUsed)
            {
                return ResponseHandler.Conflict<object>(
                    _localizer[Errors.AlreadyUsed, _localizer[Fields.PhoneNumber]],
                    errorCode: ErrorCodes.Student.PhoneAlreadyUsed);
            }



            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Create Identity User
                var createUserResult = await _identityService.CreateUserAsync(request.FirstName,
                                                                                            request.LastName,
                                                                                            request.Email,
                                                                                            request.PhoneNumber,
                                                                                            request.Password);

                if (!createUserResult.Succeeded)
                {
                    return ResponseHandler.Conflict<object>(
                        _localizer[Messages.CreationFailed, _localizer[Entities.User]],
                        errorCode: ErrorCodes.Identity.UserCreationFailed,
                        errors: createUserResult.Errors);
                }


                // Create Student
                var student = _mapper.Map<Student>(request);
                student.UserId = createUserResult.UserId;


                await _studentRepository.AddAsync(student, cancellationToken);


                await _unitOfWork.SaveChangesAsync(cancellationToken);


                return ResponseHandler.Created<object>(message: _localizer[Messages.CreatedSuccessfully, _localizer[Entities.Student]]);
            },
            cancellationToken: cancellationToken);
        }
    }
}
