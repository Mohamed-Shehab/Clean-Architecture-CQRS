using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Students.Queries.Get
{
    public sealed class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, Response<List<StudentDto>>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public GetStudentsQueryHandler(IStudentRepository studentRepository, IStringLocalizer<SharedResources> localizer)
        {
            this._studentRepository = studentRepository;
            this._localizer = localizer;
        }

        public async Task<Response<List<StudentDto>>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
        {
            // Pagination
            request.Pagination.Normalize();

            var (students, totalCount) = await _studentRepository.GetStudentsAsync(
                   request.Filter,
                   request.Sorting,
                   request.Pagination,
                   cancellationToken);


            return ResponseHandler.SuccessPaged(
                students,
                request.Pagination,
                totalCount,
                _localizer[Messages.RetrievedSuccessfully, _localizer[Entities.Students]]);
        }

    }
}
