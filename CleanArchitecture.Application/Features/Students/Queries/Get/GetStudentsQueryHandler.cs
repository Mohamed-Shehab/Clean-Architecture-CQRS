using AutoMapper;
using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Students.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq.Expressions;

namespace CleanArchitecture.Application.Features.Students.Queries.Get
{
    public sealed class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, Response<List<StudentDto>>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public GetStudentsQueryHandler(IStudentRepository studentRepository, IMapper mapper)
        {
            this._studentRepository = studentRepository;
            this._mapper = mapper;
        }

        public async Task<Response<List<StudentDto>>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
        {
            // Filtration
            Expression<Func<Student, bool>>? filter = null;
            if(!string.IsNullOrWhiteSpace(request.Filter?.Search))
            {
                var search = request.Filter.Search.Trim();

                filter = s => s.Name.Contains(search) || s.Email.Contains(search);
            }

            // Sorting
            Func<IQueryable<Student>, IOrderedQueryable<Student>>? orderBy = null;
            if(request.Sorting != null)
            {
                orderBy = request.Sorting.IsDescending 
                    ? q => q.OrderByDescending(GetSortingProperty(request))
                    : q => q.OrderBy(GetSortingProperty(request));
            }

            // Pagination
            request.Pagination.Normalize();

            // Get Paged Data
            var (students, totalCount) = await _studentRepository.GetPagedAsync(
                filter,
                orderBy,
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                cancellationToken
            );

            if(!students.Any())
                return ResponseHandler.NotFound<List<StudentDto>>();

            var studentsDto = _mapper.Map<List<StudentDto>>(students);

            return ResponseHandler.SuccessPaged(
                studentsDto,
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                totalCount
            );
        }

        private static Expression<Func<Student, object>> GetSortingProperty(GetStudentsQuery request)
        {

            return request.Sorting?.OrderBy?.Trim().ToLower() switch
            {
                "name" => s => s.Name,

                "email" => s => s.Email,

                _ => s => s.Id
            };
        }
    }
}
