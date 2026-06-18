using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Commands.Update
{
    public sealed class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Response<object>>
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

        public async Task<Response<object>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);

            if (course == null)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Course]]);

            _mapper.Map(request, course);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseHandler.Success<object>(message: _localizer[Messages.UpdatedSuccessfully, _localizer[Entities.Course]]);
        }
    }
}
