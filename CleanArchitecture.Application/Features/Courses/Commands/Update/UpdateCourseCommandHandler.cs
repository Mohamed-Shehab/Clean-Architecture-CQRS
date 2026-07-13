using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Courses.Commands.Update
{
    public sealed class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Response<int>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public UpdateCourseCommandHandler(ICourseRepository courseRepository, 
                                          IUnitOfWork unitOfWork,
                                          IMapper mapper,
                                          IStringLocalizer<SharedResources> localizer)
        {
            this._courseRepository = courseRepository;
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._localizer = localizer;
        }

        public async Task<Response<int>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            // Check is course exists
            var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);

            if (course == null)
                return ResponseHandler.NotFound<int>(_localizer[Messages.NotFound, _localizer[Entities.Course]]);


            var existingCourseId = await _courseRepository.GetIdByNameAsync(
                request.Id,
                request.NameEn,
                request.NameAr,
                cancellationToken);

            if (existingCourseId.HasValue)
            {
                return ResponseHandler.Conflict<int>(_localizer[Errors.AlreadyExists, _localizer[Entities.Course]],
                    data: existingCourseId.Value);
            }


            _mapper.Map(request, course);
            course.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return ResponseHandler.Success<int>(message: _localizer[Messages.UpdatedSuccessfully, _localizer[Entities.Course]]);
        }
    }
}
