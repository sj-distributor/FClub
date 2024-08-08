using FClub.Core.Ioc;

namespace FClub.Core.Services.Utils
{
    public interface IClock : IScopedDependency
    {
        DateTimeOffset Now { get; }
    }
}