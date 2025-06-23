using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotNetCoreBaseAPI.Middleware
{
	public class GlobalExceptionHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

		public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
			_logger = logger;
        }

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch(Exception ex)
			{
				await HandleExceptionsAsync(context, ex);
			}
		}

		private async Task HandleExceptionsAsync(HttpContext context,Exception exception)
		{
			context.Response.ContentType="application/json";
			var response = context.Response;
			var errorResponse = new ErrorResponse
			{
				Success = false,
				ErrorMessage = ""
			};
			switch (exception)
			{
				case ApplicationException ex:
					if(ex.Message.Contains("Invalid Token"))
					{
						response.StatusCode = StatusCodes.Status403Forbidden;
						errorResponse.ErrorMessage = ex.Message;
						break;
					}
					response.StatusCode = StatusCodes.Status400BadRequest;
					errorResponse.ErrorMessage = ex.Message;
					break;

				default:
					response.StatusCode = StatusCodes.Status500InternalServerError;
					errorResponse.ErrorMessage = exception.Message;
					break;
			}
			_logger.LogError(exception.Message);
			var result=JsonSerializer.Serialize(errorResponse);
			await context.Response.WriteAsync(result);
			
		}
	}
}
