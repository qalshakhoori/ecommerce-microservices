using Shopping_Aggregator.Extensions;
using Shopping_Aggregator.Models;

namespace Shopping_Aggregator.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _client;

        public OrderService(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName)
        {
            var response = await _client.GetAsync($"/api/v1/Order/{userName}");

            return await response.ReadContentAs<IEnumerable<OrderResponseModel>>();
        }
    }
}