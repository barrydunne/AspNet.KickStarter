using AspNet.KickStarter.FunctionalResult.Extensions;
using NUnit.Framework;

namespace AspNet.KickStarter.FunctionalResult.Tests.Extensions;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "IfErrorExtensions")]
internal class BindExtensionsTests
{
    [Test]
    public void Bind_Result_success_Result_success_returns_success()
    {
        Result a() => Result.Success();
        Result b() => Result.Success();

        var bind = a().Bind(b);

        Assert.That(bind.IsSuccess, Is.True);
    }

    [Test]
    public void Bind_Result_error_Result_success_returns_error()
    {
        Result a() => new Error("error.a");
        Result b() => Result.Success();

        var bind = a().Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.a"));
    }

    [Test]
    public void Bind_Result_success_Result_error_returns_error()
    {
        Result a() => Result.Success();
        Result b() => new Error("error.b");

        var bind = a().Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.b"));
    }

    [Test]
    public void Bind_ResultT_success_ResultT_success_returns_success()
    {
        Result<string> a(string input) => $"{input}.a";
        Result<string> b(string input) => $"{input}.b";

        var bind = a("Hello").Bind(b);

        Assert.That(bind.Value, Is.EqualTo("Hello.a.b"));
    }

    [Test]
    public void Bind_ResultT_error_ResultT_success_returns_error()
    {
        Result<string> a(string input) => new Error($"error.{input}.a");
        Result<string> b(string input) => $"{input}.b";

        var bind = a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a"));
    }

    [Test]
    public void Bind_ResultT_success_ResultT_error_returns_error()
    {
        Result<string> a(string input) => $"{input}.a";
        Result<string> b(string input) => new Error($"error.{input}.b");

        var bind = a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a.b"));
    }

    [Test]
    public void Bind_ResultT_success_Result_success_returns_success()
    {
        Result<string> a(string input) => $"{input}.a";
        Result b(string input) => Result.Success();

        var bind = a("Hello").Bind(b);

        Assert.That(bind.IsSuccess, Is.True);
    }

    [Test]
    public void Bind_ResultT_error_Result_success_returns_error()
    {
        Result<string> a(string input) => new Error($"error.{input}.a");
        Result b(string input) => Result.Success();

        var bind = a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a"));
    }

    [Test]
    public void Bind_ResultT_success_Result_error_returns_error()
    {
        Result<string> a(string input) => $"{input}.a";
        Result b(string input) => new Error($"error.{input}.b");

        var bind = a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a.b"));
    }

    [Test]
    public void Bind_Result_success_ResultT_success_returns_success()
    {
        Result a(string input) => Result.Success();
        Result<string> b() => "b";

        var bind = a("Hello").Bind(b);

        Assert.That(bind.Value, Is.EqualTo("b"));
    }

    [Test]
    public void Bind_Result_error_ResultT_success_returns_error()
    {
        Result a(string input) => new Error($"error.{input}.a");
        Result<string> b() => "b";

        var bind = a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a"));
    }

    [Test]
    public void Bind_Result_success_ResultT_error_returns_error()
    {
        Result a(string input) => Result.Success();
        Result<string> b() => new Error($"error.b");

        var bind = a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.b"));
    }

    [Test]
    public async Task Bind_TaskResult_success_Result_success_returns_success()
    {
        async Task<Result> a() => await Task.FromResult(Result.Success());
        Result b() => Result.Success();

        var bind = await a().Bind(b);

        Assert.That(bind.IsSuccess, Is.True);
    }

    [Test]
    public async Task Bind_TaskResult_error_Result_success_returns_error()
    {
        async Task<Result> a() => await Task.FromResult(new Error("error.a"));
        Result b() => Result.Success();

        var bind = await a().Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.a"));
    }

    [Test]
    public async Task Bind_TaskResult_success_Result_error_returns_error()
    {
        async Task<Result> a() => await Task.FromResult(Result.Success());
        Result b() => new Error("error.b");

        var bind = await a().Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.b"));
    }

    [Test]
    public async Task Bind_TaskResultT_success_ResultT_success_returns_success()
    {
        async Task<Result<string>> a(string input) => await Task.FromResult($"{input}.a");
        Result<string> b(string input) => $"{input}.b";

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Value, Is.EqualTo("Hello.a.b"));
    }

    [Test]
    public async Task Bind_TaskResultT_error_ResultT_success_returns_error()
    {
        async Task<Result<string>> a(string input) => await Task.FromResult(new Error($"error.{input}.a"));
        Result<string> b(string input) => $"{input}.b";

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a"));
    }

    [Test]
    public async Task Bind_TaskResultT_success_ResultT_error_returns_error()
    {
        async Task<Result<string>> a(string input) => await Task.FromResult($"{input}.a");
        Result<string> b(string input) => new Error($"error.{input}.b");

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a.b"));
    }

    [Test]
    public async Task Bind_TaskResultT_success_Result_success_returns_success()
    {
        async Task<Result<string>> a(string input) => await Task.FromResult($"{input}.a");
        Result b(string input) => Result.Success();

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.IsSuccess, Is.True);
    }

    [Test]
    public async Task Bind_TaskResultT_error_Result_success_returns_error()
    {
        async Task<Result<string>> a(string input) => await Task.FromResult(new Error($"error.{input}.a"));
        Result b(string input) => Result.Success();

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a"));
    }

    [Test]
    public async Task Bind_TaskResultT_success_Result_error_returns_error()
    {
        async Task<Result<string>> a(string input) => await Task.FromResult($"{input}.a");
        Result b(string input) => new Error($"error.{input}.b");

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a.b"));
    }

    [Test]
    public async Task Bind_TaskResult_success_ResultT_success_returns_success()
    {
        async Task<Result> a(string input) => await Task.FromResult(Result.Success());
        Result<string> b() => "b";

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Value, Is.EqualTo("b"));
    }

    [Test]
    public async Task Bind_TaskResult_error_ResultT_success_returns_error()
    {
        async Task<Result> a(string input) => await Task.FromResult(new Error($"error.{input}.a"));
        Result<string> b() => "b";

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a"));
    }

    [Test]
    public async Task Bind_TaskResult_success_ResultT_error_returns_error()
    {
        async Task<Result> a(string input) => await Task.FromResult(Result.Success());
        Result<string> b() => new Error($"error.b");

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.b"));
    }

    [Test]
    public async Task Bind_Result_success_TaskResult_success_returns_success()
    {
        Result a() => Result.Success();
        async Task<Result> b() => await Task.FromResult(Result.Success());

        var bind = await a().Bind(b);

        Assert.That(bind.IsSuccess, Is.True);
    }

    [Test]
    public async Task Bind_Result_error_TaskResult_success_returns_error()
    {
        Result a() => new Error("error.a");
        async Task<Result> b() => await Task.FromResult(Result.Success());

        var bind = await a().Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.a"));
    }

    [Test]
    public async Task Bind_Result_success_TaskResult_error_returns_error()
    {
        Result a() => Result.Success();
        async Task<Result> b() => await Task.FromResult(new Error("error.b"));

        var bind = await a().Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.b"));
    }

    [Test]
    public async Task Bind_ResultT_success_TaskResultT_success_returns_success()
    {
        Result<string> a(string input) => $"{input}.a";
        async Task<Result<string>> b(string input) => await Task.FromResult($"{input}.b");

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Value, Is.EqualTo("Hello.a.b"));
    }

    [Test]
    public async Task Bind_ResultT_error_TaskResultT_success_returns_error()
    {
        Result<string> a(string input) => new Error($"error.{input}.a");
        async Task<Result<string>> b(string input) => await Task.FromResult($"{input}.b");

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a"));
    }

    [Test]
    public async Task Bind_ResultT_success_TaskResultT_error_returns_error()
    {
        Result<string> a(string input) => $"{input}.a";
        async Task<Result<string>> b(string input) => await Task.FromResult(new Error($"error.{input}.b"));

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a.b"));
    }

    [Test]
    public async Task Bind_ResultT_success_TaskResult_success_returns_success()
    {
        Result<string> a(string input) => $"{input}.a";
        async Task<Result> b(string input) => await Task.FromResult(Result.Success());

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.IsSuccess, Is.True);
    }

    [Test]
    public async Task Bind_ResultT_error_TaskResult_success_returns_error()
    {
        Result<string> a(string input) => new Error($"error.{input}.a");
        async Task<Result> b(string input) => await Task.FromResult(Result.Success());

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a"));
    }

    [Test]
    public async Task Bind_ResultT_success_TaskResult_error_returns_error()
    {
        Result<string> a(string input) => $"{input}.a";
        async Task<Result> b(string input) => await Task.FromResult(new Error($"error.{input}.b"));

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a.b"));
    }

    [Test]
    public async Task Bind_Result_success_TaskResultT_success_returns_success()
    {
        Result a(string input) => Result.Success();
        async Task<Result<string>> b() => await Task.FromResult("b");

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Value, Is.EqualTo("b"));
    }

    [Test]
    public async Task Bind_Result_error_TaskResultT_success_returns_error()
    {
        Result a(string input) => new Error($"error.{input}.a");
        async Task<Result<string>> b() => await Task.FromResult("b");

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a"));
    }

    [Test]
    public async Task Bind_Result_success_TaskResultT_error_returns_error()
    {
        Result a(string input) => Result.Success();
        async Task<Result<string>> b() => await Task.FromResult(new Error($"error.b"));

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.b"));
    }

    [Test]
    public async Task Bind_TaskResult_success_TaskResult_success_returns_success()
    {
        async Task<Result> a() => await Task.FromResult(Result.Success());
        async Task<Result> b() => await Task.FromResult(Result.Success());

        var bind = await a().Bind(b);

        Assert.That(bind.IsSuccess, Is.True);
    }

    [Test]
    public async Task Bind_TaskResult_error_TaskResult_success_returns_error()
    {
        async Task<Result> a() => await Task.FromResult(new Error("error.a"));
        async Task<Result> b() => await Task.FromResult(Result.Success());

        var bind = await a().Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.a"));
    }

    [Test]
    public async Task Bind_TaskResult_success_TaskResult_error_returns_error()
    {
        async Task<Result> a() => await Task.FromResult(Result.Success());
        async Task<Result> b() => await Task.FromResult(new Error("error.b"));

        var bind = await a().Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.b"));
    }

    [Test]
    public async Task Bind_TaskResultT_success_TaskResultT_success_returns_success()
    {
        async Task<Result<string>> a(string input) => await Task.FromResult($"{input}.a");
        async Task<Result<string>> b(string input) => await Task.FromResult($"{input}.b");

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Value, Is.EqualTo("Hello.a.b"));
    }

    [Test]
    public async Task Bind_TaskResultT_error_TaskResultT_success_returns_error()
    {
        async Task<Result<string>> a(string input) => await Task.FromResult(new Error($"error.{input}.a"));
        async Task<Result<string>> b(string input) => await Task.FromResult($"{input}.b");

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a"));
    }

    [Test]
    public async Task Bind_TaskResultT_success_TaskResultT_error_returns_error()
    {
        async Task<Result<string>> a(string input) => await Task.FromResult($"{input}.a");
        async Task<Result<string>> b(string input) => await Task.FromResult(new Error($"error.{input}.b"));

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a.b"));
    }

    [Test]
    public async Task Bind_TaskResultT_success_TaskResult_success_returns_success()
    {
        async Task<Result<string>> a(string input) => await Task.FromResult($"{input}.a");
        async Task<Result> b(string input) => await Task.FromResult(Result.Success());

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.IsSuccess, Is.True);
    }

    [Test]
    public async Task Bind_TaskResultT_error_TaskResult_success_returns_error()
    {
        async Task<Result<string>> a(string input) => await Task.FromResult(new Error($"error.{input}.a"));
        async Task<Result> b(string input) => await Task.FromResult(Result.Success());

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a"));
    }

    [Test]
    public async Task Bind_TaskResultT_success_TaskResult_error_returns_error()
    {
        async Task<Result<string>> a(string input) => await Task.FromResult($"{input}.a");
        async Task<Result> b(string input) => await Task.FromResult(new Error($"error.{input}.b"));

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a.b"));
    }

    [Test]
    public async Task Bind_TaskResult_success_TaskResultT_success_returns_success()
    {
        async Task<Result> a(string input) => await Task.FromResult(Result.Success());
        async Task<Result<string>> b() => await Task.FromResult("b");

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Value, Is.EqualTo("b"));
    }

    [Test]
    public async Task Bind_TaskResult_error_TaskResultT_success_returns_error()
    {
        async Task<Result> a(string input) => await Task.FromResult(new Error($"error.{input}.a"));
        async Task<Result<string>> b() => await Task.FromResult("b");

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.Hello.a"));
    }

    [Test]
    public async Task Bind_TaskResult_success_TaskResultT_error_returns_error()
    {
        async Task<Result> a(string input) => await Task.FromResult(Result.Success());
        async Task<Result<string>> b() => await Task.FromResult(new Error($"error.b"));

        var bind = await a("Hello").Bind(b);

        Assert.That(bind.Error!.Value.Message, Is.EqualTo("error.b"));
    }
}
