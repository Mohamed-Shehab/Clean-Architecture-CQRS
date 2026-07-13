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

namespace CleanArchitecture.Application.Features.Students.Commands.Create
{
    public sealed class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Response<int>>
    {
        private readonly IIdentityService _identityService;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CreateStudentCommandHandler(IIdentityService identityService,
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

        public async Task<Response<int>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            // Check if a user with the same email already exists
            var isEmailUsed = await _identityService.IsEmailUsedAsync(request.Email, cancellationToken);

            if(isEmailUsed)
                return ResponseHandler.Conflict<int>(_localizer[Errors.AlreadyUsed, Fields.Email]);


            // Check if a user with the same phone number already exists
            var isPhoneNumberUsed = await _identityService.IsPhoneNumberUsedAsync(request.PhoneNumber, cancellationToken);

            if(isPhoneNumberUsed)
                return ResponseHandler.Conflict<int>(_localizer[Errors.AlreadyUsed, Fields.PhoneNumber]);


            // Add new user to the identity system
            var createUserResult = await _identityService.CreateUserAsync(request.FirstName,
                                                                   request.LastName,
                                                                   request.Email,
                                                                   request.PhoneNumber,
                                                                   request.Password);

            if (!createUserResult.Succeeded)
                return ResponseHandler.Conflict<int>(_localizer[Messages.CreationFailed, _localizer[Entities.User]], errors: createUserResult.Errors);


            // Map the request to a Student entity and set the UserId
            var student = _mapper.Map<Student>(request);
            student.UserId = createUserResult.UserId;


            await _studentRepository.AddAsync(student, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return ResponseHandler.Created(student.Id, _localizer[Messages.CreatedSuccessfully, _localizer[Entities.Student]]);
        }
    }
}
