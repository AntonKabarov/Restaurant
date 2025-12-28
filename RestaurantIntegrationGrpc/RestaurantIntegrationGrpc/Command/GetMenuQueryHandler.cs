using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantIntegrationGrpc.Domain;
using Sms.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIntegrationGrpc.Command
{
    
    public class GetMenuQueryHandler : IRequestHandler<GetMenuQuery, GetMenuResponse>
    {
        private readonly IMenuRepository _repository;
        private readonly ILogger<GetMenuQueryHandler> _logger; // Добавим логгер

        public GetMenuQueryHandler(
            IMenuRepository repository,
            ILogger<GetMenuQueryHandler> logger = null)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger;
        }

        public async Task<GetMenuResponse> Handle(
            GetMenuQuery request,
            CancellationToken cancellationToken)
        {
            _logger?.LogInformation("GetMenuQueryHandler called: IncludeAll={IncludeAll}",
                request.IncludeAll);

            try
            {
                // 1. Получаем данные
                var menuItems = await _repository.GetMenuItemsAsync(
                    request.IncludeAll,
                    cancellationToken);

                // 2. Применяем бизнес-логику
                var availableItems = menuItems.Where(item => item.IsAvailable).ToList();

                _logger?.LogInformation("Found {Total} items, {Available} available",
                    menuItems.Count, availableItems.Count);

                // 3. Конвертируем в gRPC ответ
                return new GetMenuResponse
                {
                    Success = true,
                    MenuItems = { availableItems.Select(item => MapToGrpc(item)) }
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetMenuQueryHandler");

                return new GetMenuResponse
                {
                    Success = false,
                    ErrorMessage = $"Error: {ex.Message}"
                };
            }
        }

        private Sms.Test.MenuItem MapToGrpc(Domain.MenuItem item)
        {
            return new Sms.Test.MenuItem
            {
                Id = item.Id,
                Article = item.Article,
                Name = item.Name,
                Price = (double)item.Price,
                IsWeighted = item.IsWeighted,
                FullPath = item.CategoryPath,
                Barcodes = { item.Barcodes }
            };
        }
    }
}