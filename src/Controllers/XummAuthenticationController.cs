using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Xumm;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Plugin.ExternalAuth.Xumm.Models;
using Nop.Plugin.ExternalAuth.Xumm.Services;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.ExternalAuth.Xumm.Controllers;

[AutoValidateAntiforgeryToken]
public class XummAuthenticationController : BasePluginController
{
    #region Fields

    private readonly IAuthenticationPluginManager _authenticationPluginManager;
    private readonly IExternalAuthenticationService _externalAuthenticationService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IOptionsMonitorCache<XummAuthenticationOptions> _optionsCache;
    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly IXummService _xummService;

    #endregion

    #region Ctor

    public XummAuthenticationController(
        IAuthenticationPluginManager authenticationPluginManager,
        IExternalAuthenticationService externalAuthenticationService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IOptionsMonitorCache<XummAuthenticationOptions> optionsCache,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreContext storeContext,
        IWorkContext workContext,
        IXummService xummService)
    {
        _authenticationPluginManager = authenticationPluginManager;
        _externalAuthenticationService = externalAuthenticationService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _optionsCache = optionsCache;
        _permissionService = permissionService;
        _settingService = settingService;
        _storeContext = storeContext;
        _workContext = workContext;
        _xummService = xummService;
    }

    #endregion

    #region Methods

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
            return AccessDeniedView();

        // Load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<XummExternalAuthenticationSettings>(storeScope);

        var pong = await _xummService.GetPongAsync(storeScope);

        var model = new ConfigurationModel
        {
            ActiveStoreScopeConfiguration = storeScope,
            ApiKey = settings.ApiKey,
            ApiSecret = settings.ApiSecret,
            RedirectUrl = await _xummService.GetRedirectUrlAsync(storeScope),
            ValidApiCredentials = pong?.Pong ?? false,
            HasRedirectUrlConfigured = await _xummService.HasRedirectUrlConfiguredAsync(storeScope, pong)
        };

        return View("~/Plugins/ExternalAuth.Xumm/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return await Configure();

        // Load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<XummExternalAuthenticationSettings>(storeScope);

        var changed = model.ApiKey != settings.ApiKey || model.ApiSecret != settings.ApiSecret;
        if (changed)
        {
            settings.ApiKey = model.ApiKey;
            settings.ApiSecret = model.ApiSecret;

            await _settingService.SaveSettingAsync(settings);

            // Clear Xumm Authentication options cache
            _optionsCache.TryRemove(XummAuthenticationDefaults.AuthenticationScheme);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
        }

        return await Configure();
    }

    public async Task<IActionResult> Login(string returnUrl)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var methodIsAvailable = await _authenticationPluginManager.IsPluginActiveAsync(XummExternalAuthenticationDefaults.SystemName, await _workContext.GetCurrentCustomerAsync(), store.Id);

        if (!methodIsAvailable)
        {
            throw new NopException("Xumm Authentication module cannot be loaded");
        }

        var settings = await _settingService.LoadSettingAsync<XummExternalAuthenticationSettings>(store.Id);
        if (string.IsNullOrEmpty(settings.ApiKey) || string.IsNullOrEmpty(settings.ApiSecret))
        {
            throw new NopException("Xumm Authentication module not configured");
        }

        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("LoginCallback", "XummAuthentication", new { returnUrl = returnUrl })
        };

        authenticationProperties.SetString(XummExternalAuthenticationDefaults.ErrorCallback, Url.RouteUrl("Login", new { returnUrl }));

        return Challenge(authenticationProperties, XummAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> LoginCallback(string returnUrl)
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(XummAuthenticationDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any())
        {
            return RedirectToRoute("Login", new { returnUrl = returnUrl });
        }

        var authenticationParameters = new ExternalAuthenticationParameters
        {
            ProviderSystemName = XummExternalAuthenticationDefaults.SystemName,
            AccessToken = await HttpContext.GetTokenAsync(XummAuthenticationDefaults.AuthenticationScheme, "access_token"),
            Email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value,
            ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
            ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
            Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
        };

        return await _externalAuthenticationService.AuthenticateAsync(authenticationParameters, returnUrl);
    }

    #endregion
}
