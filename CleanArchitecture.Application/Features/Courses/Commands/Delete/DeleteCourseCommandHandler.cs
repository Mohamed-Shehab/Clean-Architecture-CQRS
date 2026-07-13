using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Courses.Commands.Delete
{
    public sealed class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, Response<object>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public DeleteCourseCommandHandler(ICourseRepository courseRepository, 
                                          IEnrollmentRepository enrollmentRepository,
                                          IUnitOfWork unitOfWork, 
                                          IStringLocalizer<SharedResources> localizer)
        {
            this._courseRepository = courseRepository;
            this._enrollmentRepository = enrollmentRepository;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }

        public async Task<Response<object>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);

            if(course == null)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Course]]);


            var hasStudents = await _enrollmentRepository.HasStudentsAsync(request.Id, cancellationToken);

            if (hasStudents)
                return ResponseHandler.Conflict<object>(_localizer[Errors.DeletionNotAllowed, _localizer[Entities.Course]]);


            _courseRepository.Delete(course);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseHandler.Success<object>(message: _localizer[Messages.DeletedSuccessfully, _localizer[Entities.Course]]);
        }
    }
}
