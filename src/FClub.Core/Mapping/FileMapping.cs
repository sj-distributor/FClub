using AutoMapper;
using FClub.Core.Domain.File;
using FClub.Messages.Requests;

namespace FClub.Core.Mapping;

public class FileMapping : Profile
{
    public FileMapping()
    {
        CreateMap<FClubFile, GetCombineMp4VideoTaskDto>();
    }
}