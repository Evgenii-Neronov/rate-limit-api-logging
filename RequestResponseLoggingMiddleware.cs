public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var logType = context.Request.Path.ToString();

        if (LoggingRateLimiter.ShouldLog(logType))
        {
            await LogRequestResponse(context);
        }
    }

    public async Task LogRequestResponse(HttpContext context)
    {
        var queryString = context.Request.QueryString.ToString();

        if (queryString?.Length > 0)
            _logger.LogInformation($"Query string: {queryString}");

        context.Request.EnableBuffering();

        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        if (requestBody?.Length > 0)
            _logger.LogInformation($"Request body: {requestBody}");

        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        await _next(context);

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
        responseBodyStream.Seek(0, SeekOrigin.Begin);
        await responseBodyStream.CopyToAsync(originalBodyStream);
        context.Response.Body = originalBodyStream;

        if (responseBody?.Length > 0)
            _logger.LogInformation($"Response body: {responseBody}");
    }
}
