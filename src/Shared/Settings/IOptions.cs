using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Settings;

public interface IOptions
{
    static abstract string SectionName { get; }
}

public static class SettingsExtensions
{
    public static void RegisterOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class, IOptions
    {
        var section = configuration.GetSection(TOptions.SectionName);
        services.AddOptions<TOptions>().Bind(section);
    }
    
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration)
        where TOptions : IOptions, new()
    {
        var options = new TOptions();
        configuration.GetSection(TOptions.SectionName).Bind(options);
        return options;
    }
}