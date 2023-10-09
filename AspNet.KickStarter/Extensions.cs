using Microsoft.AspNetCore.Builder;

namespace AspNet.KickStarter
{
    /// <summary>
    /// Provides public extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Enable fluent configuration of the WebApplication as part of the <see cref="ApiBuilder"/> class between Build and Run.
        /// </summary>
        /// <param name="app">The application to configure.</param>
        /// <param name="additionalConfiguration">The additional configuration to perform.</param>
        /// <returns>The original WebApplication.</returns>
        public static WebApplication WithApplicationConfiguration(this WebApplication app, Action<WebApplication> additionalConfiguration)
        {
            additionalConfiguration(app);
            return app;
        }
    }
}
