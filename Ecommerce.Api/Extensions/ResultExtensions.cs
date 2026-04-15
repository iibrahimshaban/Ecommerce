using Ecommerce.Application.Common.Results;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Extensions;

public static class ResultExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {

        var Problem = Results.Problem(statusCode: result.Error.StatusCode);
        var problemDetails = Problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(Problem) as ProblemDetails;

        problemDetails!.Extensions = new Dictionary<string, object?>
            {
                 {
                    "Errors" , new []
                    {
                        result.Error.Code,
                        result.Error.Description
                    }
                 }
            };

        return new ObjectResult(problemDetails);
    }
    public static ObjectResult ToProblem(this ValidationResult result)
    {

        var Problem = Results.Problem(statusCode: StatusCodes.Status400BadRequest,title:"Validation");
        var problemDetails = Problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(Problem) as ProblemDetails;

        problemDetails!.Extensions = new Dictionary<string, object?>
            {
                 {
                    "Errors" , new []
                    {
                        result.Errors.First().ErrorCode,
                        result.Errors.First().ErrorMessage
                    }
                 }
            };

        return new ObjectResult(problemDetails);
    }
}
