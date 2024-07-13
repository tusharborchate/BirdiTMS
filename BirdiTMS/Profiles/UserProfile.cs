using AutoMapper;
using BirdiTMS.Models.Entities;
using BirdiTMS.Models.ViewModels.FromServer;
using BirdiTMS.Services;

namespace BirdiTMS.Profiles
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, SrUserViewModel>();
        }
    }
}
