namespace IdentityServer.Controllers
{
    public static class AccountOptions
    {
        public static readonly bool AllowLocalLogin = true;
        public static readonly bool AllowRememberLogin = true;
        public static readonly TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        public static readonly bool ShowLogoutPrompt = true;
        public static readonly bool AutomaticRedirectAfterSignOut = true;

        public static readonly string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}
