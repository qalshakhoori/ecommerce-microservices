using IdentityServer.ViewModels;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment env, ILogger<HomeController> logger)
        {
            _interaction = interaction;
            _env = env;
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (_env.IsDevelopment())
                return View();

            _logger.LogInformation("Homepage is disabled in production");

            return NotFound();
        }

        public async Task<IActionResult> Error(string errorId)
        {
            var viewModel = new ErrorViewModel();

            var message = await _interaction.GetErrorContextAsync(errorId);

            if (message != null)
            {
                viewModel.Error = message;

                if (!_env.IsDevelopment())
                    message.ErrorDescription = null;
            }

            return View("Error", viewModel);
        }
    }
}