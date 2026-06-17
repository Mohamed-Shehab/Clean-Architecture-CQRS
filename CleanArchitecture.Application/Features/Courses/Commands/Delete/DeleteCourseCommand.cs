using CleanArchitecture.Application.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Commands.Delete
{
    public sealed record DeleteCourseCommand(int Id) : IRequest<Response<object>>
    {
    }
}
