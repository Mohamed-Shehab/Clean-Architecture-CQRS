using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Students.Queries.GetById
{
    public sealed class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Response<StudentDetailsDto>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public GetStudentByIdQueryHandler(IStudentRepository studentRepository,
                                          IStringLocalizer<SharedResources> localizer)
        {
            this._studentRepository = studentRepository;
            this._localizer = localizer;
        }

        public async Task<Response<StudentDetailsDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetStudentDetailsAsync(request.Id,
                                                                                        cancellationToken);

            if (student == null)
            {
                return ResponseHandler.NotFound<StudentDetailsDto>(
                    _localizer[Messages.NotFound, _localizer[Entities.Student]],
                    errorCode: ErrorCodes.Student.NotFound);
            }


            return ResponseHandler.Success(
                student,
                _localizer[Messages.RetrievedSuccessfully, _localizer[Entities.Student]]);
        }
    }
}
