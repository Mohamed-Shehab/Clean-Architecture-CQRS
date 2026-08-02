using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.DatabaseException;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Courses.Commands.Update
{
    public sealed class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Response<int>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IDatabaseExceptionService _databaseExceptionService;

        public UpdateCourseCommandHandler(ICourseRepository courseRepository, 
                                          IUnitOfWork unitOfWork,
                                          IMapper mapper,
                                          IStringLocalizer<SharedResources> localizer,
                                          IDatabaseExceptionService databaseExceptionService)
        {
            this._courseRepository = courseRepository;
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._localizer = localizer;
            this._databaseExceptionService = databaseExceptionService;
        }

        public async Task<Response<int>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            // Check is course exists
            var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);

            if (course == null)
            {
                return ResponseHandler.NotFound<int>(
                    _localizer[Messages.NotFound, _localizer[Entities.Course]],
                    errorCode: ErrorCodes.Course.NotFound);
            }

            
            if (request.Capacity < course.ActiveEnrollmentsCount)
            {
                return ResponseHandler.BadRequest<int>(
                    _localizer[Errors.CapacityLessThanActiveEnrollments],
                    errorCode: ErrorCodes.Course.CapacityLessThanActiveEnrollments);
            }


            var existingCourse = await _courseRepository.GetCourseByNameAsync(
                request.Id,
                request.NameEn,
                request.NameAr,
                cancellationToken);

            if (existingCourse != null)
            {
                if (existingCourse.IsDeleted)
                {
                    return ResponseHandler.Conflict<int>(
                        _localizer[Errors.NameReservedByDeletedEntity, _localizer[Entities.Course]],
                        errorCode: ErrorCodes.Course.NameReservedByDeletedEntity,
                        data: existingCourse.Id);
                }


                return ResponseHandler.Conflict<int>(
                    _localizer[Errors.AlreadyExists, _localizer[Entities.Course]],
                    errorCode: ErrorCodes.Course.AlreadyExists,
                    data: existingCourse.Id);

            }


            _mapper.Map(request, course);
            course.UpdatedAt = DateTime.UtcNow;


            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                var courseStillExists = await _courseRepository.AnyAsync(
                    c => c.Id == request.Id,
                    cancellationToken);

                if (!courseStillExists)
                {
                    return ResponseHandler.NotFound<int>(
                        _localizer[Messages.NotFound, _localizer[Entities.Course]],
                        errorCode: ErrorCodes.Course.NotFound);
                }


                return ResponseHandler.Conflict<int>(
                    _localizer[Errors.ConcurrencyConflict],
                    errorCode: ErrorCodes.Common.ConcurrencyConflict);
            }
            catch (DbUpdateException ex)
            {
                if (_databaseExceptionService.IsUniqueConstraintViolation(ex))
                {
                    return ResponseHandler.Conflict<int>(
                        _localizer[Errors.AlreadyExists, _localizer[Entities.Course]],
                        errorCode: ErrorCodes.Course.AlreadyExists);
                }

                throw;
            }


            return ResponseHandler.Success<int>(message: _localizer[Messages.UpdatedSuccessfully, _localizer[Entities.Course]]);
        }
    }
}
