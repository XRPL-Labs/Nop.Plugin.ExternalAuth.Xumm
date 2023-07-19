using System.Threading.Tasks;
using AspNet.Security.OAuth.Xumm;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Xumm.Infrastructure;

/// <summary>
/// Represents registrar of Xumm authentication service
/// </summary>
public class XummAuthenticationRegistrar : IExternalAuthenticationRegistrar
{
    /// <summary>
    /// Configure
    /// </summary>
    /// <param name="builder">Authentication builder</param>
    public void Configure(AuthenticationBuilder builder)
    {
        builder.AddXumm(XummAuthenticationDefaults.AuthenticationScheme, options =>
        {
            //set credentials
            var settings = EngineContext.Current.Resolve<XummExternalAuthenticationSettings>();
            options.ClientId = string.IsNullOrEmpty(settings?.ApiKey) ? nameof(options.ClientId) : settings.ApiKey;
            options.ClientSecret = string.IsNullOrEmpty(settings?.ApiSecret) ? nameof(options.ClientSecret) : settings.ApiSecret;

            //store access and refresh tokens for the further usage
            options.SaveTokens = true;

            //set custom events handlers
            options.Events = new OAuthEvents
            {
                //in case of error, redirect the user to the specified URL
                OnRemoteFailure = context =>
                {
                    context.HandleResponse();

                    // We try to get the Error callback based on the state, otherwise redirect back to the login page.
                    var errorUrl = context.Properties?.GetString(XummExternalAuthenticationDefaults.ErrorCallback) ?? "/Login";
                    context.Response.Redirect(errorUrl);

                    return Task.FromResult(0);
                }
            };
        });
    }
}
