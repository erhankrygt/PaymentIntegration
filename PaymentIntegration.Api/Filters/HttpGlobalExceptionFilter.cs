using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PaymentIntegration.Api.Filters;

public class HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
    : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
            context.Exception.Message);

        var json = new JsonErrorResponse
        {
            Messages = new[] { "An error occurred. Try it again." }
        };

        if (env.IsDevelopment())
        {
            json.DeveloperMessage = context.Exception;
        }

        // Result asigned to a result object but in destiny the response is empty. This is a known bug of .net core 1.1
        // It will be fixed in .net core 1.1.2. See https://github.com/aspnet/Mvc/issues/5594 for more information
        context.Result = new InternalServerErrorObjectResult(json);
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.ExceptionHandled = true;
    }

    private class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object error) : base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

    private class JsonErrorResponse
    {
        public string[] Messages { get; set; }
        public object DeveloperMessage { get; set; }
    }
}
