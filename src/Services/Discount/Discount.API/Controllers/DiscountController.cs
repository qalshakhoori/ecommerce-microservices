using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Discount.API.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class DiscountController : ControllerBase
	{
		private readonly IDiscountRepository _repo;

		public DiscountController(IDiscountRepository repo)
		{
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
		}

		[HttpGet("{productName}", Name = "GetDiscount")]
		[ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> GetDiscount(string productName)
		{
			var coupon = await _repo.GetDiscount(productName);

			return Ok(coupon);
		}

		[HttpPost]
		[ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.Created)]
		public async Task<IActionResult> CreateDiscount([FromBody] Coupon coupon)
		{
			await _repo.CreateDiscount(coupon);
			return CreatedAtRoute("GetDiscount", new { productName = coupon.ProductName }, coupon);
		}

		[HttpPut]
		[ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> UpdateBasket([FromBody] Coupon coupon)
		{
			return Ok(await _repo.UpdateDiscount(coupon));
		}

		[HttpDelete("{productName}", Name = "DeleteDiscount")]
		[ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> DeleteDiscount(string productName)
		{
			return Ok(await _repo.DeleteDiscount(productName));
		}
	}
}