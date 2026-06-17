using AutoMapper;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Students.Commands.Create
{
    public sealed class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Response<int>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CreateStudentCommandHandler(IStudentRepository studentRepository,
                                           IUnitOfWork unitOfWork,
                                           IMapper mapper,
                                           IStringLocalizer<SharedResources> localizer)
        {
            this._studentRepository = studentRepository;
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._localizer = localizer;
        }

        public async Task<Response<int>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            //Student student = new()
            //{
            //    Password = request.Password,
            //    Email = request.Email,
            //    Name = request.Name
            //};

            Student student = _mapper.Map<Student>(request);

            await _studentRepository.AddAsync(student, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseHandler.Created(student.Id, _localizer[Messages.CreatedSuccessfully, _localizer[Entities.Student]]);
        }
    }
}
