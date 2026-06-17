using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Students.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Students.Queries.GetById
{
    public sealed record GetStudentByIdQuery(int Id) : IRequest<Response<StudentDto>>
    {
    }
}
