using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNet.KickStarter.Tests.AddIns;

internal static class AddInExtensions
{
    internal static T? GetAddIn<T>(this ApiBuilder apiBuilder) where T : class,IAddIn
        => apiBuilder.AddIns.FirstOrDefault(_ => _.GetType() == typeof(T)) as T;

    internal static void SetServices(this WebApplicationBuilder builder, ServiceCollection services)
    {
        var _hostApplicationBuilder = PrivateAccessors.GetHostApplicationBuilder(builder);
        PrivateAccessors.GetServiceCollection(_hostApplicationBuilder) = services;
    }

    internal static void SetHostServices(this WebApplicationBuilder builder, ServiceCollection services)
        => PrivateAccessors.GetServiceCollection(builder.Host) = services;
}
