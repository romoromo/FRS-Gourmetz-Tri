using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FRS.Attributes;
using FRS.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class DesanitizeMiddleware<TModel>
{
    private readonly RequestDelegate _next;

    public DesanitizeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        // Intercept the request here and sanitize the incoming data

        var originalBody = context.Request.Body;

        try
        {
            var content = await ReadRequestBody(context);

            // Deserialize the request body JSON and check if it's ISanitizeable
            var serializer = new JsonSerializer();
            try
            {
                // Deserialize the request body JSON into a JToken
                var deserializedToken = JToken.Parse(content);

                if (deserializedToken.Type == JTokenType.Array)
                {
                    // If deserialization as a single object fails, try deserializing as a list
                    var deserializedObjects = JsonConvert.DeserializeObject<List<TModel>>(content);
                    if (deserializedObjects != null)
                    {
                        // Check if all deserialized objects are of type TModel (implement ISanitizeable)
                        if (deserializedObjects.All(obj => obj is Sanitizeable))
                        {
                            // Sanitize the content
                            var sanitizedContent = DesanitizeString(content);

                            // Replace the original content with the sanitized content
                            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(sanitizedContent));
                        }
                    }
                }
                else
                {
                    var deserializedObject = JsonConvert.DeserializeObject<TModel>(content);
                    if (deserializedObject != null)
                    {
                        // Successfully deserialized as a single object, check if it's ISanitizeable
                        if (deserializedObject is Sanitizeable)
                        {
                            // Sanitize the content
                            var sanitizedContent = DesanitizeString(content);

                            // Replace the original content with the sanitized content
                            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(sanitizedContent));
                        }
                    }
                }
            }
            catch (JsonException ex)
            {

            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
        finally
        {
            // Restore the original request body after processing
            context.Request.Body = originalBody;
        }
    }

    private string DesanitizeString(string input)
    {
        // Implement your custom sanitization logic here
        // For example, you can remove any script tags and other potential harmful content
        // For demonstration purposes, let's just replace "<" and ">" with their HTML entities
        return input?.Replace("&lt;", "<").Replace("&gt;", ">");
    }

    private static async Task<string> ReadRequestBody(HttpContext context)
    {
        var buffer = new MemoryStream();
        await context.Request.Body.CopyToAsync(buffer);
        context.Request.Body = buffer;
        buffer.Position = 0;

        var encoding = Encoding.UTF8;

        var requestContent = await new StreamReader(buffer, encoding).ReadToEndAsync();
        context.Request.Body.Position = 0;

        return requestContent;
    }
}
