using Microsoft.AspNetCore.Mvc;
using Shopping_Aggregator.Models;
using Shopping_Aggregator.Services;
using System.Net;

namespace Shopping_Aggregator.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShoppingController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        public ShoppingController(ICatalogService catalogService, IBasketService basketService, IOrderService orderService)
        {
            _catalogService = catalogService;
            _basketService = basketService;
            _orderService = orderService;
        }

        [HttpGet("userName", Name = "GetShopping")]
        [ProducesResponseType(typeof(ShoppingModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingModel>> GetShopping(string userName)
        {
            // get basket with username
            var basket = await _basketService.GetBasket(userName);

            // iterate basket items and consume products with basket item productId member
            foreach (var item in basket.Items)
            {
                // map product related members into basketitem dto with extended columns
                var product = await _catalogService.GetCatalog(item.ProductId);

                // set additional product fields onto basket item
                item.ProductName = product.Name;
                item.Category = product.Catagory;
                item.Summery = product.Summery;
                item.Description = product.Description;
                item.ImageFile = product.ImageFile;
            }

            // consume ordering  microservices in order to retrive order list
            var orders = await _orderService.GetOrdersByUserName(userName);

            var shoppingModel = new ShoppingModel
            {
                UserName = userName,
                BasketWithProducts = basket,
                Orders = orders
            };

            // return root ShoppingModel dto class which including  all responses
            return Ok(shoppingModel);
        }
    }
}
