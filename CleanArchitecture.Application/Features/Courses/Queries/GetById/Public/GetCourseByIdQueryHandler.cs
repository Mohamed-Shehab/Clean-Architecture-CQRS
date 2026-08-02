using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Courses.Queries.GetById.Public
{
    public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, Response<CourseDetailsDto>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public GetCourseByIdQueryHandler(ICourseRepository courseRepository,
                                         IStringLocalizer<SharedResources> localizer)
        {
            this._courseRepository = courseRepository;
            this._localizer = localizer;
        }


        public async Task<Response<CourseDetailsDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetCourseByIdAsync(request.Id, cancellationToken);

            if (course == null)
            {
                return ResponseHandler.NotFound<CourseDetailsDto>(
                    _localizer[Messages.NotFound, _localizer[Entities.Course]],
                    errorCode: ErrorCodes.Course.NotFound);
            }


            return ResponseHandler.Success(
                course, 
                _localizer[Messages.RetrievedSuccessfully, _localizer[Entities.Course]]);
        }
    }
}
