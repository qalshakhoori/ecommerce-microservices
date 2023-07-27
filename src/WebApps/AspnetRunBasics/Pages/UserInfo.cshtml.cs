using AspnetRunBasics.Models;
using AspnetRunBasics.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AspnetRunBasics.Pages
{
    [Authorize]
    public class UserInfoModel : PageModel
    {
        private readonly IIdpService _idpService;

        public UserInfoModel(IIdpService ipdService)
        {
            _idpService = ipdService;
        }

        public UserInfo UserInfo { get; set; }

        public async Task<IActionResult> OnGet()
        {
            await LogTokenAndClaims();

            UserInfo = await _idpService.GetUserInfo();

            return Page();
        }

        private async Task LogTokenAndClaims()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            Debug.WriteLine($"Identity token: {identityToken}");

            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }
    }
}
