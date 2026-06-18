using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Students.Commands.Update
{
    public sealed class Handler : IRequestHandler<UpdateStudentCommand, Response<object>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public Handler(IStudentRepository studentRepository, 
                       IUnitOfWork unitOfWork,
                       IMapper mapper,
                       IStringLocalizer<SharedResources> localizer)
        {
            this._studentRepository = studentRepository;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }

        public async Task<Response<object>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);

            if(student == null)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Student]]);

            student.Name = request.Name;
            student.Email = request.Email;

            if(!string.IsNullOrEmpty(request.Password))
                student.Password = request.Password;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseHandler.Success<object>(message: _localizer[Messages.UpdatedSuccessfully, _localizer[Entities.Student]]);
        }
    }
}
