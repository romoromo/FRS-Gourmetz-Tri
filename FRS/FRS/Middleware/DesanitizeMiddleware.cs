using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class DesanitizeMiddleware<TModel>
{
    private readonly RequestDelegate _next;

    public DesanitizeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Intercept the request here and sanitize the incoming data

        var originalBody = context.Request.Body;

        try
        {
            // Replace the request body with a MemoryStream to read the content
            using (var memoryStream = new MemoryStream())
            {
                await context.Request.Body.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Read the content from the MemoryStream
                var content = await new StreamReader(memoryStream).ReadToEndAsync();

                // Sanitize the content
                var sanitizedContent = DesanitizeString(content);

                // Replace the original content with the sanitized content
                context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(sanitizedContent));

                // Call the next middleware in the pipeline
                await _next(context);
            }
        }
        finally
        {
            // Restore the original request body after processing
            context.Request.Body = originalBody;
        }
    }

    // Implement your own sanitization logic here
    private string DesanitizeString(string input)
    {
        // Implement your custom sanitization logic here
        // For example, you can remove any script tags and other potential harmful content
        // For demonstration purposes, let's just replace "<" and ">" with their HTML entities
        return input?.Replace("&lt;", "<").Replace("&gt;", ">");
    }
}
