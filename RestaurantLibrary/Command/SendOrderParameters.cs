using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestaurantLibrary.Command
{
    public record SendOrderParameters
    {
        public string OrderId { get; init; }
        public List<OrderMenuItem> MenuItems { get; init; }
    }
    public record OrderMenuItem
    {
        public string Id { get; init; }

        public decimal Quantity { get; init; }
    }

}
