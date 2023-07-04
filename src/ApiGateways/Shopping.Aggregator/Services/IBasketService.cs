using Shopping_Aggregator.Models;

namespace Shopping_Aggregator.Services
{
    public interface IBasketService
    {
        Task<BasketModel> GetBasket(string userName);
    }
}
