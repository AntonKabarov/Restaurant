using MediatR;
using RestaurantLibrary.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantLibrary.Command
{
    public record CreateOrderCommand(OrderData Order) : IRequest<OrderResponse>;

}
