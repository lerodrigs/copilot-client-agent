using Microsoft.Agents.CopilotStudio.Client;
using Microsoft.Identity.Client;
using copilot_client_shared.Settings;
using System.Net.Http.Headers;

namespace copilot_client_services.Handlers;

public class MyCustomTokenHandler(CopilotConnectionSettings settings) : DelegatingHandler(new HttpClientHandler())
{
    private async Task<AuthenticationResult> AuthenticateAsync(CancellationToken ct = default!)
    {
        var scopes = new string[] { CopilotClient.ScopeFromSettings(settings) };
        ArgumentNullException.ThrowIfNull(settings);
        var app = PublicClientApplicationBuilder.Create(settings.AppClientId)
            .WithAuthority(AzureCloudInstance.AzurePublic, settings.TenantId)
            .WithRedirectUri("Http://localhost")
            .Build();

        IAccount? account = (await app.GetAccountsAsync()).FirstOrDefault();
        AuthenticationResult authResponse;
        try
        {
            authResponse = await app.AcquireTokenSilent(scopes, account).ExecuteAsync(ct);
        }
        catch (MsalUiRequiredException)
        {
            authResponse = await app.AcquireTokenInteractive(scopes).ExecuteAsync(ct);
        }
        return authResponse;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization is null)
        {
            var authResponse = await AuthenticateAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);
        }
            
        return await base.SendAsync(request, cancellationToken);
    }
}
