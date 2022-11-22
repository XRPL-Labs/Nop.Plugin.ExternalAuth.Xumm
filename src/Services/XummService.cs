using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Xumm;
using Nop.Core.Domain.Stores;
using Nop.Plugin.ExternalAuth.Xumm.Models;
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

    public async Task<XummPong> GetPongAsync(int storeId = 0)
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

    public async Task<List<StoreRedirectUriModel>> GetStoreRedirectUrisAsync(XummPong pong)
    {
        var result = new List<StoreRedirectUriModel>();
        var stores = await _storeService.GetAllStoresAsync();
        foreach (var store in stores)
        {
            var redirectUrl = GetStoreRedirectUrl(store);
            var model = new StoreRedirectUriModel
            {
                StoreName = store.Name,
                RedirectUri = redirectUrl,
                Configured = pong?.Auth.Application.RedirectUris.Any(s => s.Equals(redirectUrl, StringComparison.InvariantCultureIgnoreCase)) ?? false
            };

            result.Add(model);
        }

        return result;
    }

    public async Task<string> GetRedirectUrlAsync(int storeId)
    {
        var store = storeId > 0
            ? await _storeService.GetStoreByIdAsync(storeId)
            : (await _storeService.GetAllStoresAsync()).FirstOrDefault();

        return GetStoreRedirectUrl(store);
    }

    private string GetStoreRedirectUrl(Store store)
    {
        var storeLocation = store?.Url ?? throw new Exception($"Store {store?.Id} has no URL configured");

        //ensure that URL is ended with slash
        storeLocation = $"{storeLocation.TrimEnd('/')}/";

        return $"{storeLocation}{XummAuthenticationDefaults.CallbackPath.TrimStart('/')}";
    }

    #endregion
}
