using Shopping_Aggregator.Extensions;
using Shopping_Aggregator.Models;

namespace Shopping_Aggregator.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _client;

        public BasketService(HttpClient client)
        {
            _client = client;
        }

        public async Task<BasketModel> GetBasket(string userName)
        {
            var response = await _client.GetAsync($"/api/v1/Basket/{userName}");

            return await response.ReadContentAs<BasketModel>();
        }
    }
}
