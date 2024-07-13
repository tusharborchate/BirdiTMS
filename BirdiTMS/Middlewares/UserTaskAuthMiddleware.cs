using BirdiTMS.Context;
using BirdiTMS.Controllers;
using BirdiTMS.Extensions;
using BirdiTMS.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;

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
                var userId = userManager.GetUserId(context.User);
                var result = await appDbContext.BirdiTasks.CheckExtension(a => a.Id == Convert.ToInt32(taskId) && a.UserId == userId);
                if(result==null)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.Forbidden,
                        Title = "Error occured",
                        Detail = "Forbidden"
                    };
                    await context.Response
                         .WriteAsJsonAsync(problemDetails);
                    throw new Exception();
                }
            }

               
            await _next(context);
        }
    }
}
