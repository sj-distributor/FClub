using AutoMapper;
using FClub.Core.Services.Aws;
using FClub.Core.Services.Ffmpeg;
using FClub.Core.Services.Jobs;

namespace FClub.Core.Services.FileService;

public partial class FileService : IFileService
{
    private readonly IMapper _mapper;
    private readonly IAwsS3Service _awsS3Service;
    private readonly IFfmpegService _ffmpegService;
    private readonly IFileDataProvider _fileDataProvider;
    private readonly IFClubBackgroundJobClient _backgroundJobClient;
    
    public FileService(
        IMapper mapper,
        IAwsS3Service awsS3Service,
        IFfmpegService ffmpegService,
        IFileDataProvider fileDataProvider,
        IFClubBackgroundJobClient backgroundJobClient)
    {
        _mapper = mapper;
        _awsS3Service = awsS3Service;
        _ffmpegService = ffmpegService;
        _fileDataProvider = fileDataProvider;
        _backgroundJobClient = backgroundJobClient;
    }
}
