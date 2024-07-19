using BirdiTMS.Models.Entities;
using BirdiTMS.Models.ViewModels.FromServer;
using AutoMapper;
using BirdiTMS.Models.ViewModels.FromClient;

namespace BirdiTMS.Profiles
{
    public class BirdiTaskProfile:Profile
    {
        public BirdiTaskProfile() 
        {
            CreateMap<BirdiTask, SrBirdiTask>();

            CreateMap<ClBirdiTask, BirdiTask>();
        }
    }
}
