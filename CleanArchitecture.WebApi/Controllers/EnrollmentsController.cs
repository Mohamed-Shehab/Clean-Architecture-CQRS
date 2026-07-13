using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Enrollments.Commands.Enroll;
using CleanArchitecture.Application.Features.Enrollments.Commands.Unenroll;
using CleanArchitecture.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebApi.Controllers
{
    /// <summary>
    /// Manages student course enrollments.
    /// </summary>
    [Route("api/[controller]")]

    public class EnrollmentsController : BaseApiController
    {

        public EnrollmentsController(IMediator mediator) : base(mediator)
        {
        }


        #region Enroll Student
        /// <summary>
        /// Enrolls a student in a course.
        /// </summary>
        ///
        /// <remarks>
        /// Creates a new enrollment for the specified student and course.
        ///
        /// Validation Rules:
        ///
        /// - Student must exist.
        /// - Course must exist.
        /// - Course must be active.
        /// - Student must not already be actively enrolled.
        /// - Student must not have already completed the course.
        /// - The course must have available seats.
        /// </remarks>
        ///
        /// <param name="command">
        /// Enrollment information.
        /// </param>
        ///
        /// <response code="200">
        /// Student enrolled successfully.
        /// </response>
        /// <response code="400">
        /// Validation failed.
        /// </response>
        /// <response code="404">
        /// The specified student or course was not found.
        /// </response>
        /// <response code="409">
        /// The student is already enrolled, has already completed the course,
        /// or the course has no available seats.
        /// </response>

        [HttpPost]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> EnrollStudent(EnrollStudentCommand command,
                                                       CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Unenroll Student
        /// <summary>
        /// Unenrolls a student from a course.
        /// </summary>
        ///
        /// <remarks>
        /// Removes the student's active enrollment from the specified course.
        ///
        /// Validation Rules:
        ///
        /// - Student must exist.
        /// - Course must exist.
        /// - Student must be actively enrolled in the course.
        /// - Completed enrollments cannot be withdrawn.
        /// </remarks>
        ///
        /// <param name="command">
        /// Student and course information.
        /// </param>
        ///
        /// <response code="200">
        /// Student unenrolled successfully.
        /// </response>
        /// <response code="400">
        /// Validation failed.
        /// </response>
        /// <response code="404">
        /// The specified student or course was not found.
        /// </response>
        /// <response code="409">
        /// The student is not actively enrolled in the course or has already completed it.
        /// </response>
        
        [HttpDelete]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UnenrollStudent(UnenrollStudentCommand command,
                                                         CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


    }
}
