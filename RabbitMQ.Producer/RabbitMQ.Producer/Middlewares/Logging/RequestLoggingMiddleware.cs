using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Middlewares.Logging
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger = Log.ForContext<RequestLoggingMiddleware>();

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.Value.Contains("/api/")) { 
                await _next(context);
                return;
            }

            string reqBody = "";
            string resBody = "";
            try
            {
                // Leave the body open so the next middleware can read it.
                using var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

                reqBody = await reader.ReadToEndAsync();

                // Reset the request body stream position so the next middleware can read it
                context.Request.Body.Position = 0;
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"Error While Logging: {ex.Message}"); 
            }

            try
            {
                using var memStream = new MemoryStream();
                Stream originalBody = context.Response.Body;
                context.Response.Body = memStream;
                await _next(context);

                // dummy request
                if (context.Response.StatusCode == 404)
                    return;

                memStream.Position = 0;

                if (context.Response.StatusCode >= 400)
                {
                    // Get response body here after next.Invoke()
                    resBody = new StreamReader(memStream).ReadToEnd();
                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);
                    context.Response.Body = originalBody;
                }
                else
                {
                    await memStream.CopyToAsync(originalBody);
                    context.Response.Body = originalBody;
                }
            }
            finally
            {
                string messageTemplate = "{Method}" +
                    "{StatusCode}" +
                    "{Uri}" +
                    "{ReqHead}" +
                    "{ReqBody}" +
                    "{ResHead}" +
                    "{ResBody}";

                object[] logData =  new object[] {
                    context.Request?.Method,
                    context.Response.StatusCode,
                    context.Request?.Path.Value,
                    JsonConvert.SerializeObject(context.Request?.Headers),
                    reqBody,
                    JsonConvert.SerializeObject(context.Response?.Headers),
                    resBody 
                };

                if (context.Response?.StatusCode >= 200 && context.Response?.StatusCode < 400)
                    _logger.Information(messageTemplate, logData);

                if (context.Response?.StatusCode >= 400 && context.Response?.StatusCode < 500)
                    _logger.Error(messageTemplate, logData);

                if (context.Response?.StatusCode >= 500)
                    _logger.Fatal(messageTemplate, logData);
   
            }
        }
    }
}