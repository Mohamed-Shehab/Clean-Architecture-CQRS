using CleanArchitecture.Application.Common.DTOs;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.Commands.Create;
using CleanArchitecture.Application.Features.Courses.Commands.Delete;
using CleanArchitecture.Application.Features.Courses.Commands.Register;
using CleanArchitecture.Application.Features.Courses.Commands.Unregister;
using CleanArchitecture.Application.Features.Courses.Commands.Update;
using CleanArchitecture.Application.Features.Courses.DTOs;
using CleanArchitecture.Application.Features.Courses.Queries.Get;
using CleanArchitecture.Application.Features.Courses.Queries.GetById;
using CleanArchitecture.Application.Features.Courses.Queries.GetStudents;
using CleanArchitecture.WebApi.Controllers.Base;
using CleanArchitecture.WebApi.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebApi.Controllers
{
    [Route("api/[controller]")]

    public class CoursesController : BaseApiController
    {
        public CoursesController(IMediator mediator) : base(mediator)
        {
        }

        #region EndPoints

        [HttpGet]
        [ProducesResponseType<Response<List<CourseDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourses([FromQuery] PaginationParams pagination,
                                                    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetCoursesQuery(pagination), cancellationToken);

            return HandleResponse(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType<Response<CourseDetailsDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourseById(int id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetCourseByIdQuery(id), cancellationToken);

            return HandleResponse(response);
        }

        [HttpGet("{id}/students")]
        [ProducesResponseType<Response<List<CourseStudentDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourseStudentsByCourseId(int id,
                                                                     [FromQuery] PaginationParams pagination,
                                                                     [FromQuery] FilterParams? filter,
                                                                     [FromQuery] SortingParams? sorting,
                                                                     CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(
                new GetStudentsByCourseIdQuery(id, pagination, filter, sorting),
                cancellationToken
            );

            return HandleResponse(response);                                                                   
        }

        [HttpPost]
        [ProducesResponseType<Response<int>>(StatusCodes.Status201Created)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCourse(CreateCourseCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);

            if (!response.Succeeded)
                return HandleResponse(response);
            else
                return CreatedAtAction(nameof(GetCourseById), new { id = response.Data }, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCourse(int id, UpdateCourseCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
                return BadRequest();

            var response = await _mediator.Send(command, cancellationToken);

            return HandleResponse(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudent(int id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteCourseCommand(id), cancellationToken);

            return HandleResponse(response);
        }

        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [HttpPost("{courseId}/students")]
        public async Task<IActionResult> AssignStudent(int courseId, [FromBody] AssignStudentRequest request, CancellationToken cancellationToken)
        {
            var command = new AssignStudentToCourseCommand(courseId, request.StudentId);

            var response = await _mediator.Send(command, cancellationToken);

            return HandleResponse(response);
        }

        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [HttpDelete("{courseId}/students/{studentId}")]
        public async Task<IActionResult> UnAssignStudent(int courseId, int studentId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new RemoveStudentFromCourseCommand(courseId, studentId), cancellationToken);

            return HandleResponse(response);
        }

        #endregion
    }
}
