using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Courses.Commands.Create
{
    public sealed class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Response<int>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CreateCourseCommandHandler(ICourseRepository courseRepository,
                                          IUnitOfWork unitOfWork,
                                          IMapper mapper,
                                          IStringLocalizer<SharedResources> localizer)
        {
            this._courseRepository = courseRepository;
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._localizer = localizer;
        }

        public async Task<Response<int>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            // Check if a course with the same name already exists
            var existingCourseId = await _courseRepository.GetIdByNameAsync(
                request.NameEn,
                request.NameAr,
                cancellationToken);

            if (existingCourseId.HasValue)
            {
                return ResponseHandler.Conflict(_localizer[Errors.AlreadyExists, _localizer[Entities.Course]],
                    data: existingCourseId.Value);
            }
            

            // Creating a new course
            var course = _mapper.Map<Course>(request);
            course.CreatedAt = DateTime.UtcNow;

            await _courseRepository.AddAsync(course, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseHandler.Created(course.Id, _localizer[Messages.CreatedSuccessfully, _localizer[Entities.Course]]);
        }
    }
}
