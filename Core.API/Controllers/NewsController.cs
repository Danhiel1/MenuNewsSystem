using Core.Application.Common;
using Core.Application.Features.News.Commands;
using Core.Application.Features.News.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNews([FromBody] CreateNewsCommand command)
        {
            var newsId = await _mediator.Send(command);
            return Ok(ApiResponse<int>.Success(newsId, "Tạo News thành công!"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNews()
        {
            var newsList = await _mediator.Send(new GetAllNewsQuery());
            return Ok(ApiResponse<object>.Success(newsList));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsById(int id)
        {
            var query = new GetNewsByIdQuery { NewsId = id };
            var result = await _mediator.Send(query);
            if (result != null)
                return Ok(ApiResponse<object>.Success(result));

            return NotFound(ApiResponse.Fail("News không tồn tại!"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNews(int id, [FromBody] UpdateNewsCommand command)
        {
            if (id != command.Id)
                return BadRequest(ApiResponse.Fail("ID không khớp!"));

            var isSuccess = await _mediator.Send(command);
            if (isSuccess)
                return Ok(ApiResponse.Success("Cập nhật News thành công!"));

            return NotFound(ApiResponse.Fail("News không tồn tại!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var command = new DeleteNewsCommand { Id = id };
            var isSuccess = await _mediator.Send(command);
            if (isSuccess)
                return Ok(ApiResponse.Success("Xóa News thành công!"));

            return NotFound(ApiResponse.Fail("News không tồn tại!"));
        }
    }
}
