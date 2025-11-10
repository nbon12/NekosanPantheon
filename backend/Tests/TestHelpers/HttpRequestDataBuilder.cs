using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;

namespace HelloWorldFunctionApp.Tests.TestHelpers;

public class TestHttpRequestData : HttpRequestData
{
    private readonly MemoryStream _bodyStream;
    private readonly HttpHeadersCollection _headers;
    private readonly FunctionContext _functionContext;

    public TestHttpRequestData(FunctionContext functionContext, string method, string body) 
        : base(functionContext)
    {
        _functionContext = functionContext;
        _bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(body));
        _headers = new HttpHeadersCollection();
        Method = method;
    }

    public override Stream Body => _bodyStream;
    public override HttpHeadersCollection Headers => _headers;
    public override IReadOnlyCollection<IHttpCookie> Cookies => new List<IHttpCookie>();
    public override Uri Url { get; } = new Uri("http://localhost/api/HelloWorld");
    public override IEnumerable<ClaimsIdentity> Identities => Enumerable.Empty<ClaimsIdentity>();
    public string Method { get; }

    public override HttpResponseData CreateResponse()
    {
        return new TestHttpResponseData(_functionContext);
    }

    public override HttpResponseData CreateResponse(HttpStatusCode statusCode)
    {
        return new TestHttpResponseData(_functionContext) { StatusCode = statusCode };
    }
}

public class TestHttpResponseData : HttpResponseData
{
    private readonly MemoryStream _bodyStream;
    private readonly HttpHeadersCollection _headers;

    public TestHttpResponseData(FunctionContext functionContext) 
        : base(functionContext)
    {
        _bodyStream = new MemoryStream();
        _headers = new HttpHeadersCollection();
        StatusCode = HttpStatusCode.OK;
    }

    public override HttpStatusCode StatusCode { get; set; }
    public override HttpHeadersCollection Headers => _headers;
    public override Stream Body => _bodyStream;
    public override HttpCookies Cookies => new HttpCookies();

    public string GetBody()
    {
        _bodyStream.Position = 0;
        return Encoding.UTF8.GetString(_bodyStream.ToArray());
    }
}

public class TestFunctionContext : FunctionContext
{
    public override IServiceProvider InstanceServices { get; set; } = null!;
    public override FunctionDefinition FunctionDefinition { get; } = null!;
    public override IDictionary<object, object> Items { get; set; } = new Dictionary<object, object>();
    public override IInvocationFeatures Features { get; } = new Mock<IInvocationFeatures>().Object;
    public override string InvocationId { get; } = Guid.NewGuid().ToString();
    public override string FunctionId { get; } = Guid.NewGuid().ToString();
    public override TraceContext TraceContext { get; } = new Mock<TraceContext>().Object;
    public override BindingContext BindingContext { get; } = new Mock<BindingContext>().Object;
    public override RetryContext RetryContext { get; } = new Mock<RetryContext>().Object;
}

