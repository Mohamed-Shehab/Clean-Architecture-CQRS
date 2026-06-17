using CleanArchitecture.Application.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Students.Commands.Update
{
    public sealed record UpdateStudentCommand(int Id, string Name, string Email, string? Password) : IRequest<Response<object>>
    {
    }
}
