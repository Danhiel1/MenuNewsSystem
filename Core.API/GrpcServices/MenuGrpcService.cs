using Grpc.Core;
using MediatR;
using Core.API.Protos; 
using Core.Application.Features.Menus.Commands;
using Core.Application.Features.Menus.Queries;
namespace Core.API.GrpcServices
{
    public class MenuGrpcService : MenuService.MenuServiceBase
    {
        private readonly IMediator _mediator;

        public MenuGrpcService(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task<MenuResponse> CreateMenu(CreateMenuRequest request, ServerCallContext context)
        {
            var command = new CreateMenuCommand
            {
                Name = request.Name,
                Description = request.Description
            };

            var newId = await _mediator.Send(command);
            return new MenuResponse
            {
                Id = newId,
                Name = request.Name,
                Description = request.Description
            };
        }
        public override async Task<MenuResponse> GetMenu(GetMenuRequest request, ServerCallContext context)
        {
            var query = new GetMenuWithNewsQuery { MenuId = request.Id};
            var result = await _mediator.Send(query);

            if (result == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Menu {request.Id} not found"));
            return new MenuResponse
            {
                Id = result.Id,
                Name = result.Name,
                Description = result.Description
            };
        }
        public override async Task<GetAllMenusResponse> GetAllMenus(GetAllMenusRequest request, ServerCallContext context)
        {
            var query = new GetAllMenusQuery();
            var result = await _mediator.Send(query);
            var response = new GetAllMenusResponse();
            response.Menus.AddRange(result.Select(m => new MenuResponse
            {
                Id = m.Id,
                Name = m.Name ?? string.Empty,
                Description = string.Empty
            }));
            return response;
        }
        public override async Task<MenuResponse> UpdateMenu(UpdateMenuRequest request, ServerCallContext context)
        {
            var command = new UpdateMenuCommand
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description
            };
            var success = await _mediator.Send(command);
            if (!success)
                throw new RpcException(new Status(StatusCode.NotFound, $"Menu {request.Id} not found"));
            return new MenuResponse
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description
            };
        }
        public override async Task<DeleteMenuResponse> DeleteMenu(DeleteMenuRequest request, ServerCallContext context)
            {
                var command = new DeleteMenuCommand { Id = request.Id };
                var success = await _mediator.Send(command);
                return new DeleteMenuResponse { Success = success };
        }

    }
 }
