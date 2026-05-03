using FluentValidation;

namespace Core.Application.Features.News.Commands
{
    public class CreateNewsCommandValidator : AbstractValidator<CreateNewsCommand>
    {
        public CreateNewsCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề không được để trống!")
                .MaximumLength(200).WithMessage("Tiêu đề không được vượt quá 200 ký tự!");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Nội dung không được để trống!");
        }
    }
}
