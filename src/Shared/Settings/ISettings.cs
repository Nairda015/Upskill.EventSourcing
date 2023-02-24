using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Settings;

public interface ISettings
{
    static abstract string SectionName { get; }
}

public static class SettingsExtensions
{
    public static void RegisterOptions<TOptions>(this WebApplicationBuilder builder)
        where TOptions : class, ISettings
    {
        var section = builder.Configuration.GetSection(TOptions.SectionName);
        builder.Services.AddOptions<TOptions>().Bind(section);
    }
    
    public static void RegisterOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class, ISettings
    {
        var section = configuration.GetSection(TOptions.SectionName);
        services.AddOptions<TOptions>().Bind(section);
    }
    
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration)
        where TOptions : ISettings, new()
    {
        var options = new TOptions();
        configuration.GetSection(TOptions.SectionName).Bind(options);
        return options;
    }
    
    
    // public static void RegisterSettings<TMarker>(this IServiceCollection services, IConfiguration configuration)
    // {
    //     var assembly = Assembly.GetAssembly(typeof(TMarker));
    //     var moduleSettings = assembly!
    //         .GetTypes()
    //         .Where(x => typeof(IOptions).IsAssignableFrom(x) && x.IsClass)
    //         .Select(Activator.CreateInstance)
    //         .Cast<IOptions>()
    //         .ToList();
    //     
    //     moduleSettings.ForEach(x => configuration.GetSection(x.SectionName).Bind(x));
    //     
    //     services.Configure<TOptions>(configuration);
    //
    //     moduleSettings.ForEach(x =>
    //     {
    //         var section = configuration.GetSection(x.SectionName);
    //         services.AddOptions<>().Bind(section);
    //     });
    // }

}