using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Commands.Register
{
    public sealed class AssignStudentToCourseCommandHandler : IRequestHandler<AssignStudentToCourseCommand, Response<object>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentCourseRepository _studentCourseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public AssignStudentToCourseCommandHandler(ICourseRepository courseRepository,
                                                   IStudentRepository studentRepository,
                                                   IStudentCourseRepository studentCourseRepository,
                                                   IUnitOfWork unitOfWork,
                                                   IStringLocalizer<SharedResources> localizer)
        {
            this._courseRepository = courseRepository;
            this._studentRepository = studentRepository;
            this._studentCourseRepository = studentCourseRepository;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }

        public async Task<Response<object>> Handle(AssignStudentToCourseCommand request, CancellationToken cancellationToken)
        {
            // Check Is Course Exists
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

            if(course == null)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Course]]);


            // Check Is Student Exists
            var studentExists = await _studentRepository.AnyAsync(s => s.Id == request.StudentId, cancellationToken);

            if(!studentExists)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Student]]);


            // Check Is Student Already Assigned To Course
            var alreadyAssigned = await _studentCourseRepository.IsAssignedAsync(request.StudentId, request.CourseId, cancellationToken);

            if (alreadyAssigned)
                return ResponseHandler.BadRequest<object>(_localizer[Errors.StudentAlreadyAssigned]);


            // Check Course Capacity
            var studentsCount = await _studentCourseRepository.GetStudentsCountAsync(request.CourseId, cancellationToken);
            
            if (studentsCount >= course.MaxStudents)
                return ResponseHandler.BadRequest<object>(_localizer[Errors.CourseIsFull]);


            // Assign Student To Course
            StudentCourse studentCourse = new StudentCourse
            {
                CourseId = request.CourseId,
                StudentId = request.StudentId,
                EnrolledAt = DateTime.UtcNow
            };

            await _studentCourseRepository.AddAsync(studentCourse, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseHandler.Success<object>(message: _localizer[Messages.StudentAssignedSuccessfully]);

        }
    }
}
