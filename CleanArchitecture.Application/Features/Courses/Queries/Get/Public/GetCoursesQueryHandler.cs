using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Courses.Queries.Get.Public
{
    public sealed class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, Response<List<CourseDto>>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public GetCoursesQueryHandler(ICourseRepository courseRepository,
                                      IStringLocalizer<SharedResources> localizer)
        {
            this._courseRepository = courseRepository;
            this._localizer = localizer;
        }


        public async Task<Response<List<CourseDto>>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
        {
            request.Pagination.Normalize();


            var (courses, totalCount) = await _courseRepository.GetCoursesAsync(
                request.Filter,
                request.Sorting,
                request.Pagination,
                cancellationToken);


            return ResponseHandler.SuccessPaged(
                courses,
                request.Pagination,
                totalCount,
                _localizer[Messages.RetrievedSuccessfully, _localizer[Entities.Courses]]);
        }
    }
}
