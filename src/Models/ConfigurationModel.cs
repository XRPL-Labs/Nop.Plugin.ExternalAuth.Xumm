using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.ExternalAuth.Xumm.Models;

/// <summary>
/// Represents plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    #region Properties

    /// <summary>
    /// Gets or sets an API Key
    /// </summary>
    [NopResourceDisplayName("Plugins.ExternalAuth.Xumm.Fields.ApiKey")]
    public string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets an API Secret
    /// </summary>
    [NopResourceDisplayName("Plugins.ExternalAuth.Xumm.Fields.ApiSecret")]
    public string ApiSecret { get; set; }

    /// <summary>
    /// Gets or sets a Redirect URL
    /// </summary>
    [NopResourceDisplayName("Plugins.ExternalAuth.Xumm.Fields.RedirectUris")]
    public Dictionary<string, bool> RedirectUris { get; set; } = new();

    public bool ValidApiCredentials { get; set; }

    public bool ApiCredentialsProvided => !string.IsNullOrWhiteSpace(ApiKey) && !string.IsNullOrWhiteSpace(ApiSecret);

    public bool HasRedirectUrlConfigured { get; set; }

    #endregion
}
