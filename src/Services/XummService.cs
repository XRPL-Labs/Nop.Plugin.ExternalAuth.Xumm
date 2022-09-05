using AspNet.Security.OAuth.Xumm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nop.Core;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using XUMM.NET.SDK.Clients;
using XUMM.NET.SDK.Configs;
using XUMM.NET.SDK.Extensions;
using XUMM.NET.SDK.Models.Misc;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.ExternalAuth.Xumm.Services;

public class XummService : IXummService
{
    #region Fields

    private readonly IServiceCollection _serviceCollection;
    private readonly IWebHelper _webHelper;
    private readonly XummExternalAuthenticationSettings _xummExternalAuthSettings;
    private readonly ILogger _logger;

    #endregion

    #region Ctor

    public XummService(
        IServiceCollection serviceCollection,
        IWebHelper webHelper,
        XummExternalAuthenticationSettings xummExternalAuthSettings,
        ILogger logger)
    {
        _serviceCollection = serviceCollection;
        _webHelper = webHelper;
        _xummExternalAuthSettings = xummExternalAuthSettings;
        _logger = logger;
    }

    #endregion

    #region Methods

    public async Task<XummPong> GetPongAsync()
    {
        try
        {
            var xummMiscClient = GetMiscClient();
            return await xummMiscClient.GetPingAsync();
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Failed to retrieve Xumm pong with provided credentials.", ex);
            return null;
        }
    }

    public bool HasRedirectUrlConfigured(XummPong pong)
    {
        return pong?.Auth.Application.RedirectUris.Any(s => s.Equals(RedirectUrl, StringComparison.InvariantCultureIgnoreCase)) ?? false;
    }

    private XummMiscClient GetMiscClient()
    {
        var serviceProvider = _serviceCollection.BuildServiceProvider();

        var apiConfig = new ApiConfig();
        if (_xummExternalAuthSettings.ApiKey.IsValidUuid())
        {
            apiConfig.ApiKey = _xummExternalAuthSettings.ApiKey;
        }

        if (_xummExternalAuthSettings.ApiSecret.IsValidUuid())
        {
            apiConfig.ApiSecret = _xummExternalAuthSettings.ApiSecret;
        }

        // It's possible that the 'Payments.Xumm` plugin is also installed so we have to make sure that we use the credentials configured in this plugin.
        var xummHttpClient = new XummHttpClient(serviceProvider.GetRequiredService<IHttpClientFactory>(), Options.Create(apiConfig), serviceProvider.GetRequiredService<ILogger<XummHttpClient>>());
        return new XummMiscClient(xummHttpClient);
    }

    #endregion

    #region Properties

    public string RedirectUrl => $"{_webHelper.GetStoreLocation()}{XummAuthenticationDefaults.CallbackPath.TrimStart('/')}";

    #endregion
}
