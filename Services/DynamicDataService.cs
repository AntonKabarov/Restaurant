using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace RestaurantApi.Services
{
    public interface IDataService
    {
        Task<List<dynamic>> GetMenuItemsAsync();
        Task<dynamic> CreateOrderAsync(dynamic orderJson);
        Task<dynamic> GetOrderAsync(string orderId);
    }
    public class DynamicDataService : IDataService
    {
        private readonly List<dynamic> _menuItems;
        private readonly List<dynamic> _orders;
        private int _orderCounter = 1;

        public DynamicDataService()
        {
            // Инициализация тестовых данных через динамические объекты
            _menuItems = new List<dynamic>
            {
                new {
                    Id = "5979224",
                    Article = "A1004292",
                    Name = "Каша гречневая",
                    Price = 50m,
                    IsWeighted = false,
                    FullPath = "ПРОИЗВОДСТВО\\Гарниры",
                    Barcodes = new List<string> { "57890975627974236429" }
                },
                new {
                    Id = "9084246",
                    Article = "A1004293",
                    Name = "Конфеты Коровка",
                    Price = 300m,
                    IsWeighted = true,
                    FullPath = "ДЕСЕРТЫ\\Развес",
                    Barcodes = new List<string>()
                },
                new {
                    Id = "1234567",
                    Article = "A1004294",
                    Name = "Суп куриный",
                    Price = 120m,
                    IsWeighted = false,
                    FullPath = "ПРОИЗВОДСТВО\\Супы",
                    Barcodes = new List<string> { "123456789012" }
                }
            };

            _orders = new List<dynamic>();
        }

        public Task<List<dynamic>> GetMenuItemsAsync()
        {
            return Task.FromResult(_menuItems);
        }

        public async Task<dynamic> CreateOrderAsync(dynamic orderJson)
        {
            // Преобразуем dynamic в JObject для удобства работы
            JObject orderJObject = orderJson as JObject ?? JObject.FromObject(orderJson);

            // Получаем позиции заказа
            var orderItems = orderJObject["OrderItems"] as JArray;

            // Рассчитываем общую сумму
            decimal totalAmount = 0;

            if (orderItems != null)
            {
                foreach (var item in orderItems)
                {
                    var menuItemId = item["MenuItemId"]?.Value<string>();
                    var quantity = item["Quantity"]?.Value<decimal>() ?? 0;
                    var price = item["Price"]?.Value<decimal>() ?? 0;

                    // Если цена не указана, ищем в меню
                    if (price == 0)
                    {
                        var menuItem = _menuItems.FirstOrDefault(m =>
                            m.Id.ToString() == menuItemId);
                        if (menuItem != null)
                        {
                            price = menuItem.Price;
                        }
                    }

                    totalAmount += quantity * price;
                }
            }

            // Создаем динамический объект заказа
            dynamic order = new ExpandoObject();
            order.OrderId = $"ORD{DateTime.Now:yyyyMMdd}_{_orderCounter++:D4}";
            order.TableNumber = orderJObject["TableNumber"]?.Value<string>();
            order.WaiterName = orderJObject["WaiterName"]?.Value<string>();
            order.OrderItems = orderItems;
            order.Comment = orderJObject["Comment"]?.Value<string>();
            order.IsDelivery = orderJObject["IsDelivery"]?.Value<bool>() ?? false;
            order.DeliveryAddress = orderJObject["DeliveryAddress"]?.Value<string>();
            order.CustomerPhone = orderJObject["CustomerPhone"]?.Value<string>();
            order.CreatedAt = DateTime.Now;
            order.TotalAmount = totalAmount;
            order.Status = "Новый";

            // Сохраняем заказ
            _orders.Add(order);

            // Возвращаем только необходимые для ответа данные
            dynamic response = new ExpandoObject();
            response.OrderId = order.OrderId;
            response.TotalAmount = order.TotalAmount;
            response.CreatedAt = order.CreatedAt;
            response.Status = order.Status;

            return await Task.FromResult(response);
        }

        public Task<dynamic> GetOrderAsync(string orderId)
        {
            var order = _orders.FirstOrDefault(o =>
                (o as dynamic).OrderId == orderId);

            return Task.FromResult(order);
        }

        /// <summary>
        /// Получить блюдо по ID
        /// </summary>
        public dynamic GetMenuItemById(string id)
        {
            return _menuItems.FirstOrDefault(m =>
                (m as dynamic).Id.ToString() == id);
        }

        /// <summary>
        /// Поиск блюд по названию
        /// </summary>
        public List<dynamic> SearchMenuItems(string searchTerm)
        {
            return _menuItems.Where(m =>
                ((string)(m as dynamic).Name).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Получить все заказы
        /// </summary>
        public List<dynamic> GetAllOrders()
        {
            return _orders;
        }

        /// <summary>
        /// Обновить статус заказа
        /// </summary>
        public bool UpdateOrderStatus(string orderId, string newStatus)
        {
            var order = _orders.FirstOrDefault(o =>
                (o as dynamic).OrderId == orderId);

            if (order != null)
            {
                (order as dynamic).Status = newStatus;
                (order as dynamic).UpdatedAt = DateTime.Now;
                return true;
            }

            return false;
        }
    }
}

