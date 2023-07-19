using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.ExternalAuth.Xumm.Models;
using Nop.Plugin.ExternalAuth.Xumm.Services;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.ExternalAuth.Xumm.Components;

/// <summary>
/// Represents view component to display login button
/// </summary>
public class XummAuthenticationViewComponent : NopViewComponent
{
    #region Fields

    private readonly IStoreContext _storeContext;
    private readonly IXummService _xummService;

    #endregion

    #region Ctor

    public XummAuthenticationViewComponent(
        IStoreContext storeContext,
        IXummService xummService)
    {
        _storeContext = storeContext;
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
        var store = await _storeContext.GetCurrentStoreAsync();
        var pong = await _xummService.GetPongAsync(store.Id);

        var model = new PublicInfoModel
        {
            IsConfigured = await _xummService.HasRedirectUrlConfiguredAsync(store.Id, pong)
        };

        return View("~/Plugins/ExternalAuth.Xumm/Views/PublicInfo.cshtml", model);
    }

    #endregion
}
