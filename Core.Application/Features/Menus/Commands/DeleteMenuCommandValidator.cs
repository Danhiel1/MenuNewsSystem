using FluentValidation;

namespace Core.Application.Features.Menus.Commands
{
    public class DeleteMenuCommandValidator : AbstractValidator<DeleteMenuCommand>
    {
        public DeleteMenuCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id Menu không hợp lệ! Id phải là số nguyên dương.");
        }
    }
}
