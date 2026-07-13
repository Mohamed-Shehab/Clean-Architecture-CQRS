using CleanArchitecture.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebApi.Controllers
{
    /// <summary>
    /// Provides endpoints for authenticated students to manage their own profile and enrollments.
    /// </summary>
    [Route("api/[controller]")]

    public class StudentsController : BaseApiController
    {

        public StudentsController(IMediator mediator) : base(mediator)
        {
        }


        //GET    /api/students/me

        //PUT    /api/students/me

        //GET    /api/students/me/courses
    }
}
