using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Courses.Queries.GetById.Management
{
    public sealed class GetCourseManagementByIdQueryHandler
        : IRequestHandler<GetCourseManagementByIdQuery, Response<CourseManagementDetailsDto>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public GetCourseManagementByIdQueryHandler(ICourseRepository courseRepository, 
                                                   IStringLocalizer<SharedResources> localizer)
        {
            this._courseRepository = courseRepository;
            this._localizer = localizer;
        }


        public async Task<Response<CourseManagementDetailsDto>> Handle(GetCourseManagementByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetCourseManagementByIdAsync(request.Id, cancellationToken);

            if (course == null)
                return ResponseHandler.NotFound<CourseManagementDetailsDto>(_localizer[Messages.NotFound, _localizer[Entities.Course]]);


            return ResponseHandler.Success(course, 
                _localizer[Messages.RetrievedSuccessfully, _localizer[Entities.Course]]);
        }
    }
}
