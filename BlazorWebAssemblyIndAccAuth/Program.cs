using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWebAssemblyIndAccAuth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOidcAuthentication(options =>
{
    // Configure your authentication provider options here.
    // For more information, see https://aka.ms/blazor-standalone-auth
    // builder.Configuration.Bind("Local", options.ProviderOptions);

    // Adding Keycloak specific options
    options.ProviderOptions.Authority = "http://localhost:8080/realms/CustomRealmName";
    options.ProviderOptions.ClientId = "frontend";
    options.ProviderOptions.ResponseType = "code"; // Authorization Code flow
    // Optional: If using custom scope for audience, request it
    // options.ProviderOptions.DefaultScopes.Add("myapi_scope");
});

await builder.Build().RunAsync();
