using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantLibrary.Dto;
using RestaurantLibrary.Services;
using RestaurantLibrary.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantLibrary.Command
{
    public class GetMenuHandler : IRequestHandler<GetMenuQuery, MenuResponse>
    {
        private readonly ILogger<GetMenuHandler> _logger;
        private readonly IMenuService _menuService;

        public GetMenuHandler(ILogger<GetMenuHandler> logger, IMenuService menuService)
        {
            _logger = logger;
            _menuService = menuService;
        }

        public async Task<MenuResponse> Handle(GetMenuQuery request, CancellationToken token)
        {
            _logger.LogInformation("Получение меню. WithPrice: {WithPrice}", request.WithPrice);

            var dishes = await _menuService.GetDishesAsync(request.WithPrice);
            return new MenuResponse(dishes);
        }
    }
}
