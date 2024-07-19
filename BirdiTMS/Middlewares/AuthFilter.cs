using BirdiTMS.Context;
using BirdiTMS.Extensions;
using BirdiTMS.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BirdiTMS.Middlewares
{
    public class AuthFilter : Attribute, IAsyncAuthorizationFilter
    {

        public AuthFilter()
        {
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //if user requesting to update/delete task
            //which not belongs to him then return unauthorized
            var appDbContext = context.HttpContext.RequestServices.GetService<ApplicationDbContext>();

            var userManager = context.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();

            string controllerName = context.RouteData.Values["controller"].ToString();

            if (context.RouteData.Values.TryGetValue("id", out object taskId) && controllerName == "BirdiTasks")
            {
                var user = await userManager.GetUser(context.HttpContext.User);
                var result = await appDbContext.BirdiTasks.CheckExtension(a => a.Id == Convert.ToInt32(taskId) && a.UserId == user.Id);
                if (result == null)
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}
