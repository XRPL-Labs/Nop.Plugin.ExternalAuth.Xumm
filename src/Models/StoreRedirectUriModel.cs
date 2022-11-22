namespace Nop.Plugin.ExternalAuth.Xumm.Models;

public class StoreRedirectUriModel
{
    #region Properties

    public string StoreName { get; set; } = default!;

    public string RedirectUri { get; set; } = default!;

    public bool Configured { get; set; }

    #endregion
}
