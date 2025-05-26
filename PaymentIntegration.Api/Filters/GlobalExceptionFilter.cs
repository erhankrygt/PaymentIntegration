using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PaymentIntegration.Api.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An unhandled exception occurred");

        var response = new
        {
            Error = _env.IsDevelopment() ? context.Exception.Message : "An error occurred while processing your request.",
            StackTrace = _env.IsDevelopment() ? context.Exception.StackTrace : null
        };

        context.Result = new JsonResult(response)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }
} 