using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Students.DTOs;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Students.Queries.GetById
{
    public sealed class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Response<StudentDto>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public GetStudentByIdQueryHandler(IStudentRepository studentRepository, 
                                          IMapper mapper,
                                          IStringLocalizer<SharedResources> localizer)
        {
            this._studentRepository = studentRepository;
            this._mapper = mapper;
            this._localizer = localizer;
        }

        public async Task<Response<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);

            var x = _localizer[Messages.NotFound, _localizer[Entities.Student]];
            if (student == null)
                return ResponseHandler.NotFound<StudentDto>(x);

            var studentDto = _mapper.Map<StudentDto>(student);

            return ResponseHandler.Success<StudentDto>(studentDto);
        }
    }
}
