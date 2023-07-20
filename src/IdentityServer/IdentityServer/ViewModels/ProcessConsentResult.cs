using IdentityServer4.Models;

namespace IdentityServer.ViewModels
{
    public class ProcessConsentResult
    {
        public string RedirectUri { get; set; }
        public bool IsRedirect => RedirectUri != null;
        public Client Client { get; set; }

        public ConsentViewModel ViewModel { get; set; }
        public bool ShowView => ViewModel != null;

        public string ValidationError { get; set; }
        public bool HasValidationError => ValidationError != null;

    }
}
