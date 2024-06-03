using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AspNet.KickStarter.Core.Tests.AddIns;

public class PrivateAccessors
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_hostApplicationBuilder")]
    public static extern ref HostApplicationBuilder GetHostApplicationBuilder(WebApplicationBuilder webApplicationBuilder);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<ConfigureHostBuilder>k__BackingField")]
    public static extern ref ConfigureHostBuilder GetConfigureHostBuilder(WebApplicationBuilder webApplicationBuilder);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_serviceCollection")]
    public static extern ref ServiceCollection GetServiceCollection(HostApplicationBuilder hostApplicationBuilder);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_services")]
    public static extern ref IServiceCollection GetServiceCollection(ConfigureHostBuilder hostApplicationBuilder);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_dataSources")]
    public static extern ref List<EndpointDataSource> GetDataSources(WebApplication webApplication);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<ApplicationBuilder>k__BackingField")]
    public static extern ref ApplicationBuilder GetApplicationBuilder(WebApplication webApplication);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_components")]
    public static extern ref List<Func<RequestDelegate, RequestDelegate>> GetComponents(ApplicationBuilder applicationBuilder);

    // Can't use inacessible types
    //[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_dataSources")]
    //public static extern ref List<RouteEntry> GetRouteEntries(RouteEndpointDataSource routeEndpointDataSource);
    public static List<RouteEntryStub> GetRouteEntries(object routeEndpointDataSource)
    {
        List<RouteEntryStub> routeEntries = new();
        var field = routeEndpointDataSource?.GetType()?.GetField("_routeEntries", BindingFlags.Instance | BindingFlags.NonPublic);
        var value = field?.GetValue(routeEndpointDataSource);
        var list = value as IList;
        if (list is not null)
        {
            foreach (var item in list)
            {
                var routePattern = item?.GetType()?.GetProperty("RoutePattern", BindingFlags.Instance | BindingFlags.Public)?.GetValue(item);
                if (routePattern is not null)
                    routeEntries.Add(new() { RoutePattern = (RoutePattern)routePattern });
            }
        }
        return routeEntries;
    }

    public readonly struct RouteEntryStub
    {
        public required RoutePattern RoutePattern { get; init; }
    }
}
