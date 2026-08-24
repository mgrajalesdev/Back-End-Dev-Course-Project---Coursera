namespace UserManagementApi.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 1. Capture information before the request proceeds
            string method = context.Request.Method;
            string path = context.Request.Path;

            await _next(context); // Call the next delegate/middleware in the pipeline

            // 2. Capture status code after the request has been processed
            int statusCode = context.Response.StatusCode;

            // 3. Log the details
            _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode}", method, path, statusCode);
        }
    }
}