using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetStudentEnrollments;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetStudentEnrollments.Models;
using CleanArchitecture.Application.Features.Students.Commands.Delete;
using CleanArchitecture.Application.Features.Students.Commands.Update;
using CleanArchitecture.Application.Features.Students.Queries.Get;
using CleanArchitecture.Application.Features.Students.Queries.Get.Models;
using CleanArchitecture.Application.Features.Students.Queries.GetById;
using CleanArchitecture.WebApi.Controllers.Base;
using CleanArchitecture.WebApi.Swagger.Examples.Students;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace CleanArchitecture.WebApi.Controllers.Admin
{
    /// <summary>
    /// Provides administrative endpoints for managing students.
    /// </summary>
    [Route("api/admin/students")]

    public class AdminStudentsController : BaseApiController
    {

        public AdminStudentsController(IMediator mediator) : base(mediator)
        {
        }


        #region Get Students
        /// <summary>
        /// Retrieves a paginated list of students.
        /// </summary>
        ///
        /// <remarks>
        /// Supports pagination, filtering, and sorting.<br/>
        ///
        /// Available Filters:
        /// - Full name
        /// - Email
        /// - Phone number <br/>
        ///
        /// Available Sorting:
        /// - Id
        /// - Full name
        /// - Date of birth
        /// </remarks>
        ///
        /// <param name="pagination">
        /// Pagination information.
        /// </param>
        /// <param name="filter">
        /// Optional filtering criteria.
        /// </param>
        /// <param name="sorting">
        /// Optional sorting options.
        /// </param>
        ///
        /// <response code="200">
        /// Students retrieved successfully.
        /// </response>
        /// <response code="400">
        /// The request contains invalid query parameters.
        /// </response>
        
        [HttpGet]
        [ProducesResponseType<Response<List<StudentDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStudents([FromQuery] PaginationModel pagination,
                                                     [FromQuery] StudentFilterModel? filter,
                                                     [FromQuery] StudentSortingModel? sorting,
                                                     CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetStudentsQuery(pagination, filter, sorting), cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Get Student By Id
        /// <summary>
        /// Retrieves a student by its identifier.
        /// </summary>
        ///
        /// <remarks>
        /// Returns the student's profile information.
        /// </remarks>
        ///
        /// <param name="id">
        /// The unique identifier of the student.
        /// </param>
        ///
        /// <response code="200">
        /// Student retrieved successfully.
        /// </response>
        /// <response code="404">
        /// The specified student was not found.
        /// </response>
        
        [HttpGet("{id}")]
        [ProducesResponseType<Response<StudentDetailsDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentById(int id,
                                                        CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetStudentByIdQuery(id), cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Update Student
        /// <summary>
        /// Updates an existing student.
        /// </summary>
        /// 
        /// <remarks>
        /// Updates the student's profile information.
        /// 
        /// Validation Rules:
        /// - Student must exist.
        /// - Phone number must be unique.
        /// </remarks>
        /// 
        /// <param name="id">
        /// The unique identifier of the student.
        /// </param>
        /// <param name="command">
        /// The updated student information.
        /// </param>
        /// 
        /// <response code="200">
        /// The student was updated successfully.
        /// </response>
        /// <response code="400">
        /// Validation failed.
        /// </response>
        /// <response code="404">
        /// The specified student was not found.
        /// </response>
        /// <response code="409">
        /// The phone number is already in use.
        /// </response>
        
        [HttpPut("{id}")]
        [SwaggerRequestExample(typeof(UpdateStudentCommand), typeof(UpdateStudentCommandExample))]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateStudent(int id,
                                                       UpdateStudentCommand command,
                                                       CancellationToken cancellationToken)
        {
            command = command with { Id = id };

            var response = await _mediator.Send(command, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Delete Student
        /// <summary>
        /// Deletes an existing student.
        /// </summary>
        ///
        /// <remarks>
        /// Validation Rules:
        ///
        /// - Student must exist.
        /// </remarks>
        ///
        /// <param name="id">
        /// The unique identifier of the student.
        /// </param>
        ///
        /// <response code="200">
        /// Student deleted successfully.
        /// </response>
        /// <response code="400">
        /// The deletion request could not be completed.
        /// </response>
        /// <response code="404">
        /// The specified student was not found.
        /// </response>
       
        [HttpDelete("{id}")]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudent(int id,
                                                       CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteStudentCommand(id), cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Get Student's Course Enrollments
        /// <summary>
        /// Retrieves the courses in which the specified student is enrolled.
        /// </summary>
        ///
        /// <remarks>
        /// Returns a paginated list of the student's course enrollments.
        ///
        /// Optional filtering:
        /// - Course name.
        /// - Enrollment status.
        ///
        /// Supports sorting by:
        /// - Course name.
        /// - Enrollment date.
        /// - Enrollment status.
        /// </remarks>
        ///
        /// <param name="studentId">
        /// The unique identifier of the student.
        /// </param>
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
        /// Student enrollments retrieved successfully.
        /// </response>
        ///
        /// <response code="404">
        /// The specified student was not found.
        /// </response>
       
        [HttpGet("{studentId}/courses")]
        [ProducesResponseType<Response<List<StudentEnrollmentDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentEnrollments(int studentId,
                                                               [FromQuery] PaginationModel pagination,
                                                               [FromQuery] StudentEnrollmentFilterModel? filter,
                                                               [FromQuery] StudentEnrollmentSortingModel? sorting,
                                                               CancellationToken cancellationToken)
        {
            var query = new GetStudentEnrollmentsQuery(
                studentId,
                pagination,
                filter,
                sorting
            );

            var response = await _mediator.Send(query, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


    }
}
