using AutoFixture;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection;

namespace AspNet.KickStarter.Tests.HttpHandlers.HealthHandler;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "HttpHandler")]
internal class HealthHandlerTests
{
    private readonly HealthHandlerTestsContext _context = new();
    private readonly Fixture _fixture = new();

    [Test]
    public void HealthHandler_GetStatus_returns_OK()
    {
        var result = _context.Sut.GetStatus();
        Assert.That(result, Is.TypeOf<Ok<string>>());
    }

    [Test]
    public void HealthHandler_GetStatus_returns_GOOD()
    {
        var result = _context.Sut.GetStatus();
        Assert.That((result as Ok<string>)?.Value, Is.EqualTo("GOOD"));
    }

    [Test]
    public async Task HealthHandler_GetVersionAsync_returns_file_version_if_exists()
    {
        var version = _fixture.Create<string>();
        _context.WithFileVersion(version);
        var result = await _context.Sut.GetVersionAsync();
        Assert.That((result as Ok<string>)?.Value, Is.EqualTo(version));
    }

    [Test]
    public async Task HealthHandler_GetVersionAsync_returns_assembly_version_if_file_does_not_exist()
    {
        var result = await _context.Sut.GetVersionAsync();
        Assert.That((result as Ok<string>)?.Value, Is.EqualTo(_context.AssemblyVersion));
    }

    [Test]
    public async Task HealthHandler_GetVersionAsync_returns_UNKNOWN_on_exception()
    {
        _context.WithGetVersionAsyncException();
        var result = await _context.Sut.GetVersionAsync();
        Assert.That((result as Ok<string>)?.Value, Is.EqualTo("UNKNOWN"));
    }

    [Test]
    public void HealthHandler_uses_entry_assembly_path_if_no_assembly_provided()
    {
        _context.WithoutAssembly();
        var path = _context.Sut.VersionFilePath;
        var expected = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "version.txt");
        Assert.That(path, Is.EqualTo(expected));
    }
}
