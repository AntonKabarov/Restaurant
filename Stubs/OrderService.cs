using RestaurantLibrary.Dto;
using RestaurantLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantLibrary.Stubs
{
    public interface IOrderService
    {
        Task<OrderResult> CreateOrderAsync(OrderData order);
    }
   
    public class OrderService : IOrderService
    {
        public Task<OrderResult> CreateOrderAsync(OrderData order)
        {
            return Task.FromResult(new OrderResult(
                $"STUB_{Guid.NewGuid()}",
                order.Items.Sum(i => i.Quantity * i.Price),
                "STUB_STATUS",
                 DateTime.Now));
        }
    }
}
