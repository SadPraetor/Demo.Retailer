using Demo.Retailer.Api.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Retailer.Api.ExceptionHandlers
{
	public class FaultyPaginationQueryExceptionHandler : IExceptionHandler
	{
		public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			if (exception is not FaultyPaginationQueryException)
			{
				return false;
			}

			var problemDetials = new ProblemDetails
			{
				Status = StatusCodes.Status400BadRequest,
				Title = "Faulty pagination query",
				Detail = exception.Message,
			};

			httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
			await httpContext.Response.WriteAsJsonAsync(problemDetials, cancellationToken);

			return true;
		}
	}
}
