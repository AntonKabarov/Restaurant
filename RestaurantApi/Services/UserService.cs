namespace RestaurantApi.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
    }

    public class UserService : IUserService
    {
        // В реальном приложении - проверка из БД
        private readonly List<User> _users = new List<User>
        {
            new User { Id = 1, Username = "api_user", Password = "secure_password", Role = "ApiUser" },
            new User { Id = 2, Username = "waiter", Password = "waiter123", Role = "Waiter" },
            new User { Id = 3, Username = "admin", Password = "admin123", Role = "Administrator" }
        };

        public Task<User> Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x =>
                x.Username == username && x.Password == password);

            return Task.FromResult(user);
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
