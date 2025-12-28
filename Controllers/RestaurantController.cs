using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RestaurantApi.Services;

namespace RestaurantApi.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize] //Basic 
    public class RestaurantController : Controller
    {
        private readonly ILogger<RestaurantController> _logger;
        private readonly IDataService _dataService;

        public RestaurantController(
            ILogger<RestaurantController> logger,
            IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        /// <summary>
        /// Основной endpoint для обработки всех JSON запросов
        /// </summary>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> HandleJsonRequest([FromBody] JObject requestJson)
        {
            try
            {
                _logger.LogInformation("Получен JSON запрос: {Request}",
                    JsonConvert.SerializeObject(requestJson, Formatting.Indented));

                // Динамически разбираем JSON
                dynamic request = requestJson;

                //string command = request?.Command?.Value<string>();

                var command = request?.Command.Value;

                if (string.IsNullOrEmpty(command))
                {
                    return CreateJsonErrorResponse("Unknown", "Команда не указана");
                }

                // Динамически обрабатываем команду
                switch (command)
                {
                    case "GetMenu":
                        return await HandleGetMenuJson(request);

                    case "CreateOrder":
                        return await HandleCreateOrderJson(request);

                    default:
                        return CreateJsonErrorResponse(command, $"Неизвестная команда: {command}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки JSON запроса");
                return CreateJsonErrorResponse("Unknown", $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработка GetMenu через JSON
        /// </summary>
        private async Task<IActionResult> HandleGetMenuJson(dynamic request)
        {
            try
            {
                // Динамически извлекаем параметры
                bool withPrice = true;

                if (request?.CommandParameters?.WithPrice != null)
                {
                    withPrice = request.CommandParameters.WithPrice.Value;
                }

                // Получаем данные из сервиса
                var menuItems = await _dataService.GetMenuItemsAsync();

                // Динамически создаем ответ
                dynamic response = new ExpandoObject();
                response.Command = "GetMenu";
                response.Success = true;
                response.ErrorMessage = "";

                // Динамически создаем Data
                dynamic data = new ExpandoObject();
                var menuList = new List<dynamic>();

                foreach (var item in menuItems)
                {
                    dynamic menuItem = new ExpandoObject();
                    menuItem.Id = item.Id;
                    menuItem.Article = item.Article;
                    menuItem.Name = item.Name;
                    menuItem.Price = withPrice ? item.Price : 0;
                    menuItem.IsWeighted = item.IsWeighted;
                    menuItem.FullPath = item.FullPath;
                    menuItem.Barcodes = item.Barcodes ?? new List<string>();
                    menuList.Add(menuItem);
                }

                data.MenuItems = menuList;
                response.Data = data;

                _logger.LogInformation("GetMenu обработан успешно. Блюд: {Count}", menuItems.Count);

                // Возвращаем как JSON
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки GetMenu");
                return CreateJsonErrorResponse("GetMenu", $"Ошибка получения меню: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработка CreateOrder через JSON
        /// </summary>
        private async Task<IActionResult> HandleCreateOrderJson(dynamic request)
        {
            try
            {
                // Проверяем наличие Order
                if (request?.CommandParameters?.Order == null)
                {
                    return CreateJsonErrorResponse("CreateOrder", "Не указан заказ");
                }

                // Динамически извлекаем Order
                dynamic orderJson = request.CommandParameters.Order;

                // Валидируем заказ
                var validationError = ValidateOrderJson(orderJson);
                if (!string.IsNullOrEmpty(validationError))
                {
                    return CreateJsonErrorResponse("CreateOrder", validationError);
                }

                // Создаем заказ
                var orderResult = await _dataService.CreateOrderAsync(orderJson);

                // Динамически создаем ответ
                dynamic response = new ExpandoObject();
                response.Command = "CreateOrder";
                response.Success = true;
                response.ErrorMessage = "";

                dynamic data = new ExpandoObject();
                data.OrderId = orderResult.OrderId;
                data.TotalAmount = orderResult.TotalAmount;
                data.CreatedAt = orderResult.CreatedAt;
                data.Status = orderResult.Status;

                response.Data = data;

                //_logger.LogInformation("CreateOrder обработан успешно. OrderId: {OrderId}",
                //    orderResult.OrderId);

                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки CreateOrder");
                return CreateJsonErrorResponse("CreateOrder", $"Ошибка создания заказа: {ex.Message}");
            }
        }

        /// <summary>
        /// Создание JSON ответа с ошибкой
        /// </summary>
        private IActionResult CreateJsonErrorResponse(string command, string errorMessage)
        {
            dynamic errorResponse = new ExpandoObject();
            errorResponse.Command = command;
            errorResponse.Success = false;
            errorResponse.ErrorMessage = errorMessage;
            errorResponse.Data = null;

            var jsonResponse = JsonConvert.SerializeObject(errorResponse);

            // Всегда возвращаем 200 OK, даже при ошибках
            return Content(jsonResponse, "application/json");
        }

        /// <summary>
        /// Валидация заказа в JSON формате
        /// </summary>
        private string ValidateOrderJson(dynamic orderJson)
        {
            try
            {
                // Проверяем OrderItems
                var orderItems = orderJson?.OrderItems as JArray;
                if (orderItems == null || orderItems.Count == 0)
                {
                    return "Заказ должен содержать хотя бы одну позицию";
                }

                //bool isDelivery = orderJson?.IsDelivery?.Value<bool>() ?? false;
                bool isDelivery = orderJson?.IsDelivery?.Value ?? false;
                // Проверяем номер стола для не-доставки
                if (!isDelivery)
                {
                    string tableNumber = orderJson?.TableNumber?.Value;
                    if (string.IsNullOrWhiteSpace(tableNumber))
                    {
                        return "Укажите номер стола";
                    }
                }

                // Проверяем данные для доставки
                if (isDelivery)
                {
                    string address = orderJson?.DeliveryAddress?.Value;
                    if (string.IsNullOrWhiteSpace(address))
                    {
                        return "Укажите адрес доставки";
                    }

                    string phone = orderJson?.CustomerPhone?.Value;
                    if (string.IsNullOrWhiteSpace(phone))
                    {
                        return "Укажите телефон клиента";
                    }
                }

                // Проверяем каждую позицию заказа
                for (int i = 0; i < orderItems.Count; i++)
                {
                    var item = orderItems[i];

                    string menuItemId = item["MenuItemId"]?.Value<string>();
                    if (string.IsNullOrWhiteSpace(menuItemId))
                    {
                        return $"Позиция {i + 1}: не указан ID блюда";
                    }

                    var quantityToken = item["Quantity"];
                    if (quantityToken == null || quantityToken.Value<decimal>() <= 0)
                    {
                        return $"Позиция {i + 1}: количество должно быть больше 0";
                    }

                    var priceToken = item["Price"];
                    if (priceToken != null && priceToken.Value<decimal>() < 0)
                    {
                        return $"Позиция {i + 1}: цена не может быть отрицательной";
                    }
                }

                return null; // Валидация пройдена
            }
            catch (Exception ex)
            {
                return $"Ошибка валидации заказа: {ex.Message}";
            }
        }

        /// <summary>
        /// Метод для тестирования - возвращает примеры запросов/ответов
        /// </summary>
        [HttpGet("examples")]
        [AllowAnonymous]
        public IActionResult GetExamples()
        {
            var examples = new
            {
                GetMenuRequest = new
                {
                    Command = "GetMenu",
                    CommandParameters = new
                    {
                        WithPrice = true
                    }
                },
                GetMenuResponse = new
                {
                    Command = "GetMenu",
                    Success = true,
                    ErrorMessage = "",
                    Data = new
                    {
                        MenuItems = new[]
                        {
                            new
                            {
                                Id = "5979224",
                                Article = "A1004292",
                                Name = "Каша гречневая",
                                Price = 50,
                                IsWeighted = false,
                                FullPath = "ПРОИЗВОДСТВО\\Гарниры",
                                Barcodes = new[] { "57890975627974236429" }
                            }
                        }
                    }
                },
                CreateOrderRequest = new
                {
                    Command = "CreateOrder",
                    CommandParameters = new
                    {
                        Order = new
                        {
                            TableNumber = "15",
                            WaiterName = "Иванов Иван",
                            OrderItems = new[]
                            {
                                new
                                {
                                    MenuItemId = "5979224",
                                    Quantity = 2,
                                    Price = 50
                                }
                            },
                            Comment = "Столик у окна"
                        }
                    }
                },
                CreateOrderResponse = new
                {
                    Command = "CreateOrder",
                    Success = true,
                    ErrorMessage = "",
                    Data = new
                    {
                        OrderId = "ORD20231215_0001",
                        TotalAmount = 100,
                        CreatedAt = DateTime.Now,
                        Status = "Новый"
                    }
                }
            };

            return Content(JsonConvert.SerializeObject(examples, Newtonsoft.Json.Formatting.Indented),
                "application/json");
        }

        /// <summary>
        /// Метод для проверки здоровья API
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0"
            });
        }
    }
}

