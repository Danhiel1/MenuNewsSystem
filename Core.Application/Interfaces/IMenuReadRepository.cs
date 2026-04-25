using Core.Application.ReadModels;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface IMenuReadRepository
    {
        Task<IEnumerable<ReadModels.MenuReadModel>> GetAllAsync();
        Task<MenuReadModel?> GetByIdAsync(int id);
    }
}
