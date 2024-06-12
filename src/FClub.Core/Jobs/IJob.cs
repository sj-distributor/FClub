using FClub.Core.Ioc;

namespace FClub.Core.Jobs;

public interface IJob : IScopedDependency
{
    Task Execute();
    
    string JobId { get; }
}