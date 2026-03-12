var builder = DistributedApplication.CreateBuilder(args);

var configFallbackEnabled = bool.TryParse(
    builder.Configuration["LangLe:UseExecutableProjectRunnerFallback"],
    out var useConfiguredFallback) && useConfiguredFallback;

var useExecutableProjectRunnerFallback =
    configFallbackEnabled ||
    string.Equals(
        Environment.GetEnvironmentVariable("LANGLE_USE_EXECUTABLE_PROJECT_RUNNER"),
        "true",
        StringComparison.OrdinalIgnoreCase);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("langdb");

if (useExecutableProjectRunnerFallback)
{
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
        .WithHttpEndpoint(port: 5416, targetPort: 5416, name: "http", isProxied: false)
        .WithHttpsEndpoint(port: 7463, targetPort: 7463, name: "https", isProxied: false)
        .WithHttpHealthCheck("/health");

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
