using Core.Application.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Features.Menus.Commands
{
    public class CreateMenuCommandValidator : AbstractValidator<CreateMenuCommand>
    {
        private readonly IMenuRepository _repository;
        public CreateMenuCommandValidator(IMenuRepository repository)
        {
            _repository = repository;
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên Menu không được để trống!")
                .MaximumLength(100).WithMessage("Tên Menu không được vượt quá 100 ký tự!")
                .MustAsync(BeUniqueName).WithMessage("Tên Menu đã tồn tại, vui lòng chọn tên khác!");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả Menu không được vượt quá 500 ký tự!");
        }
        // Hàm kiểm tra tên duy nhất
        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        {
            // Chúng ta cần bổ sung hàm IsNameUniqueAsync vào Repository
            return await _repository.IsNameUniqueAsync(name);
        }
    }
}
