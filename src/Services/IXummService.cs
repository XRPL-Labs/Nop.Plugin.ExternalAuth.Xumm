using System.Threading.Tasks;
using XUMM.NET.SDK.Models.Misc;

namespace Nop.Plugin.ExternalAuth.Xumm.Services;

public interface IXummService
{
    Task<XummPong> GetPongAsync(int storeId);
    Task<bool> HasRedirectUrlConfiguredAsync(int storeId, XummPong pong);
    Task<string> GetRedirectUrlAsync(int storeId);
}
