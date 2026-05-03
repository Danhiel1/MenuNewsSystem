using FluentValidation;

namespace Core.Application.Features.News.Commands
{
    public class DeleteNewsCommandValidator : AbstractValidator<DeleteNewsCommand>
    {
        public DeleteNewsCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id News không hợp lệ! Id phải là số nguyên dương.");
        }
    }
}
