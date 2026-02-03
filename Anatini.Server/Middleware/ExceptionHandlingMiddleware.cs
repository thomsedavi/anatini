using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var statusCode = ex switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                KeyNotFoundException => HttpStatusCode.NotFound,
                DbUpdateException dbEx when dbEx.InnerException is CosmosException { StatusCode: HttpStatusCode.Conflict } => HttpStatusCode.Conflict,
                _ => HttpStatusCode.InternalServerError
            };
        
            await HandleExceptionAsync(context, statusCode);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode code)
    {
        if (context.Response.HasStarted) return;

        context.Response.ContentType = MediaTypeNames.Application.ProblemJson;
        context.Response.StatusCode = (int)code;

        var problem = new ProblemDetails
        {
            Status = (int)code,
            Title = code.ToString(),
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}
