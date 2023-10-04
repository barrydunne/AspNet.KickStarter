using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace AspNet.KickStarter
{
    /// <summary>
    /// Consolidates the extensions app.MapXXX().WithName().WithDescription().WithOpenApi().
    /// </summary>
    public static class EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP DELETE requests
        /// for the specified pattern.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointConventionBuilder MapDelete(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, RequestDelegate requestDelegate)
            => endpoints.MapDelete(pattern, requestDelegate).WithName(name).WithDescription(description).WithOpenApi();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP DELETE requests
        /// for the specified pattern.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="handler">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
        public static RouteHandlerBuilder MapDelete(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, Delegate handler)
            => endpoints.MapDelete(pattern, handler).WithName(name).WithDescription(description).WithOpenApi();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP DELETE requests
        /// for the specified pattern.
        /// </summary>
        /// <typeparam name="TResponse">The type of response from this endpoint.</typeparam>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="handler">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
        public static RouteHandlerBuilder MapDelete<TResponse>(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, Delegate handler)
            => endpoints.MapDelete(pattern, handler).WithName(name).WithDescription(description).WithOpenApi().Produces<TResponse>();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP GET requests
        /// for the specified pattern.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointConventionBuilder MapGet(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, RequestDelegate requestDelegate)
            => endpoints.MapGet(pattern, requestDelegate).WithName(name).WithDescription(description).WithOpenApi();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP GET requests
        /// for the specified pattern.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="handler">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
        public static RouteHandlerBuilder MapGet(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, Delegate handler)
            => endpoints.MapGet(pattern, handler).WithName(name).WithDescription(description).WithOpenApi();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP GET requests
        /// for the specified pattern.
        /// </summary>
        /// <typeparam name="TResponse">The type of response from this endpoint.</typeparam>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="handler">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
        public static RouteHandlerBuilder MapGet<TResponse>(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, Delegate handler)
            => endpoints.MapGet(pattern, handler).WithName(name).WithDescription(description).WithOpenApi().Produces<TResponse>();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP PATCH requests
        /// for the specified pattern.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointConventionBuilder MapPatch(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, RequestDelegate requestDelegate)
            => endpoints.MapPatch(pattern, requestDelegate).WithName(name).WithDescription(description).WithOpenApi();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP PATCH requests
        /// for the specified pattern.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="handler">The <see cref="Delegate" /> executed when the endpoint is matched.</param>
        /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
        public static RouteHandlerBuilder MapPatch(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, Delegate handler)
            => endpoints.MapPatch(pattern, handler).WithName(name).WithDescription(description).WithOpenApi();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP PATCH requests
        /// for the specified pattern.
        /// </summary>
        /// <typeparam name="TResponse">The type of response from this endpoint.</typeparam>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="handler">The <see cref="Delegate" /> executed when the endpoint is matched.</param>
        /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
        public static RouteHandlerBuilder MapPatch<TResponse>(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, Delegate handler)
            => endpoints.MapPatch(pattern, handler).WithName(name).WithDescription(description).WithOpenApi().Produces<TResponse>();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP POST requests
        /// for the specified pattern.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointConventionBuilder MapPost(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, RequestDelegate requestDelegate)
            => endpoints.MapPost(pattern, requestDelegate).WithName(name).WithDescription(description).WithOpenApi();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP POST requests
        /// for the specified pattern.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="handler">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
        public static RouteHandlerBuilder MapPost(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, Delegate handler)
            => endpoints.MapPost(pattern, handler).WithName(name).WithDescription(description).WithOpenApi();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP POST requests
        /// for the specified pattern.
        /// </summary>
        /// <typeparam name="TResponse">The type of response from this endpoint.</typeparam>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="handler">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
        public static RouteHandlerBuilder MapPost<TResponse>(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, Delegate handler)
            => endpoints.MapPost(pattern, handler).WithName(name).WithDescription(description).WithOpenApi().Produces<TResponse>();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP PUT requests
        /// for the specified pattern.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointConventionBuilder MapPut(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, RequestDelegate requestDelegate)
            => endpoints.MapPut(pattern, requestDelegate).WithName(name).WithDescription(description).WithOpenApi();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP PUT requests
        /// for the specified pattern.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="handler">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
        public static RouteHandlerBuilder MapPut(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, Delegate handler)
            => endpoints.MapPut(pattern, handler).WithName(name).WithDescription(description).WithOpenApi();

        /// <summary>
        /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP PUT requests
        /// for the specified pattern.
        /// </summary>
        /// <typeparam name="TResponse">The type of response from this endpoint.</typeparam>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="name">The endpoint name.</param>
        /// <param name="description">A string representing a detailed description of the endpoint.</param>
        /// <param name="handler">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
        public static RouteHandlerBuilder MapPut<TResponse>(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, string name, string description, Delegate handler)
            => endpoints.MapPut(pattern, handler).WithName(name).WithDescription(description).WithOpenApi().Produces<TResponse>();
    }
}
