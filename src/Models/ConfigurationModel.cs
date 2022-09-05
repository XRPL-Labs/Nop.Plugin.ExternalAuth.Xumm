using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.ExternalAuth.Xumm.Models;

/// <summary>
/// Represents plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
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
    [NopResourceDisplayName("Plugins.ExternalAuth.Xumm.Fields.RedirectUrl")]
    public string RedirectUrl { get; set; }

    public bool ValidApiCredentials { get; set; }

    public bool HasRedirectUrlConfigured { get; set; }
}
