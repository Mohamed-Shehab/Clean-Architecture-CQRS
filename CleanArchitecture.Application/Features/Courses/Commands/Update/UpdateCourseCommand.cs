using CleanArchitecture.Application.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Commands.Update
{
    public sealed record UpdateCourseCommand(int Id, string Title, int MaxStudents) : IRequest<Response<object>>
    {
    }
}
