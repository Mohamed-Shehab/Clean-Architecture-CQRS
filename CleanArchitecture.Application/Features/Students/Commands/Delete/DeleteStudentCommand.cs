using CleanArchitecture.Application.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Students.Commands.Delete
{
    public sealed record DeleteStudentCommand(int Id) : IRequest<Response<object>>
    {
    }
}
