using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.DTOs;
using Core.Application.Interfaces;
using MediatR;

namespace Core.Application.Interfaces
{
    public interface IMenuRepository 
    {
        Task<int> CreateMenuAsync(Menu menu);
        Task<IEnumerable<Menu>> GetAllMenusAsync();
        Task<Menu?> GetByIdAsync(int id);
        Task<bool> UpdateMenuAsync(Menu menu);
        Task<bool> DeleteMenuAsync(int id);
        Task<Menu?> GetMenuWithNewsAsync(int menuId);
        Task<bool> IsNameUniqueAsync(string name);
    }

}
