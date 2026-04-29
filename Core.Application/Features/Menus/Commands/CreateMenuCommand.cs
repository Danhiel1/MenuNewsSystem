using Core.Application.Events;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MassTransit;
using MediatR;

namespace Core.Application.Features.Menus.Commands
{
    public class CreateMenuCommand : IRequest<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// TẠI SAO inject IUnitOfWork thay vì IMenuRepository?
    /// - IUnitOfWork.Repository&lt;Menu&gt;() đã có CRUD đầy đủ
    /// - SaveChanges qua UoW → quản lý tập trung
    /// - Transaction qua UoW → rollback được khi lỗi
    /// - Chỉ inject IMenuRepository khi cần query ĐẶC THÙ (GetMenuWithNewsAsync)
    /// 
    /// FLOW AN TOÀN (có transaction):
    ///   BeginTransaction → Add Menu → SaveChanges (pending) → Publish Event → Commit
    ///                                                          ↓ (nếu lỗi)
    ///                                                       Rollback → DB y nguyên
    /// 
    /// FLOW KHÔNG AN TOÀN (trước đây):
    ///   Add Menu → SaveChanges (ĐÃ COMMIT!) → Publish Event (FAIL!) → Menu đã lưu, event mất
    /// </summary>
    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateMenuCommandHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<int> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var menu = new Menu
                {
                    Name = request.Name,
                    Description = request.Description
                };

                var repo = _unitOfWork.Repository<Menu>();
                await repo.AddAsync(menu);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Publish event → Consumer sync sang MongoDB
                await _publishEndpoint.Publish(new MenuCreatedEvent
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    Description = menu.Description
                }, cancellationToken);

                // CHỈ commit khi TẤT CẢ thành công
                await _unitOfWork.CommitAsync(cancellationToken);
                return menu.Id;
            }
            catch
            {
                // BẤT KỲ lỗi nào → rollback → DB y nguyên
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw; // Re-throw để GlobalExceptionHandler xử lý
            }
        }
    }
}
