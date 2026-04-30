using Core.Application.Common;
using Core.Application.Features.Menus.Commands;
using Core.Application.Features.Menus.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// TẠI SAO dùng ApiResponse&lt;int&gt;?
        /// Vì CreateMenu trả về menuId (int). Frontend nhận:
        /// { "status": true, "message": "Tạo Menu thành công!", "data": 5 }
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuCommand command)
        {
            var menuId = await _mediator.Send(command);
            return Ok(ApiResponse<int>.Success(menuId, "Tạo Menu thành công!"));
        }

        /// <summary>
        /// TẠI SAO dùng ApiResponse&lt;object&gt;?
        /// Vì data là danh sách DTO — dùng object để linh hoạt.
        /// Frontend nhận: { "status": true, "data": [ {id:1, name:"..."}, ... ] }
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllMenus()
        {
            var menus = await _mediator.Send(new GetAllMenusQuery());
            return Ok(ApiResponse<object>.Success(menus));
        }

        /// <summary>
        /// TẠI SAO dùng ApiResponse (không có T)?
        /// Vì Update chỉ cần trả status + message, không cần data.
        /// Thành công: { "status": true, "message": "Cập nhật thành công!" }
        /// Thất bại:   { "status": false, "message": "Menu không tồn tại!" }
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] UpdateMenuCommand command)
        {
            if (id != command.Id)
                return BadRequest(ApiResponse.Fail("ID không khớp!"));

            var isSuccess = await _mediator.Send(command);
            if (isSuccess)
                return Ok(ApiResponse.Success("Cập nhật Menu thành công!"));

            return NotFound(ApiResponse.Fail("Menu không tồn tại!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var command = new DeleteMenuCommand { Id = id };
            var isSuccess = await _mediator.Send(command);
            if (isSuccess)
                return Ok(ApiResponse.Success("Xóa Menu thành công!"));

            return NotFound(ApiResponse.Fail("Menu không tồn tại!"));
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetMenuDetails(int id)
        {
            var query = new GetMenuWithNewsQuery { MenuId = id };
            var result = await _mediator.Send(query);
            if (result != null)
                return Ok(ApiResponse<object>.Success(result));

            return NotFound(ApiResponse.Fail("Menu không tồn tại!"));
        }
    }
}