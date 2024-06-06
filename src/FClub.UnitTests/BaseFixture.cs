using AutoMapper;
using FClub.Core.Services.Aws;
using FClub.Core.Services.Utils;
using FClub.Core.Services.Jobs;
using FClub.Core.Services.Ffmpeg;
using FClub.Core.Services.FileService;

namespace FClub.UnitTests;

public partial class BaseFixture
{
    protected readonly IClock _clock;
    protected readonly IMapper _mapper;
    protected readonly IFileService _fileService;
    protected readonly IAwsS3Service _awsS3Service;
    protected readonly IFfmpegService _ffmpegService;
    protected readonly IFileDataProvider _fileDataProvider;
    protected readonly IFClubBackgroundJobClient _backgroundJobClient;

    public BaseFixture()
    {
        _fileService = MockFileService(_clock, _mapper, _awsS3Service, _ffmpegService, _fileDataProvider, _backgroundJobClient);
    }

    protected IFileService MockFileService(
        IClock clock,
        IMapper mapper,
        IAwsS3Service awsS3Service,
        IFfmpegService ffmpegService,
        IFileDataProvider fileDataProvider,
        IFClubBackgroundJobClient backgroundJobClient)
    {
        return new FileService(clock, mapper, awsS3Service, ffmpegService, fileDataProvider, backgroundJobClient);
    }
}