using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        using (LogContext.PushProperty("TraceId", context.TraceIdentifier))
        using (LogContext.PushProperty("Path", context.Request.Path.ToString()))
        using (LogContext.PushProperty("Method", context.Request.Method))
        {
            try
            {
               

                await _next(context);
                sw.Stop();

                _logger.LogInformation("HTTP {Method} {Path} -> {StatusCode} in {Elapsed} ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    sw.ElapsedMilliseconds);
            }
            catch
            {
                sw.Stop();
                // exception will be logged by ExceptionMiddleware
                throw;
            }
        }
    }
}
