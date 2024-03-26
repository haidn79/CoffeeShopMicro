using CounterService.Application.Commands;
using CounterService.Domain;
using Infrastructure.Domain;
using Infrastructure.Repository;
using MediatR;

namespace CounterService.Application.Handlers;

public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, IResult>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IItemGateway _itemGateway;
    private readonly IPublisher _publisher;

    public PlaceOrderHandler(IRepository<Order> orderRepository, IItemGateway itemGateway, IPublisher publisher)
    {
        _orderRepository = orderRepository;
        _itemGateway = itemGateway;
        _publisher = publisher;
    }

    public async Task<IResult> Handle(PlaceOrderCommand placeOrderCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(placeOrderCommand);

        var order = await Order.From(placeOrderCommand, _itemGateway);
        await _orderRepository.AddAsync(order, cancellationToken: cancellationToken);

        await order.RelayAndPublishEvents(_publisher, cancellationToken);

        return Results.Ok();
    }
}