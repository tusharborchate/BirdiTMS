using BirdiTMS.Models.Entities;
using BirdiTMS.Models.ViewModels.FromClient;

namespace BirdiTMS.Services
{
    public interface IBirdiTask
    {
        Task<List<BirdiTask>> GetAll(string userId);
        Task<BirdiTask> GetTask(int taskId, string userId);
        Task<BirdiTask> CreateTask(ClBirdiTask clBirdiTask, ApplicationUser user);
        Task<BirdiTask> UpdateTask(ClBirdiTask clBirdiTask, ApplicationUser user);
        Task<bool> DeleteTask(int taskId, string userId);
    }
}
