using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Enrollments.Queries.GetStudentEnrollments
{
    public sealed class GetStudentEnrollmentsQueryHandler
        : IRequestHandler<GetStudentEnrollmentsQuery, Response<List<StudentEnrollmentDto>>>
    {
        private readonly IStudentRepository _studentRepository; 
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public GetStudentEnrollmentsQueryHandler(IStudentRepository studentRepository,  
                                             IEnrollmentRepository enrollmentRepository, 
                                             IStringLocalizer<SharedResources> localizer)
        {
            this._studentRepository = studentRepository;
            this._enrollmentRepository = enrollmentRepository;
            this._localizer = localizer;
        }

        public async Task<Response<List<StudentEnrollmentDto>>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
        {
            // Check Is Student Exists
            var isStudentExists = await _studentRepository
                .AnyAsync(s => s.Id == request.StudentId, cancellationToken);

            if (!isStudentExists)
            {
                return ResponseHandler.NotFound<List<StudentEnrollmentDto>>(
                    _localizer[Messages.NotFound, _localizer[Entities.Student]],
                    errorCode: ErrorCodes.Student.NotFound);
            }


            // Normalize Pagination
            request.Pagination.Normalize();

            var (enrollments, totalCount) = await _enrollmentRepository.GetStudentEnrollmentsAsync(
                request.StudentId,
                request.Filter,
                request.Sorting,
                request.Pagination,
                cancellationToken);


            return ResponseHandler.SuccessPaged(
                enrollments,
                request.Pagination,
                totalCount,
                _localizer[Messages.RetrievedSuccessfully, _localizer[Entities.Courses]]
            );
        }

    }
}
