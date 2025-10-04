using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ProductManagement.UnitTests.TestHelpers;

// Minimal test double for FunctionContext so we can construct HttpRequestData
public class TestFunctionContext : FunctionContext
{
    private IDictionary<object, object> _items = new Dictionary<object, object>();

    public override string InvocationId { get; } = Guid.NewGuid().ToString();
    public override string FunctionId { get; } = Guid.NewGuid().ToString();
    public override TraceContext TraceContext { get; } = null!; // Not required for tests
    public override BindingContext BindingContext { get; } = null!; // Not required
    public override IServiceProvider InstanceServices { get; set; } = new SimpleServiceProvider();
    public override FunctionDefinition FunctionDefinition { get; } = null!; // Not required
    public override IInvocationFeatures Features { get; } = new InvocationFeatures();
    public override IDictionary<object, object> Items { get => _items; set => _items = value; }
    public override RetryContext RetryContext => null!; // Not used in tests
}

public class InvocationFeatures : IInvocationFeatures
{
    private readonly Dictionary<Type, object> _features = new();

    public T Get<T>()
    {
        if (_features.TryGetValue(typeof(T), out var value))
        {
            return (T)value;
        }
        return default!; // For tests we return default if not set
    }

    public void Set<T>(T instance)
    {
        _features[typeof(T)] = instance!;
    }

    public IEnumerator<KeyValuePair<Type, object>> GetEnumerator() => _features.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _features.GetEnumerator();
}

public class SimpleServiceProvider : IServiceProvider
{
    public object? GetService(Type serviceType) => null; // Manual JSON serialization removes need for services
}

public class TestHttpRequestData : HttpRequestData
{
    private readonly MemoryStream _bodyStream;
    public TestHttpRequestData(FunctionContext context, string method = "GET", string body = "", string url = "https://localhost/test") : base(context)
    {
        Method = method;
        Url = new Uri(url);
        _bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(body));
        Headers = new HttpHeadersCollection();
        _cookies = new List<IHttpCookie>();
    }

    public override Stream Body => _bodyStream;
    public override HttpHeadersCollection Headers { get; }
    private readonly List<IHttpCookie> _cookies;
    public override IReadOnlyCollection<IHttpCookie> Cookies => _cookies;
    public override Uri Url { get; }
    public override IEnumerable<ClaimsIdentity> Identities => Enumerable.Empty<ClaimsIdentity>();
    public override string Method { get; }
    public override HttpResponseData CreateResponse() => new TestHttpResponseData(FunctionContext);
}

public class TestHttpResponseData : HttpResponseData
{
    private Stream _body;
    private HttpHeadersCollection _headers;

    public TestHttpResponseData(FunctionContext context) : base(context)
    {
        _body = new MemoryStream();
        _headers = new HttpHeadersCollection();
    }

    public override HttpStatusCode StatusCode { get; set; }
    public override HttpHeadersCollection Headers { get => _headers; set => _headers = value; }
    public override Stream Body { get => _body; set => _body = value; }
    public override HttpCookies Cookies { get; } = null!; // Not needed
}
