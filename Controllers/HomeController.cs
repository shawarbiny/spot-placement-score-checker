using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using SpotPlacementScoreWebsite.Models;
using SpotPlacementScoreWebsite.Services;
using System.Diagnostics;

namespace SpotPlacementScoreWebsite.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAzureResourceService _azureResourceService;
        private readonly ISpotPlacementScoreService _spotPlacementScoreService;
        private readonly ITokenAcquisition _tokenAcquisition;

        public HomeController(
            ILogger<HomeController> logger,
            IAzureResourceService azureResourceService,
            ISpotPlacementScoreService spotPlacementScoreService,
            ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _azureResourceService = azureResourceService;
            _spotPlacementScoreService = spotPlacementScoreService;
            _tokenAcquisition = tokenAcquisition;
        }

        /// <summary>
        /// Display the main form with subscriptions dropdown
        /// </summary>
        [AuthorizeForScopes(Scopes = new[] { "https://management.azure.com/.default" })]
        public async Task<IActionResult> Index()
        {
            try
            {
                var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(
                    new[] { "https://management.azure.com/.default" });

                var subscriptions = await _azureResourceService.GetSubscriptionsAsync(accessToken);

                var model = new HomeViewModel
                {
                    Subscriptions = subscriptions
                };

                return View(model);
            }
            catch (MicrosoftIdentityWebChallengeUserException)
            {
                // Re-throw to allow AuthorizeForScopes to handle consent
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading subscriptions");
                ModelState.AddModelError("", "Failed to load subscriptions. Please try again.");
                return View(new HomeViewModel());
            }
        }

        /// <summary>
        /// Load instance series based on selected subscription
        /// Returns JSON for AJAX call
        /// </summary>
        [HttpGet]
        [AuthorizeForScopes(Scopes = new[] { "https://management.azure.com/.default" })]
        public async Task<IActionResult> GetInstanceSeries(string subscriptionId)
        {
            try
            {
                if (string.IsNullOrEmpty(subscriptionId))
                {
                    return BadRequest("Subscription ID is required");
                }

                var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(
                    new[] { "https://management.azure.com/.default" });

                var series = await _azureResourceService.GetInstanceSeriesAsync(subscriptionId, accessToken);

                return Json(new { series });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching instance series");
                return StatusCode(500, "Error fetching data");
            }
        }

        /// <summary>
        /// Load VM SKUs and regions based on selected subscription and series
        /// Returns JSON for AJAX call
        /// </summary>
        [HttpGet]
        [AuthorizeForScopes(Scopes = new[] { "https://management.azure.com/.default" })]
        public async Task<IActionResult> GetSkusAndRegions(string subscriptionId, string? series)
        {
            try
            {
                if (string.IsNullOrEmpty(subscriptionId))
                {
                    return BadRequest("Subscription ID is required");
                }

                var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(
                    new[] { "https://management.azure.com/.default" });

                var skus = await _azureResourceService.GetVmSkusAsync(subscriptionId, series, accessToken);
                var regions = await _azureResourceService.GetRegionsAsync(subscriptionId, accessToken);

                return Json(new { skus, regions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching SKUs and regions");
                return StatusCode(500, "Error fetching data");
            }
        }

        /// <summary>
        /// Process the form submission and check spot placement scores
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeForScopes(Scopes = new[] { "https://management.azure.com/.default" })]
        public async Task<IActionResult> CheckSpotPlacement(SpotPlacementRequest request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(request.SubscriptionId))
                {
                    ModelState.AddModelError("", "Please select a subscription");
                }

                if (request.SelectedSkus == null || request.SelectedSkus.Count == 0)
                {
                    ModelState.AddModelError("", "Please select at least one VM SKU");
                }
                else if (request.SelectedSkus.Count > 5)
                {
                    ModelState.AddModelError("", "Please select no more than 5 VM SKUs");
                }

                if (request.SelectedRegions == null || request.SelectedRegions.Count == 0)
                {
                    ModelState.AddModelError("", "Please select at least one region");
                }
                else if (request.SelectedRegions.Count > 3)
                {
                    ModelState.AddModelError("", "Please select no more than 3 regions");
                }

                if (!ModelState.IsValid)
                {
                    var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(
                        new[] { "https://management.azure.com/.default" });
                    
                    var subscriptions = await _azureResourceService.GetSubscriptionsAsync(accessToken);
                    var skus = await _azureResourceService.GetVmSkusAsync(request.SubscriptionId, null, accessToken);
                    var regions = await _azureResourceService.GetRegionsAsync(request.SubscriptionId, accessToken);

                    var model = new HomeViewModel
                    {
                        Subscriptions = subscriptions,
                        VmSkus = skus,
                        Regions = regions,
                        Request = request
                    };

                    return View("Index", model);
                }

                // Get access token and check spot placement scores
                var token = await _tokenAcquisition.GetAccessTokenForUserAsync(
                    new[] { "https://management.azure.com/.default" });

                var (results, requestJson, responseJson) = await _spotPlacementScoreService.CheckSpotPlacementScoreAsync(
                    request.SubscriptionId,
                    request.SelectedSkus ?? new List<string>(),
                    request.SelectedRegions ?? new List<string>(),
                    request.UseAvailabilityZones,
                    request.DesiredCount,
                    token);

                // Create view model with results and raw API data
                var viewModel = new HomeViewModel
                {
                    Results = results,
                    RawApiRequest = requestJson,
                    RawApiResponse = responseJson,
                    Request = request
                };

                // Return results view
                return View("Results", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking spot placement");
                ModelState.AddModelError("", "An error occurred while checking spot placement scores. Please try again.");
                
                var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(
                    new[] { "https://management.azure.com/.default" });
                
                var subscriptions = await _azureResourceService.GetSubscriptionsAsync(accessToken);

                return View("Index", new HomeViewModel { Subscriptions = subscriptions });
            }
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
