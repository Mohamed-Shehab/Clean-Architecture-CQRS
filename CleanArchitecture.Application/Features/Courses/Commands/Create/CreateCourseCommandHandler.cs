using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.DatabaseException;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Courses.Commands.Create
{
    public sealed class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Response<int>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IDatabaseExceptionService _databaseExceptionService;

        public CreateCourseCommandHandler(ICourseRepository courseRepository,
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

        public async Task<Response<int>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            // Check if a course with the same name already exists
            var existingCourse = await _courseRepository.GetCourseByNameAsync(
                request.NameEn,
                request.NameAr,
                cancellationToken);

            if (existingCourse != null)
            {
                // Restore deleted course
                if (existingCourse.IsDeleted)
                {
                    existingCourse.IsDeleted = false;
                    existingCourse.DeletedAt = null;

                    existingCourse.UpdatedAt = DateTime.UtcNow;

                    _mapper.Map(request, existingCourse);


                    await _unitOfWork.SaveChangesAsync(cancellationToken);


                    return ResponseHandler.Success(
                        existingCourse.Id,
                        _localizer[Messages.RestoredSuccessfully, _localizer[Entities.Course]]);
                }


                // Course already exists
                return ResponseHandler.Conflict(
                    _localizer[Errors.AlreadyExists, _localizer[Entities.Course]],
                    errorCode: ErrorCodes.Course.AlreadyExists,
                    data: existingCourse.Id);
            }


            

            // Creating a new course
            var course = _mapper.Map<Course>(request);
            course.CreatedAt = DateTime.UtcNow;

            await _courseRepository.AddAsync(course, cancellationToken);


            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return ResponseHandler.Conflict<int>(
                    _localizer[Errors.ConcurrencyConflict, _localizer[Entities.Course]],
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


            return ResponseHandler.Created(
                course.Id, 
                _localizer[Messages.CreatedSuccessfully, _localizer[Entities.Course]]);
        }
    }
}
