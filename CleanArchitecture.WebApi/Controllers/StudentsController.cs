using CleanArchitecture.Application.Features.Students.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.WebApi.Controllers.Base;

using CleanArchitecture.Application.Features.Students.Queries.Get;
using CleanArchitecture.Application.Features.Students.Queries.GetById;
using CleanArchitecture.Application.Features.Students.Commands.Create;
using CleanArchitecture.Application.Features.Students.Commands.Delete;
using CleanArchitecture.Application.Features.Students.Commands.Update;
using CleanArchitecture.Application.Common.DTOs;
using CleanArchitecture.Application.Features.Students.Queries.GetCourses;

namespace CleanArchitecture.WebApi.Controllers
{
    [Route("api/[controller]")]

    public class StudentsController : BaseApiController
    {
        public StudentsController(IMediator mediator) : base(mediator)
        {
        }


        #region EndPoints

        [HttpGet]
        [ProducesResponseType<Response<List<StudentDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudents([FromQuery] PaginationParams pagination,
                                                     [FromQuery] FilterParams? filter,
                                                     [FromQuery] SortingParams? sorting,
                                                     CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetStudentsQuery(pagination, filter,sorting), cancellationToken);

            return HandleResponse(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType<Response<StudentDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentById(int id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetStudentByIdQuery(id), cancellationToken);

            return HandleResponse(response);
        }

        [HttpPost]
        [ProducesResponseType<Response<int>>(StatusCodes.Status201Created)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateStudent(CreateStudentCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);

            if(!response.Succeeded)
                return HandleResponse(response);
            else
                return CreatedAtAction(nameof(GetStudentById), new {id = response.Data}, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStudent(int id, UpdateStudentCommand command, CancellationToken cancellationToken)
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
            var response = await _mediator.Send(new DeleteStudentCommand(id), cancellationToken);

            return HandleResponse(response);
        }

        [HttpGet("{studentId}/courses")]
        [ProducesResponseType<Response<List<StudentCourseDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCoursesByStudent(int studentId,
                                                             [FromQuery] PaginationParams pagination,
                                                             [FromQuery] FilterParams? filter,
                                                             [FromQuery] SortingParams? sorting,
                                                             CancellationToken cancellationToken)
        {
            var query = new GetCoursesByStudentIdQuery(
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
