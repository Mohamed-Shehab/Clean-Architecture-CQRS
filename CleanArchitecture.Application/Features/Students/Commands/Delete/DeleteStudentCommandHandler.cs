using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Students.Commands.Delete
{
    public sealed class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Response<object>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public DeleteStudentCommandHandler(IStudentRepository studentRepository,
                                           IUnitOfWork unitOfWork,
                                           IStringLocalizer<SharedResources> localizer)
        {
            this._studentRepository = studentRepository;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }

        public async Task<Response<object>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);

            if (student == null)
                return ResponseHandler.NotFound<object>(_localizer[Messages.NotFound, _localizer[Entities.Student]]);

            _studentRepository.Delete(student);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseHandler.Success<object>(message: _localizer[Messages.DeletedSuccessfully, _localizer[Entities.Student]]);
        }
    }
}
