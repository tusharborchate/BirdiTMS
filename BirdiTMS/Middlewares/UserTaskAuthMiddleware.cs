using BirdiTMS.Context;
using BirdiTMS.Extensions;
using BirdiTMS.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BirdiTMS.Middlewares
{
    public class UserTaskAuthMiddleware
    {

        private readonly RequestDelegate _next;

        public UserTaskAuthMiddleware(RequestDelegate next)
        {
            _next = next; 
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext appDbContext,
           IConfiguration configuration,UserManager<ApplicationUser> userManager)
        {               
           string controllerName = context.Request.RouteValues["controller"].ToString();

            if (context.Request.RouteValues.TryGetValue("id", out object taskId) && controllerName == "BirdiTasks")
            {
                var user =await userManager.GetUser(context.User);
                var result = await appDbContext.BirdiTasks.CheckExtension(a => a.Id == Convert.ToInt32(taskId) && a.UserId == user.Id);
                if(result==null)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.Forbidden,
                        Title = "Error occured",
                        Detail = "Forbidden"
                    };
                    //await context.Response
                    //     .WriteAsJsonAsync(problemDetails);
                    throw new Exception(problemDetails.Detail);
                }
            }
            await _next(context);
        }
    }
}
