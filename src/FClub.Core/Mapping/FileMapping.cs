using AutoMapper;
using FClub.Core.Domain.File;
using FClub.Messages.Commands;

namespace FClub.Core.Mapping;

public class FileMapping : Profile
{
    public FileMapping()
    {
        CreateMap<CombineMp4VideosTaskCommand, FClubFile>();
    }
}