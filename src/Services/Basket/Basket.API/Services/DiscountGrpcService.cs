using Discount.Grpc.Protos;

namespace Basket.API.Services
{
  public class DiscountGrpcService
  {
    private readonly DiscountProtoService.DiscountProtoServiceClient _client;

    public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient client)
    {
      _client = client;
    }

    public async Task<CouponModel> GetDiscountAsync(string productName)
    {
      var discountRequest = new GetDiscountRequest { ProductName = productName };

      return await _client.GetDiscountAsync(discountRequest);
    }
  }
}