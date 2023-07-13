using IdentityServer.Models;

namespace IdentityServer.ViewModels
{
    public class LogoutViewModel : LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; } = true;
    }
}
