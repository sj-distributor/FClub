using Xunit;
using Mediator.Net;
using FClub.Messages.Commands;
using FClub.IntegrationTests.TestBaseClasses;

namespace FClub.IntegrationTests.Services;

public class FileServiceFixture : FileFixtureBase
{
    [Fact]
    public async Task ShouldCombineAsync()
    {
        await Run<IMediator>(async mediator =>
        {
            var command = new CombineMp4VideosCommand
            {
                FilePath = "FClub/2dfab0fe-1b1f-4c90-9903-59b97cde2f4e.mp4",
               
            };
        });
    }
}