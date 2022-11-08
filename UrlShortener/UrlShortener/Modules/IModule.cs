namespace UrlShortener.Modules;

public interface IModule
{
    IServiceCollection RegisterModule(IServiceCollection builder, ConfigurationManager configuration);
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}