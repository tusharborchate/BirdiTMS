using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BirdiTMS.Models.Entities;

namespace BirdiTMS.Services
{
    public interface IUser
    {
        Task<JwtSecurityToken> GetJwtSecurityToken(IdentityUser user);
    }
}
