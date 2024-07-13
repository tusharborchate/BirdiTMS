using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BirdiTMS.Middlewares
{
    public class GlobalExceptionHandler(IHostEnvironment env) : IExceptionHandler
    {

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var details = exception.Message; 
          if(!env.IsDevelopment())
            {
                details = "We are trying to solve it";
            }
            var problemDetails = new ProblemDetails
            {
                Status = httpContext.Response.StatusCode,
                Title = "Error occured",
                Detail =  details
            };

            await httpContext.Response
           .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
