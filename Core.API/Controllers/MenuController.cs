using Core.Application.Features.Menus.Commands;
using Core.Application.Features.Menus.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;

namespace Core.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenuController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuCommand command)
        {
            var menuId = await _mediator.Send(command);
            return Ok(new { Message = "Tạo Menu thành công!", Id = menuId });
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMenus()
        {
            var query = new GetAllMenusQuery();
            var menus = await _mediator.Send(query);
            return Ok(menus);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] UpdateMenuCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { Message = "ID không khớp!" });
            var isSuccess = await _mediator.Send(command);
            if (isSuccess)
                return Ok(new { Message = "Cập nhật Menu thành công!" });
            return NotFound(new { Message = "Menu không tồn tại!" });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var command = new DeleteMenuCommand { Id = id };
            var isSuccess = await _mediator.Send(command);
            if (isSuccess)
                return Ok(new { Message = "Xóa Menu thành công!" });
            return NotFound(new { Message = "Menu không tồn tại!" });
        }
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetMenuDetails(int id)
        {
            var query = new GetMenuWithNewsQuery { MenuId = id };
            var result = await _mediator.Send(query);
            if (result != null)
                return Ok(result);
            return NotFound(new { Message = "Menu không tồn tại!" });

        }
    }
}