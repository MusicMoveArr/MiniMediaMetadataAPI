using Microsoft.Extensions.Options;
using MiniMediaMetadataAPI.Options;
using Prometheus;

namespace MiniMediaMetadataAPI.Middlewares;

public class RequestMiddleware  
{  
    private readonly RequestDelegate _next;  
    private readonly ILogger _logger;  
    private readonly PrometheusOptions _prometheusOptions;
  
    public RequestMiddleware(  
        RequestDelegate next  
        , ILoggerFactory loggerFactory
        , IOptions<PrometheusOptions> prometheusOptions
    )  
    {  
        _next = next;  
        _logger = loggerFactory.CreateLogger<RequestMiddleware>();
        _prometheusOptions = prometheusOptions.Value;
    }  
      
    public async Task Invoke(HttpContext httpContext)  
    {  
        var path = httpContext.Request.Path.Value;  
        var method = httpContext.Request.Method;  
  
        var counter = Metrics.CreateCounter("MiniMediaMetadataAPI_request_total", "HTTP Requests Total", new CounterConfiguration  
        {  
            LabelNames = new[] { "path", "method", "status" }  
        });  
  
        var statusCode = 200;  
  
        try  
        {  
            await _next.Invoke(httpContext);  
        }  
        catch (Exception)  
        {  
            statusCode = 500;  
            counter.Labels(path, method, statusCode.ToString()).Inc();  
  
            throw;  
        }  
          
        if (path != _prometheusOptions.MetricsUrl)  
        {  
            statusCode = httpContext.Response.StatusCode;  
            counter.Labels(path, method, statusCode.ToString()).Inc();  
        }  
    }  
}  
  
public static class RequestMiddlewareExtensions  
{          
    public static IApplicationBuilder UseRequestMiddleware(this IApplicationBuilder builder)  
    {  
        return builder.UseMiddleware<RequestMiddleware>();  
    }  
}  