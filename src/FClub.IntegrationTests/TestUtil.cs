using Autofac;

namespace FClub.IntegrationTests;

public class TestUtil : TestUtilBase
{
    protected TestUtil(ILifetimeScope scope)
    {
        SetupScope(scope);
    }
}