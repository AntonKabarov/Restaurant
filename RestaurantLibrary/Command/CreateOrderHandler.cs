using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantLibrary.Dto;
using RestaurantLibrary.Services;
using RestaurantLibrary.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantLibrary.Command
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
    {
        private readonly ILogger<CreateOrderHandler> _logger;
        private readonly IOrderService _orderService;

        public CreateOrderHandler(ILogger<CreateOrderHandler> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken token)
        {
            _logger.LogInformation("Создание заказа для стола: {Table}", request.Order.TableNumber);

            // Валидация
            ValidateOrder(request.Order);

            // Создание заказа
            var order = await _orderService.CreateOrderAsync(request.Order);

            return new OrderResponse(order.Id, order.TotalAmount, order.Status);
        }

        private void ValidateOrder(OrderData order)
        {
            if (order.Items.Count == 0)
                throw new ArgumentException("Заказ должен содержать позиции");

            if (!order.IsDelivery && string.IsNullOrWhiteSpace(order.TableNumber))
                throw new ArgumentException("Укажите номер стола");

            if (order.IsDelivery)
            {
                if (string.IsNullOrWhiteSpace(order.DeliveryAddress))
                    throw new ArgumentException("Укажите адрес доставки");
                if (string.IsNullOrWhiteSpace(order.CustomerPhone))
                    throw new ArgumentException("Укажите телефон клиента");
            }
        }
    }
}
