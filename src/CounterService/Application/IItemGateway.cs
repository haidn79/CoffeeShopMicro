using Contracts;

namespace CounterService.Application;

public interface IItemGateway
{
    Task<IEnumerable<ItemDto>> GetItemsByType(ItemType[] itemTypes);
}