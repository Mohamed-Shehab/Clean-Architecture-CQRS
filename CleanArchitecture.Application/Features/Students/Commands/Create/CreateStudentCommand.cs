using CleanArchitecture.Application.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Students.Commands.Create
{
    public sealed record CreateStudentCommand(string Name, string Email, string Password) : IRequest<Response<int>>
    {
    }
}
