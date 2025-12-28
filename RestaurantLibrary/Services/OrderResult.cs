using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantLibrary.Services
{
    public record OrderResult(
        string Id,
        decimal TotalAmount,
        string Status,
        DateTime CreatedAt,
        string TableNumber = null,
        bool IsDelivery = false);
}
