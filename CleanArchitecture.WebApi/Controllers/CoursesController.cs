using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Public;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Public.Modles;
using CleanArchitecture.Application.Features.Courses.Queries.GetById.Public;
using CleanArchitecture.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebApi.Controllers
{
    /// <summary>
    /// Provides public endpoints for browsing courses.
    /// </summary>
    [Route("api/[controller]")]

    public class CoursesController : BaseApiController
    {

        public CoursesController(IMediator mediator) : base(mediator)
        {
        }


        #region Get Courses
        /// <summary>
        /// Retrieves a paginated list of available courses.
        /// </summary>
        ///
        /// <remarks>
        /// Supports pagination, filtering, and sorting.
        ///
        /// Filtering:
        ///
        /// - Course name.
        /// - Availability.
        ///
        /// Sorting:
        ///
        /// - Id.
        /// - Name.
        /// - Available seats.
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
        [ProducesResponseType<Response<List<CourseDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCourses([FromQuery] PaginationModel pagination,
                                                    [FromQuery] CourseFilterModel? filter,
                                                    [FromQuery] CourseSortingModel? sorting,
                                                    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetCoursesQuery(pagination, filter, sorting), cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Get Course By Id
        /// <summary>
        /// Retrieves course details.
        /// </summary>
        ///
        /// <remarks>
        /// Returns detailed information about a specific course,
        /// including its description and seat availability.
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
        [ProducesResponseType<Response<CourseDetailsDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourseById(int id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetCourseByIdQuery(id), cancellationToken);


            return HandleResponse(response);
        }
        #endregion


    }
}
