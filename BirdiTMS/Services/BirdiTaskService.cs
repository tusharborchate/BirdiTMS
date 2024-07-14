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

        public async Task<SrBirdiTask> GetTask(int taskId)
        {
            return _mapper.Map<SrBirdiTask>(await _baseRepository.GetByQuery(a => a.Id == taskId).FirstOrDefaultAsync());
        }

        public async Task<List<SrBirdiTask>> GetAll(string userId)
        {
            return  _mapper.Map<List<SrBirdiTask>>(await _baseRepository.GetByQuery(a => a.UserId == userId).ToListAsync());
        }

        public async Task<SrBirdiTask> CreateTask(ClBirdiTask clBirdiTask, ApplicationUser user)
        {
            var entity = _mapper.Map<BirdiTask>(clBirdiTask);
            entity.User = user;
            await _baseRepository.Create(entity);
            return _mapper.Map<SrBirdiTask>(entity);
        }
        public async Task<SrBirdiTask> UpdateTask(ClBirdiTask clBirdiTask)
        {
            var entity = await _baseRepository.GetByQuery(a => a.Id == clBirdiTask.Id).FirstOrDefaultAsync();
            entity.Title = clBirdiTask.Title;
            entity.Description = clBirdiTask.Description;
            entity.Status = clBirdiTask.Status;
            entity.DueDate = clBirdiTask.DueDate;
            await _baseRepository.Update(entity);
            return _mapper.Map<SrBirdiTask>(entity);
        }
        public async Task<bool> DeleteTask(int taskId)
        {
            var task= await _baseRepository.GetByQuery(a => a.Id == taskId).FirstOrDefaultAsync();
            if (task != null)
            {
                await _baseRepository.Delete(task);
                return true;
            }
            return false;
        }

    }
}
