using BirdiTMS.Models.Entities;
using BirdiTMS.Models.ViewModels.FromClient;
using BirdiTMS.Models.ViewModels.FromServer;

namespace BirdiTMS.Services
{
    public interface IBirdiTask
    {
        Task<List<SrBirdiTask>> GetAll(string userId);
        Task<SrBirdiTask> GetTask(int taskId);
        Task<SrBirdiTask> CreateTask(ClBirdiTask clBirdiTask, ApplicationUser user);
        Task<SrBirdiTask> UpdateTask(ClBirdiTask clBirdiTask);
        Task<bool> DeleteTask(int taskId);
    }
}
