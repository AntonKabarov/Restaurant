using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantLibrary.Dto;
using RestaurantLibrary.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantLibrary.Command
{
    public class SendOrderHandler : IRequestHandler<SendOrderCommand, OrderResponse>
    {
        private readonly ILogger<SendOrderHandler> _logger;
        private readonly IMenuService _menuService;

        public SendOrderHandler(
            ILogger<SendOrderHandler> logger,
            IMenuService menuService)
        {
            _logger = logger;
            _menuService = menuService;
        }

        public async Task<OrderResponse> Handle(
            SendOrderCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Обработка SendOrder для OrderId: {OrderId}", request.OrderId);

            // 1. Валидация
            if (request.MenuItems == null || request.MenuItems.Count == 0)
            {
                throw new ArgumentException("Добавьте хотя бы одно блюдо в заказ");
            }

            // 2. Проверка существования блюд и расчет суммы
            decimal totalAmount = 0;
            foreach (var item in request.MenuItems)
            {
                var dish = await _menuService.GetDishByIdAsync(item.Id);
                if (dish == null)
                {
                    throw new ArgumentException($"Блюдо с ID {item.Id} не найдено");
                }

                totalAmount += dish.Price * item.Quantity;
            }

            // 3. Логика создания/обновления заказа
            // (здесь может быть сохранение в БД, вызов внешнего API и т.д.)
            _logger.LogInformation("Заказ {OrderId} обработан. Сумма: {TotalAmount}",
                request.OrderId, totalAmount);

            // 4. Возвращаем ответ
            return new OrderResponse(
                OrderId: request.OrderId,
                TotalAmount: totalAmount,
                Status: "Принят в обработку");
        }
    }
}
