using FClub.Core.Ioc;

namespace FClub.Core.Services.Infrastructure
{
    public interface IClock : IScopedDependency
    {
        DateTimeOffset Now { get; }
        
        DateTime DateTimeNow { get; }
    }
}