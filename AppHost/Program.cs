var builder = DistributedApplication.CreateBuilder(args);

// Get Keycloak username and password from appsettings.json
var username = builder.AddParameter("username");
var password = builder.AddParameter("password", secret: true);
// Add Keyclock service
IResourceBuilder<KeycloakResource> keycloak = builder.AddKeycloak("keycloak", 8080, username, password)
                    // .WithImageTag("25.0")
                    .WithDataVolume()
                    .WithRealmImport("./Realm/KeycloakRealmConfig.json");

String launchProfileName = "http";

var webApp = builder.AddProject<Projects.BlazorApp>("BlazorApp", launchProfileName)
    .WithExternalHttpEndpoints()
    .WithReference(keycloak)
    .WaitFor(keycloak);

// Adding BlazorwebIndividualAccount Project
var webAppWithIndividualAccount = builder.AddProject<Projects.BlazorWebIndividualAccountAuth>("BlazorWebIndividualAccountAuth", launchProfileName)
    .WithExternalHttpEndpoints()
    .WithReference(keycloak)
    .WaitFor(keycloak);

// Adding BlazorwebAssemblyIndividualAccount Project
var webAssembly = builder.AddProject<Projects.BlazorWebAssemblyIndAccAuth>("BlazorWebAssemblyIndAccAuth", launchProfileName)
    .WithExternalHttpEndpoints()
    .WithReference(keycloak)
    .WaitFor(keycloak);

// Wire up the callback urls (self reference)
// webApp.WithEnvironment("CallBackUrl", webApp.GetEndpoint("http").ToString());

builder.Build().Run();
