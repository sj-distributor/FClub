using Mediator.Net;
using FClub.Core.Data;
using Mediator.Net.Context;
using Mediator.Net.Pipeline;
using Mediator.Net.Contracts;

namespace FClub.Core.Middlewares.UnitOfWork;

public static class UnitOfWorkMiddleware
{
    public static void UseUnitOfWork<TContext>(this IPipeConfigurator<TContext> configurator,
        IUnitOfWork unitOfWork = null) where TContext : IContext<IMessage>
    {
        if (unitOfWork == null && configurator.DependencyScope == null)
        {
            throw new DependencyScopeNotConfiguredException(
                $"{nameof(unitOfWork)} is not provided and IDependencyScope is not configured, Please ensure {nameof(unitOfWork)} is registered properly if you are using IoC container, otherwise please pass {nameof(unitOfWork)} as parameter");
        }

        unitOfWork ??= configurator.DependencyScope.Resolve<IUnitOfWork>();

        configurator.AddPipeSpecification(new UnitOfWorkSpecification<TContext>(unitOfWork));
    }
}