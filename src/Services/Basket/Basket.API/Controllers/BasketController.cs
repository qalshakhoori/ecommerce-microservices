using Basket.API.Entities;
using Basket.API.Repositories;
using Basket.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")]
  public class BasketController : ControllerBase
  {
    private readonly IBasketRepository _repo;
    private readonly DiscountGrpcService _discountGrpcService;

    public BasketController(IBasketRepository repo, DiscountGrpcService discountGrpcService)
    {
      _repo = repo ?? throw new ArgumentException(null, nameof(repo));
      _discountGrpcService = discountGrpcService;
    }

    [HttpGet("{username}", Name = "GetBasket")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShoppingCart))]
    public async Task<IActionResult> GetBasket(string username)
    {
      var basket = await _repo.GetBasket(username);

      return Ok(basket ?? new ShoppingCart(username));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShoppingCart))]
    public async Task<IActionResult> UpdateBasket([FromBody] ShoppingCart basket)
    {
      // TODO: Communicate with discount.grpc and Calculate latest prices of product into shopping cart
      foreach (var item in basket.Items)
      {
        var coupon = await _discountGrpcService.GetDiscountAsync(item.ProductName);

        item.Price -= coupon.Amount;
      }

      return Ok(await _repo.UpdateBasket(basket));
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
    public async Task<IActionResult> DeleteBasket(string username)
    {
      await _repo.DeleteBasket(username);

      return Ok();
    }
  }
}