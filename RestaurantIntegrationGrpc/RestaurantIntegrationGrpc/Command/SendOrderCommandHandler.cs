using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantLibrary.Command;
using RestaurantLibrary.Stubs;
using Sms.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIntegrationGrpc.Command
{
    public class SendOrderCommandHandler
     : IRequestHandler<SendOrderCommand, SendOrderResponse>  
    {
        private readonly ILogger<SendOrderHandler> _logger;

        public SendOrderCommandHandler(
          ILogger<SendOrderHandler> logger
         )
        {
            _logger = logger;
        }


        public async Task<SendOrderResponse> Handle(
            SendOrderCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("=== ОБРАБОТКА ЗАКАЗА ===");
            _logger.LogInformation("Order ID: {OrderId}", request.Order.Id);
            _logger.LogInformation("Items count: {Count}", request.Order.OrderItems.Count);

            foreach (var item in request.Order.OrderItems)
            {
                _logger.LogInformation("  Product: {Id}, Quantity: {Quantity}",
                    item.Id, item.Quantity);
            }

            return new SendOrderResponse
            {
                Success = true,
                ErrorMessage = "Заказ успешно обработан!"
            };
        }
    }
}
