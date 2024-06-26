﻿using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Logging;
using PIMS.allsoft.Controllers;
using PIMS.allsoft.Exceptions;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using KeyNotFoundException = PIMS.allsoft.Exceptions.KeyNotFoundException;
using NotImplementedException = PIMS.allsoft.Exceptions.NotImplementedException;
using UnauthorizedAccessException = PIMS.allsoft.Exceptions.UnauthorizedAccessException;

namespace PIMS.allsoft.Configurations;

public class GlobalExceptionHandelingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandelingMiddleware> _logger;
    public GlobalExceptionHandelingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandelingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            _logger.LogInformation("GET request received", context);
            await _next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error");
            await HandleExceptionAsync(context, e);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception e)
    {
        int statusCode = StatusCodes.Status500InternalServerError;
        HttpStatusCode status;
        var StackTrace = string.Empty;
        string message = e.Message;
        var exceptionType = e.GetType();
        switch (e)
        {
            case NotFoundException _:
                statusCode = StatusCodes.Status404NotFound;
                message = e.Message;
                status = HttpStatusCode.NotFound;
                StackTrace = e.StackTrace;
                break;
            case BadRequestException _:
                statusCode = StatusCodes.Status400BadRequest;
                message = e.Message;
                status = HttpStatusCode.BadRequest;
                StackTrace = e.StackTrace;
                break;
            case NotImplementedException _:
                statusCode = StatusCodes.Status501NotImplemented;
                message = e.Message;
                status = HttpStatusCode.NotImplemented;
                StackTrace = e.StackTrace;
                break;
            case KeyNotFoundException _:
                statusCode = StatusCodes.Status404NotFound;
                message = e.Message;
                status = HttpStatusCode.NotFound;
                StackTrace = e.StackTrace;
                break;
            case UnauthorizedAccessException _:
                statusCode = StatusCodes.Status401Unauthorized;
                message = e.Message;
                status = HttpStatusCode.Unauthorized;
                StackTrace = e.StackTrace;
                break;
        }

        var exceptionResult = JsonSerializer.Serialize(new
        {
            status = statusCode,
            message = message
        });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsync(exceptionResult);
    }
}


