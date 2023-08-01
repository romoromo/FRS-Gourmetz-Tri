using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FRS.Middleware
{
    public class AntiXssMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AntiXssMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // enable buffering so that the request can be read by the model binders next
            httpContext.Request.EnableBuffering();

            // Check XSS in URL
            if (!string.IsNullOrWhiteSpace(httpContext.Request.Path.Value))
            {
                var url = httpContext.Request.Path.Value;

                if (CrossSiteScriptingValidation.IsDangerousString(url, out _))
                {
                    throw new BadRequestException("XSS injection detected from middleware.");
                }
            }

            // Check XSS in query string
            if (!string.IsNullOrWhiteSpace(httpContext.Request.QueryString.Value))
            {
                var queryString = WebUtility.UrlDecode(httpContext.Request.QueryString.Value);

                if (CrossSiteScriptingValidation.IsDangerousString(queryString, out _))
                {
                    throw new BadRequestException("XSS injection detected from middleware.");
                }
            }

            // Check XSS in request content
            var originalBody = httpContext.Request.Body;
            try
            {
                var allowedContentTypes = _configuration["AppSettings:WHITELISTED_CONTENT_TYPE"].Split(";");
                bool allowed = false;
                foreach(var contentType in allowedContentTypes)
                {
                    if(httpContext.Request.ContentType != null && httpContext.Request.ContentType.IndexOf(contentType) > -1)
                    {
                        allowed = true;
                        break;
                    }
                }

                var content = await ReadRequestBody(httpContext);

                if (!allowed && CrossSiteScriptingValidation.IsDangerousString(content, out _))
                {
                    throw new BadRequestException("XSS injection detected from middleware.");
                }
                await _next(httpContext).ConfigureAwait(false);
            }
            finally
            {
                httpContext.Request.Body = originalBody;
            }

            //// leaveOpen: true to leave the stream open after disposing, so it can be read by the model binders
            //using (var streamReader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            //{
            //    var raw = await streamReader.ReadToEndAsync();
            //    var sanitiser = new HtmlSanitizer();
            //    var sanitised = sanitiser.Sanitize(raw);

            //    if (raw != sanitised)
            //    {
            //        throw new BadRequestException("XSS injection detected from middleware.");
            //    }
            //}

            //// rewind the stream for the next middleware
            //httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            //await _next.Invoke(httpContext);
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
}
