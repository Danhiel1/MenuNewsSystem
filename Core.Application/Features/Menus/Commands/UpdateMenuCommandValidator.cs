using Core.Application.Interfaces;
using FluentValidation;

namespace Core.Application.Features.Menus.Commands
{
    public class UpdateMenuCommandValidator : AbstractValidator<UpdateMenuCommand>
    {
        private readonly IMenuRepository _repository;

        public UpdateMenuCommandValidator(IMenuRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id Menu không hợp lệ!");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên Menu không được để trống!")
                .MaximumLength(100).WithMessage("Tên Menu không được vượt quá 100 ký tự!")
                .MustAsync(BeUniqueNameExcludingSelf).WithMessage("Tên Menu đã tồn tại, vui lòng chọn tên khác!");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả Menu không được vượt quá 500 ký tự!");
        }
        private async Task<bool> BeUniqueNameExcludingSelf(UpdateMenuCommand command, string name, CancellationToken cancellationToken)
        {
            return await _repository.IsNameUniqueAsync(name, excludeId: command.Id);
        }
    }
}
