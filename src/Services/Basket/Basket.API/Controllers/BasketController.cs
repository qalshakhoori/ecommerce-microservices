using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories;
using Basket.API.Services;
using Common;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Constants.Client_Id_Policy)]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publish;
        private readonly DiscountGrpcService _discountGrpcService;

        public BasketController(IBasketRepository repo, DiscountGrpcService discountGrpcService, IMapper mapper, IPublishEndpoint publish)
        {
            _repo = repo ?? throw new ArgumentException(null, nameof(repo));
            _discountGrpcService = discountGrpcService;
            _mapper = mapper;
            _publish = publish;
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

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShoppingCart))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            // get existing basket with total price
            var basket = await _repo.GetBasket(basketCheckout.UserName);
            if (basket == null)
                return BadRequest();

            // Create basketCheckoutEvent -- Set TotalPrice on basketCheckout eventMessage
            var checkoutEvent = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            checkoutEvent.TotalPrice = basket.TotalPrice;


            // send checkout event to rabbitmq
            await _publish.Publish(checkoutEvent);

            // remove the basket
            await _repo.DeleteBasket(basket.Username);

            return Accepted();
        }
    }
}