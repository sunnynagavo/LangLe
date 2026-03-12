var builder = DistributedApplication.CreateBuilder(args);

// Default to the executable runner, but allow environment or config to override it.
var useExecutableProjectRunnerFallback = true;

// Environment variable takes precedence so the hosting mode can be changed without code edits.
if (bool.TryParse(
        Environment.GetEnvironmentVariable("LANGLE_USE_EXECUTABLE_PROJECT_RUNNER"),
        out var useFallbackFromEnvironment))
{
    useExecutableProjectRunnerFallback = useFallbackFromEnvironment;
}
else if (bool.TryParse(
             builder.Configuration["LangLe:UseExecutableProjectRunnerFallback"],
             out var useFallbackFromConfiguration))
{
    useExecutableProjectRunnerFallback = useFallbackFromConfiguration;
}

// Persistent Postgres instance for local development data.
var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("langdb");

if (useExecutableProjectRunnerFallback)
{
    // Run the API explicitly via `dotnet run` when using the fallback hosting path.
    var apiService = builder.AddExecutable(
            "apiservice",
            "dotnet",
            @"..\LangLe.ApiService",
            "run",
            "--project",
            "LangLe.ApiService.csproj",
            "--launch-profile",
            "https")
        .WithReference(postgres)
        .WaitFor(postgres)
        // Keep API ports predictable during local development.
        .WithHttpEndpoint(port: 5416, targetPort: 5416, name: "http", isProxied: false)
        .WithHttpsEndpoint(port: 7463, targetPort: 7463, name: "https", isProxied: false)
        .WithHttpHealthCheck("/health");

    // Start the Blazor frontend separately and pass in the API endpoints it depends on.
    builder.AddExecutable(
            "webfrontend",
            "dotnet",
            @"..\LangLe.Web",
            "run",
            "--project",
            "LangLe.Web.csproj",
            "--launch-profile",
            "https")
        .WithReference(apiService.GetEndpoint("https"))
        .WithReference(apiService.GetEndpoint("http"))
        .WaitFor(apiService)
        .WithHttpEndpoint(port: 5043, targetPort: 5043, name: "http", isProxied: false)
        .WithHttpsEndpoint(port: 7176, targetPort: 7176, name: "https", isProxied: false)
        .WithHttpHealthCheck("/health");
}
else
{
    // Preferred Aspire path: register projects directly and let Aspire manage them.
    var apiService = builder.AddProject<Projects.LangLe_ApiService>("apiservice")
        .WithReference(postgres)
        .WaitFor(postgres)
        .WithHttpHealthCheck("/health");

    builder.AddProject<Projects.LangLe_Web>("webfrontend")
        .WithExternalHttpEndpoints()
        .WithHttpHealthCheck("/health")
        .WithReference(apiService)
        .WaitFor(apiService);
}

builder.Build().Run();
