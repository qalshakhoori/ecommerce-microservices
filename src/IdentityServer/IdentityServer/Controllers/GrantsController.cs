using IdentityServer.ViewModels;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    [SecurityHeaders]
    [Authorize]
    public class GrantsController : Controller
    {
        private readonly IIdentityServerInteractionService _interactions;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IEventService _eventService;

        public GrantsController(IIdentityServerInteractionService interactions, IClientStore clientStore, IResourceStore resourceStore, IEventService eventService)
        {
            _interactions = interactions;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View("Index", await GetViewModelAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke(string clientId)
        {
            await _interactions.RevokeUserConsentAsync(clientId);
            await _eventService.RaiseAsync(new GrantsRevokedEvent(User.GetSubjectId(), clientId));

            return RedirectToAction("Index");
        }

        private async Task<GrantsViewModel> GetViewModelAsync()
        {
            var grants = await _interactions.GetAllUserGrantsAsync();

            var list = new List<GrantViewModel>();
            foreach (var grant in grants)
            {
                var client = await _clientStore.FindClientByIdAsync(grant.ClientId);
                if (client != null)
                {
                    var resource = await _resourceStore.FindResourcesByScopeAsync(grant.Scopes);

                    var item = new GrantViewModel
                    {
                        ClientId = client.ClientId,
                        ClientName = client.ClientName ?? client.ClientId,
                        ClientLogoUrl = client.LogoUri,
                        ClientUrl = client.ClientUri,
                        Description = grant.Description,
                        Created = grant.CreationTime,
                        Expires = grant.Expiration,
                        IdentityGrantNames = resource.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
                        ApiGrantNames = resource.ApiResources.Select(x => x.DisplayName ?? x.Name).ToArray()
                    };

                    list.Add(item);
                }
            }

            return new GrantsViewModel
            {
                Grants = list
            };
        }
    }
}