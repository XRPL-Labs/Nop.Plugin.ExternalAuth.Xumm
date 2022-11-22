using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.ExternalAuth.Xumm.Models;
using XUMM.NET.SDK.Models.Misc;

namespace Nop.Plugin.ExternalAuth.Xumm.Services;

public interface IXummService
{
    Task<XummPong> GetPongAsync(int storeId = 0);
    Task<bool> HasRedirectUrlConfiguredAsync(int storeId, XummPong pong);
    Task<string> GetRedirectUrlAsync(int storeId);
    Task<List<StoreRedirectUriModel>> GetStoreRedirectUrisAsync(XummPong pong);
}
