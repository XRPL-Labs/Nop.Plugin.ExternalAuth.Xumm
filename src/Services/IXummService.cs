using System.Threading.Tasks;
using XUMM.NET.SDK.Models.Misc;

namespace Nop.Plugin.ExternalAuth.Xumm.Services;

public interface IXummService
{
    Task<XummPong> GetPongAsync();
    bool HasRedirectUrlConfigured(XummPong pong);
    string RedirectUrl { get; }
}
