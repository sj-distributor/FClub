using AutoMapper;
using FClub.Core.Services.Aws;
using FClub.Core.Services.Ffmpeg;
using FClub.Core.Services.Http.Clients;
using FClub.Core.Services.Jobs;
using FClub.Core.Services.Utils;

namespace FClub.Core.Services.FileService;

public partial class FileService : IFileService
{
    private readonly IClock _clock;
    private readonly IMapper _mapper;
    private readonly IAwsS3Service _awsS3Service;
    private readonly IFfmpegService _ffmpegService;
    private readonly IFileDataProvider _fileDataProvider;
    private readonly ISugarTalkClient _sugarTalkClient;
    private readonly IFClubBackgroundJobClient _backgroundJobClient;
    
    public FileService(
        IClock clock,
        IMapper mapper,
        IAwsS3Service awsS3Service,
        IFfmpegService ffmpegService,
        IFileDataProvider fileDataProvider,
        ISugarTalkClient sugarTalkClient,
        IFClubBackgroundJobClient backgroundJobClient)
    {
        _clock = clock;
        _mapper = mapper;
        _awsS3Service = awsS3Service;
        _ffmpegService = ffmpegService;
        _fileDataProvider = fileDataProvider;
        _sugarTalkClient = sugarTalkClient;
        _backgroundJobClient = backgroundJobClient;
    }
}
