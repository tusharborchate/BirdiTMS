using Microsoft.AspNetCore.Identity;

namespace BirdiTMS.Models.Entities
{
    public class ApplicationUser:IdentityUser<string>
    {
        public virtual ICollection<BirdiTask> BirdiTasks { get; set; }
    }
}
