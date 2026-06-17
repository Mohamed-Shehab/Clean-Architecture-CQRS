using CleanArchitecture.Application.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Commands.Unregister
{
    public sealed record RemoveStudentFromCourseCommand(int CourseId, int StudentId) : IRequest<Response<object>>
    {
    }
}
