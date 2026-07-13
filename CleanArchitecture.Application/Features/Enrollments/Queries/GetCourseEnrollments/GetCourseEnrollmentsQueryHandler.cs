using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Enrollments.Queries.GetCourseEnrollments
{
    public class GetCourseEnrollmentsQueryHandler : IRequestHandler<GetCourseEnrollmentsQuery, Response<List<CourseEnrollmentDto>>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public GetCourseEnrollmentsQueryHandler(ICourseRepository courseRepository,
                                                IEnrollmentRepository enrollmentRepository,
                                                IStringLocalizer<SharedResources> localizer)
        {
            this._courseRepository = courseRepository;
            this._enrollmentRepository = enrollmentRepository;
            this._localizer = localizer;
        }


        public async Task<Response<List<CourseEnrollmentDto>>> Handle(GetCourseEnrollmentsQuery request, CancellationToken cancellationToken)
        {
            // Normalize Pagination
            request.Pagination.Normalize();

            
            var isCourseExists = await _courseRepository.AnyAsync(c => c.Id == request.CourseId, cancellationToken);

            if (!isCourseExists)
                return ResponseHandler.NotFound<List<CourseEnrollmentDto>>(_localizer[Messages.NotFound, _localizer[Entities.Course]]);


            var (students, totalCount) = await _enrollmentRepository.GetCourseEnrollmentsAsync(
                request.CourseId,
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
