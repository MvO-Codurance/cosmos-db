using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace UrlShortner.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime, ITestOutputHelperAccessor 
{
    public string BaseUri { get; set; } = string.Empty;

    public ITestOutputHelper? OutputHelper { get; set; }

    public string BuildUri()
    {
        return BuildUri(string.Empty);
    }

    public string BuildUri(string relativeUri)
    {
        const string prefix = ""; 
        return CombineUri(prefix, BaseUri, relativeUri);
    }

    public async Task DumpResponse(HttpResponseMessage response)
    {
        StringBuilder sb = new();
        sb.AppendLine();
        sb.AppendLine("=============================================================");
        sb.AppendLine(await response.Content.ReadAsStringAsync());
        sb.AppendLine();

        OutputHelper?.WriteLine(sb.ToString());
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }
  
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddXUnit(this);
        });
        
        builder?.ConfigureServices(services =>
        {
            // remove & add services
            // ...
            
            var sp = services.BuildServiceProvider();

            // do initialisation
            // ...
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);

        // add "accept JSON" header to each request made to the SUT
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
    }
    
    private static string CombineUri(string baseUri, params string[] segments)
    {
        IEnumerable<string> cleanSegments = segments.Where(x => !string.IsNullOrWhiteSpace(x));
        return string.Join("/", new[] { baseUri.TrimEnd('/') }.Concat(cleanSegments.Select(s => s.Trim('/'))));
    }
}