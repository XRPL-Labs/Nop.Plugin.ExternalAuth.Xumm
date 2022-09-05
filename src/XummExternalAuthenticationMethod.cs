using Nop.Core;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.Xumm;

/// <summary>
/// Represents method for the authentication with Xumm account
/// </summary>
public class XummExternalAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public XummExternalAuthenticationMethod(
        ILocalizationService localizationService,
        ISettingService settingService,
        IWebHelper webHelper)
    {
        _localizationService = localizationService;
        _settingService = settingService;
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/XummAuthentication/Configure";
    }

    /// <summary>
    /// Gets a name of a view component for displaying plugin in public store
    /// </summary>
    /// <returns>View component name</returns>
    public string GetPublicViewComponentName()
    {
        return XummExternalAuthenticationDefaults.VIEW_COMPONENT_NAME;
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //settings
        await _settingService.SaveSettingAsync(new XummExternalAuthenticationSettings());

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.ExternalAuth.Xumm.Section.ApiSettings"] = "API Settings",
            ["Plugins.ExternalAuth.Xumm.Section.ApiSettings.Instructions"] = @"
                    <div style=""margin: 0 0 20px;"">
                        For plugin configuration, follow these steps:<br />
                        <br />
                        1. You will need a Xumm Developer account. If you don't already have one, you can sign up here: <a href=""https://apps.xumm.dev/"" target=""_blank"">https://apps.xumm.dev/</a><br />
                        2. Sign in to 'Xumm Developer Dashboard'. Go to 'Settings' tab, copy 'API Key', 'API Secret' and paste it into the same fields below.<br />
                        3. Update the Redirect URI's with URL <em>{0}</em> on the 'Application details' section of 'Settings'.<br />
                    </div>",
            ["Plugins.ExternalAuth.Xumm.Button.ShowHideSecrets"] = "Show/Hide Secrets",
            ["Plugins.ExternalAuth.Xumm.Fields.ApiKey"] = "API Key",
            ["Plugins.ExternalAuth.Xumm.Fields.ApiKey.Hint"] = "Enter the API Key for the live environment.",
            ["Plugins.ExternalAuth.Xumm.Fields.ApiKey.Required"] = "API Key is required",
            ["Plugins.ExternalAuth.Xumm.Fields.ApiKey.Invalid"] = "API Key format is invalid",
            ["Plugins.ExternalAuth.Xumm.Fields.ApiSecret"] = "API Secret",
            ["Plugins.ExternalAuth.Xumm.Fields.ApiSecret.Hint"] = "Enter the API Secret for the live environment.",
            ["Plugins.ExternalAuth.Xumm.Fields.ApiSecret.Required"] = "API Secret is required",
            ["Plugins.ExternalAuth.Xumm.Fields.ApiSecret.Invalid"] = "API Secret format is invalid",
            ["Plugins.ExternalAuth.Xumm.Fields.RedirectUrl"] = "Redirect URL",
            ["Plugins.ExternalAuth.Xumm.Fields.RedirectUrl.Hint"] = "Update the redirect URI's with URL at the 'Application details' section of 'Settings' in the Xumm Developer Dashboard.",
            ["Plugins.ExternalAuth.Xumm.Fields.RedirectUrl.NotConfigured"] = "Redirect URL has not been configured in the Xumm Developer Dashboard.",
            ["Plugins.ExternalAuth.Xumm.PublicInfo.SignInWithXumm"] = "Sign In with Xumm"
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<XummExternalAuthenticationSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.ExternalAuth.Xumm");

        await base.UninstallAsync();
    }

    #endregion
}
