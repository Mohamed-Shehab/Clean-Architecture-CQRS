using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.Commands.Create;
using CleanArchitecture.Application.Features.Courses.Commands.Delete;
using CleanArchitecture.Application.Features.Courses.Commands.Update;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Management;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Management.Models;
using CleanArchitecture.Application.Features.Courses.Queries.GetById.Management;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetCourseEnrollments;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetCourseEnrollments.Models;
using CleanArchitecture.WebApi.Controllers.Base;
using CleanArchitecture.WebApi.Swagger.Examples.Courses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace CleanArchitecture.WebApi.Controllers.Admin
{
    /// <summary>
    /// Administrative operations for managing courses.
    /// </summary>
    [Route("api/admin/courses")]

    public class AdminCoursesController : BaseApiController
    {

        public AdminCoursesController(IMediator mediator) : base(mediator)
        {
        }


        #region Get Courses
        /// <summary>
        /// Retrieves a paginated list of courses for management.
        /// </summary>
        ///
        /// <remarks>
        /// Supports pagination, filtering, and sorting.
        ///
        /// Filtering:
        /// - Course name.
        /// - Active status.
        ///
        /// Sorting:
        /// - Id.
        /// - Name.
        /// - Capacity.
        /// - Enrolled students.
        /// - Active status.
        /// </remarks>
        ///
        /// <param name="pagination">
        /// Pagination options.
        /// </param>
        ///
        /// <param name="filter">
        /// Optional filtering criteria.
        /// </param>
        ///
        /// <param name="sorting">
        /// Optional sorting options.
        /// </param>
        ///
        /// <response code="200">
        /// Courses retrieved successfully.
        /// </response>
        /// <response code="400">
        /// Invalid query parameters.
        /// </response>
        
        [HttpGet]
        [ProducesResponseType<Response<List<CourseManagementDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCourses([FromQuery] PaginationModel pagination,
                                                    [FromQuery] CourseManagementFilterModel? filter,
                                                    [FromQuery] CourseManagementSortingModel? sorting,
                                                    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetCoursesManagementQuery(pagination, filter, sorting), cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Get Course By Id
        /// <summary>
        /// Retrieves course details for management.
        /// </summary>
        ///
        /// <remarks>
        /// Returns detailed information about a specific course,
        /// including capacity, enrolled students, available seats,
        /// activity status, and audit timestamps.
        /// </remarks>
        ///
        /// <param name="id">
        /// The unique identifier of the course.
        /// </param>
        ///
        /// <response code="200">
        /// Course retrieved successfully.
        /// </response>
        /// <response code="400">
        /// Invalid course identifier.
        /// </response>
        /// <response code="404">
        /// The specified course was not found.
        /// </response>
        
        [HttpGet("{id}")]
        [ProducesResponseType<Response<CourseManagementDetailsDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourseById(int id, 
                                                       CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetCourseManagementByIdQuery(id), cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Create Course
        /// <summary>
        /// Creates a new course.
        /// </summary>
        ///
        /// <remarks>
        /// Validation Rules:
        ///
        /// - English name must be unique.
        /// - Arabic name must be unique.
        /// - Capacity must be greater than zero.
        /// </remarks>
        ///
        /// <param name="command">
        /// Course information.
        /// </param>
        ///
        /// <returns>
        /// Returns the created course identifier.
        /// </returns>
        ///
        /// <response code="201">
        /// Course created successfully.
        /// </response>
        /// <response code="400">
        /// Validation failed.
        /// </response>
        /// <response code="409">
        /// Another course with the same name already exists.
        /// </response>
        
        [HttpPost]
        [SwaggerRequestExample(typeof(CreateCourseCommand), typeof(CreateCourseCommandExample))]
        [ProducesResponseType<Response<int>>(StatusCodes.Status201Created)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<int>>(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateCourse(CreateCourseCommand command,
                                                      CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);

            if (response.Succeeded)
                return CreatedAtAction(nameof(GetCourseById), new { id = response.Data }, response);


            return HandleResponse(response);
        }
        #endregion


        #region Update Course
        /// <summary>
        /// Updates an existing course.
        /// </summary>
        ///
        /// <remarks>
        /// Validation Rules:
        ///
        /// - The course must exist.
        /// - English name must be unique.
        /// - Arabic name must be unique.
        /// - Capacity must be greater than zero.
        /// </remarks>
        ///
        /// <param name="id">
        /// The unique identifier of the course.
        /// </param>
        ///
        /// <param name="command">
        /// Updated course information.
        /// </param>
        ///
        /// <response code="200">
        /// Course updated successfully.
        /// </response>
        /// <response code="400">
        /// Validation failed.
        /// </response>
        /// <response code="409">
        /// Another course with the same name already exists.
        /// </response>
        /// <response code="404">
        /// The specified course was not found.
        /// </response>
        
        [HttpPut("{id}")]
        [SwaggerRequestExample(typeof(UpdateCourseCommand), typeof(UpdateCourseCommandExample))]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Response<int>>(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateCourse(int id, UpdateCourseCommand command, CancellationToken cancellationToken)
        {
            command = command with { Id = id };

            var response = await _mediator.Send(command, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Delete Course
        /// <summary>
        /// Deletes an existing course.
        /// </summary>
        ///
        /// <remarks>
        /// Validation Rules:
        ///
        /// - The course must exist.
        /// - The course cannot be deleted while students are enrolled in it.
        /// </remarks>
        ///
        /// <param name="id">
        /// The unique identifier of the course.
        /// </param>
        ///
        /// <response code="200">
        /// The course was deleted successfully.
        /// </response>
        /// <response code="400">
        /// Validation failed.
        /// </response>
        /// <response code="404">
        /// The specified course was not found.
        /// </response>
        /// <response code="409">
        /// The course cannot be deleted because it has enrolled students.
        /// </response>
        
        [HttpDelete("{id}")]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteCourse(int id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteCourseCommand(id), cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Get Course Enrollments
        /// <summary>
        /// Retrieves the students enrolled in a specific course.
        /// </summary>
        ///
        /// <remarks>
        /// Supports:
        ///
        /// - Pagination.
        /// - Filtering by student name, email, phone number, and enrollment status.
        /// - Sorting by student name, enrollment status, or enrollment date.
        /// </remarks>
        ///
        /// <param name="id">
        /// The unique identifier of the course.
        /// </param>
        ///
        /// <param name="pagination">
        /// Pagination options.
        /// </param>
        ///
        /// <param name="filter">
        /// Optional filtering options.
        /// </param>
        ///
        /// <param name="sorting">
        /// Optional sorting options.
        /// </param>
        ///
        /// <response code="200">
        /// Enrolled students retrieved successfully.
        /// </response>
        /// <response code="400">
        /// Validation failed.
        /// </response>
        /// <response code="404">
        /// The specified course was not found.
        /// </response>
        
        [HttpGet("{id}/students")]
        [ProducesResponseType<Response<List<CourseEnrollmentDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourseEnrollments(int id,
                                                              [FromQuery] PaginationModel pagination,
                                                              [FromQuery] CourseEnrollmentFilterModel? filter,
                                                              [FromQuery] CourseEnrollmentSortingModel? sorting,
                                                              CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetCourseEnrollmentsQuery(id, pagination, filter, sorting),
                cancellationToken);


            return HandleResponse(response);
        }
        #endregion


    }
}
