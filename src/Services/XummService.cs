using System;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Xumm;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Stores;
using XUMM.NET.SDK;
using XUMM.NET.SDK.Extensions;
using XUMM.NET.SDK.Models.Misc;

namespace Nop.Plugin.ExternalAuth.Xumm.Services;

public class XummService : IXummService
{
    #region Fields

    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;
    private readonly ILogger _logger;

    #endregion

    #region Ctor

    public XummService(
        ISettingService settingService,
        IStoreService storeService,
        ILogger logger)
    {
        _settingService = settingService;
        _storeService = storeService;
        _logger = logger;
    }

    #endregion

    #region Methods

    public async Task<XummSdk> GetXummSdk(int storeId)
    {
        var settings = await _settingService.LoadSettingAsync<XummExternalAuthenticationSettings>(storeId);
        if (!settings.ApiKey.IsValidUuid() || !settings.ApiSecret.IsValidUuid())
        {
            throw new InvalidOperationException($"Missing API Key and/or Secret to create an instance of {nameof(XummSdk)}");
        }

        return new XummSdk(settings.ApiKey, settings.ApiSecret);
    }

    public async Task<XummPong> GetPongAsync(int storeId)
    {
        try
        {
            return await (await GetXummSdk(storeId)).Miscellaneous.GetPingAsync();
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Failed to retrieve Xumm pong with provided credentials for store: {storeId}.", ex);
            return null;
        }
    }

    public async Task<bool> HasRedirectUrlConfiguredAsync(int storeId, XummPong pong)
    {
        var redirectUrl = await GetRedirectUrlAsync(storeId);
        return pong?.Auth.Application.RedirectUris.Any(s => s.Equals(redirectUrl, StringComparison.InvariantCultureIgnoreCase)) ?? false;
    }

    public async Task<string> GetRedirectUrlAsync(int storeId)
    {
        var storeLocation = await GetStoreLocation(storeId);
        return $"{storeLocation}{XummAuthenticationDefaults.CallbackPath.TrimStart('/')}";
    }

    private async Task<string> GetStoreLocation(int storeId)
    {
        var store = storeId > 0
            ? await _storeService.GetStoreByIdAsync(storeId)
            : (await _storeService.GetAllStoresAsync()).FirstOrDefault();

        var storeLocation = store?.Url ?? throw new Exception($"Store {storeId} cannot be loaded");

        //ensure that URL is ended with slash
        storeLocation = $"{storeLocation.TrimEnd('/')}/";

        return storeLocation;
    }

    #endregion
}
