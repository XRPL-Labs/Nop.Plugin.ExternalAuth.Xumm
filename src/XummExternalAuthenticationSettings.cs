using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.Xumm;

/// <summary>
/// Represents settings of the Xumm authentication method
/// </summary>
public class XummExternalAuthenticationSettings : ISettings
{
    #region Properties

    /// <summary>
    /// API Key which can be obtained from the Xumm Developer Console
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// API Secret which can be obtained from the Xumm Developer Console
    /// </summary>
    public string ApiSecret { get; set; }

    #endregion
}
