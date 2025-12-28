using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIntegrationGrpc.Domain
{
    public class MenuItem
    {
        public string Id { get; set; } = string.Empty;
        public string Article { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }  
        public bool IsWeighted { get; set; }
        public string CategoryPath { get; set; } = string.Empty;
        public List<string> Barcodes { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public int Stock { get; set; }

        public bool IsAvailable { get; set; } = true;   

        public decimal CalculatePriceWithTax(decimal taxRate)
            => Price * (1 + taxRate);
    }
}
