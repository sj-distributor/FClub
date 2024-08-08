using System.Net;
using FluentValidation;
using FClub.Messages.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FClub.Api.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var statusCode = context.Exception switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        context.Result = new OkObjectResult(new FClubResponse()
        {
            Code = statusCode,
            Msg = context.Exception.Message
        });

        context.ExceptionHandled = true;
    }
}