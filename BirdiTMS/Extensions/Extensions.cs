using BirdiTMS.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BirdiTMS.Extensions
{
    public static class Extensions
    {

        public static async Task<BirdiTask> CheckExtension(this IEnumerable<BirdiTask> tasks,
                                      Func<BirdiTask, bool> pred)
        {
            var query = tasks.Where(pred).AsQueryable().AsNoTracking();
            return query.FirstOrDefault();
        }
        public static async Task<ApplicationUser>  GetUser(this UserManager<ApplicationUser> userManager,
                                     ClaimsPrincipal claimsPrincipal)
        {
            string userEmail = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userManager.FindByEmailAsync(userEmail);
            return user;
        }
    }
}
