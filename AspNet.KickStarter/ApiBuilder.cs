using Microsoft.AspNetCore.Builder;

namespace AspNet.KickStarter;

/// <summary>
/// Simplifies the repetitive steps in creating a WebApplication for an AspNet API.
/// </summary>
public class ApiBuilder
{
    private readonly List<IAddIn> _addIns;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiBuilder"/> class.
    /// </summary>
    public ApiBuilder() => _addIns = new();

    /// <summary>
    /// Gets the currently registered add-ins.
    /// </summary>
    internal IReadOnlyCollection<IAddIn> AddIns => _addIns.AsReadOnly();

    /// <summary>
    /// Register a new add-in for the builder to use.
    /// </summary>
    /// <param name="addIn">The add-in to use.</param>
    internal void RegisterAddIn(IAddIn addIn) => _addIns.Add(addIn);

    /// <summary>
    /// Builds the <see cref="WebApplication"/>.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>The configured <see cref="WebApplication"/> ready to run.</returns>
    public WebApplication Build(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        foreach (var addIn in _addIns)
            addIn.Configure(builder);

        var app = builder.Build();
        foreach (var addIn in _addIns)
            addIn.Configure(app);

        return app;
    }
}
