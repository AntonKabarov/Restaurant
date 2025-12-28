using Microsoft.Extensions.Logging;
using RestaurantIntegrationGrpc.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIntegrationGrpc.Domain
{
    public interface IMenuRepository
    {
        Task<List<Domain.MenuItem>> GetMenuItemsAsync(bool includeAll, CancellationToken cancellationToken);


    }
    public class MenuRepository : IMenuRepository
    {
        private readonly ILogger<MenuRepository> _logger;

        public MenuRepository(ILogger<MenuRepository> logger)
        {
            _logger = logger;
        }

        public Task<List<Domain.MenuItem>> GetMenuItemsAsync(
            bool includeAll,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("MenuRepository.GetMenuItemsAsync: includeAll={IncludeAll}",
                includeAll);

            // Возвращаем тестовые данные
            var items = new List<Domain.MenuItem>
        {
            new Domain.MenuItem
            {
                Id = "1",
                Article = "TEST001",
                Name = "Test Product 1",
                Price = 100.50m,
                IsWeighted = false,
                CategoryPath = "Test/Category1",
                Barcodes = new List<string> { "1234567890123" },
                IsAvailable = true
            },
            new Domain.MenuItem
            {
                Id = "2",
                Article = "TEST002",
                Name = "Test Product 2",
                Price = 200.75m,
                IsWeighted = true,
                CategoryPath = "Test/Category2",
                Barcodes = new List<string> { "2345678901234" },
                IsAvailable = true
            }
        };

            if (!includeAll)
            {
                items = items.Where(item => item.IsAvailable).ToList();
            }

            return Task.FromResult(items);
        }
    }
}
