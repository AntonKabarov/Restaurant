using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantLibrary.Dto
{
    // Основная команда SendOrder
    public record SendOrderCommand : IRequest<OrderResponse>
    {
        public string OrderId { get; init; }
        public List<OrderMenuItem> MenuItems { get; init; }
    }

    // Позиция в заказе
    public record OrderMenuItem
    {
        public string Id { get; init; }
        public decimal Quantity { get; init; }
    }

}
