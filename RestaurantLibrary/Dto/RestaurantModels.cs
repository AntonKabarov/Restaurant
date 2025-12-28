using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantLibrary.Dto
{
    public record Request(string Command, object CommandParameters);

    public record OrderData(
      string TableNumber,
      string WaiterName,
      List<OrderItem> Items,
      string Comment = "",
      bool IsDelivery = false,
      string DeliveryAddress = "",
      string CustomerPhone = "");


    public record OrderItem(
     string DishId,
     decimal Quantity,
     decimal Price);


    public record Dish(
      string Id,
      string Article,
      string Name,
      decimal Price,
      bool IsWeighted = false,
      string FullPath = "",
      List<string> Barcodes = null);

    public record MenuResponse(List<Dish> MenuItems);
    public record OrderResponse(string OrderId, decimal TotalAmount, string Status);

}
