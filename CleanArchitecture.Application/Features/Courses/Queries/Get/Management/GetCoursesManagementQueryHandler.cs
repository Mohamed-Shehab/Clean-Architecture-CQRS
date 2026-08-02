using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Courses.Queries.Get.Management
{
    public sealed class GetCoursesManagementQueryHandler : IRequestHandler<GetCoursesManagementQuery, Response<List<CourseManagementDto>>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public GetCoursesManagementQueryHandler(ICourseRepository courseRepository,
                                      IStringLocalizer<SharedResources> localizer)
        {
            _courseRepository = courseRepository;
            _localizer = localizer;
        }

        public async Task<Response<List<CourseManagementDto>>> Handle(GetCoursesManagementQuery request, CancellationToken cancellationToken)
        {
            request.Pagination.Normalize();

            var (courses, totalCount) = await _courseRepository.GetCoursesManagementAsync(request.Filter,
                                                                                                                  request.Sorting, 
                                                                                                                  request.Pagination, 
                                                                                                                  cancellationToken);


            // Success Response
            return ResponseHandler.SuccessPaged(
                courses,
                request.Pagination,
                totalCount,
                _localizer[Messages.RetrievedSuccessfully, _localizer[Entities.Courses]]
            );
        }
    }
}
