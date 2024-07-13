using AutoMapper;
using BirdiTMS.Models.Entities;
using BirdiTMS.Models.ViewModels.FromClient;
using BirdiTMS.Models.ViewModels.FromServer;
using BirdiTMS.Repository;
using Microsoft.EntityFrameworkCore;

namespace BirdiTMS.Services
{
    public class BirdiTaskService : IBirdiTask
    {
        private readonly IBaseRepository<BirdiTask> _baseRepository;
        private readonly IMapper _mapper;

        public BirdiTaskService(IBaseRepository<BirdiTask> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<BirdiTask> GetTask(int taskId, string userId)
        {
            return await _baseRepository.GetByQuery(a => a.Id == taskId && a.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<List<BirdiTask>> GetAll(string userId)
        {
            return await _baseRepository.GetByQuery(a => a.UserId == userId).ToListAsync();
        }

        public async Task<BirdiTask> CreateTask(ClBirdiTask clBirdiTask, ApplicationUser user)
        {
            var entity = _mapper.Map<BirdiTask>(clBirdiTask);
            entity.User = user;
            await _baseRepository.Create(entity);
            return entity;

        }
        public async Task<BirdiTask> UpdateTask(ClBirdiTask clBirdiTask, ApplicationUser user)
        {
            var entity = _mapper.Map<BirdiTask>(clBirdiTask);
            entity.User = user;
            entity.Title = clBirdiTask.Title;
            entity.Description = clBirdiTask.Description;
            entity.Status = clBirdiTask.Status;
            entity.DueDate = clBirdiTask.DueDate;
            await _baseRepository.Update(entity);
            return entity;
        }
        public async Task<bool> DeleteTask(int taskId, string userId)
        {
            var task= await _baseRepository.GetByQuery(a => a.Id == taskId && a.UserId == userId).FirstOrDefaultAsync();
            if (task != null)
            {
                await _baseRepository.Delete(task);
                return true;
            }
            return false;
        }

    }
}
