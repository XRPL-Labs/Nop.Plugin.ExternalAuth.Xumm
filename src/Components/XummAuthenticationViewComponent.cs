using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.ExternalAuth.Xumm.Models;
using Nop.Plugin.ExternalAuth.Xumm.Services;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.Xumm.Components;

/// <summary>
/// Represents view component to display login button
/// </summary>
[ViewComponent(Name = XummExternalAuthenticationDefaults.VIEW_COMPONENT_NAME)]
public class XummAuthenticationViewComponent : NopViewComponent
{
    #region Fields

    private readonly IXummService _xummService;

    #endregion

    #region Ctor

    public XummAuthenticationViewComponent(IXummService xummService)
    {
        _xummService = xummService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <returns>View component result</returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var pong = await _xummService.GetPongAsync();

        var model = new PublicInfoModel
        {
            IsConfigured = _xummService.HasRedirectUrlConfigured(pong)
        };

        return View("~/Plugins/ExternalAuth.Xumm/Views/PublicInfo.cshtml", model);
    }

    #endregion
}
