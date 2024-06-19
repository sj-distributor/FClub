using AutoMapper;
using FClub.Core.Domain.Account;
using FClub.Messages.Dto.Users;

namespace FClub.Core.Mapping
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<UserAccount, UserAccountDto>().ReverseMap();
        }
    }
}