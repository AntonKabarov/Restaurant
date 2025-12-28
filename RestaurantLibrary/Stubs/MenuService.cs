using RestaurantLibrary.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantLibrary.Stubs
{
    public interface IMenuService
    {
        Task<List<Dish>> GetDishesAsync(bool withPrice);
        Task<Dish> GetDishByIdAsync(string id);
    }

    // Реализации
    public class MenuService : IMenuService
    {
        private readonly List<Dish> _dishes = new()
    {
        new Dish("5979224", "A1004292", "Каша гречневая", 50, false),
        new Dish("9084246", "A1004293", "Конфеты Коровка", 300, true)
    };
        public Task<List<Dish>> GetDishesAsync(bool withPrice)
        {
            var dishes = new List<Dish>
            {
                new Dish(
                    Id: "5979224",
                    Article: "A1004292",
                    Name: "Каша гречневая",
                    Price: withPrice ? 50 : 0,
                    FullPath: "ПРОИЗВОДСТВО\\Гарниры",
                    Barcodes: new List<string> { "57890975627974236429" }
                ),
                new Dish(
                    Id: "9084246",
                    Article: "A1004293",
                    Name: "Конфеты Коровка",
                    Price: withPrice ? 300 : 0,
                    IsWeighted: true,
                    FullPath: "ДЕСЕРТЫ\\Развес"
                )
            };

            return Task.FromResult(dishes);
        }
        public Task<Dish> GetDishByIdAsync(string id)
        {
            var dish = _dishes.FirstOrDefault(d => d.Id == id);
            return Task.FromResult(dish);
        }
    }
}
