using Autofac;
using Serilog;
using FClub.Core;
using NSubstitute;
using MySqlConnector;
using Newtonsoft.Json;
using FClub.Core.DbUp;
using FClub.Core.Settings;
using FClub.Core.Services.Jobs;
using FClub.IntegrationTests.Mocks;
using Microsoft.Extensions.Configuration;

namespace FClub.IntegrationTests;

public partial class TestBase
{
    private readonly List<string> _tableRecordsDeletionExcludeList = new()
    {
        "schemaversions"
    };
    
    private void RegisterBaseContainer(ContainerBuilder containerBuilder)
    {
        var logger = Substitute.For<ILogger>();
        
        var configuration = RegisterConfiguration(containerBuilder);
        
        containerBuilder.RegisterModule(
            new FClubModule(logger, configuration, typeof(FClubModule).Assembly, typeof(TestBase).Assembly));
        
        RegisterSugarTalkBackgroundJobClient(containerBuilder);
    }
    
    private IConfiguration RegisterConfiguration(ContainerBuilder containerBuilder)
    {
        var targetJson = $"appsettings{_testTopic}.json";
        File.Copy("appsettings.json", targetJson, true);
        dynamic jsonObj = JsonConvert.DeserializeObject(File.ReadAllText(targetJson));
        jsonObj["ConnectionStrings"]["FClubConnectionString"] =
            jsonObj["ConnectionStrings"]["FClubConnectionString"].ToString()
                .Replace("Database=f_club", $"Database={_databaseName}");
        File.WriteAllText(targetJson, JsonConvert.SerializeObject(jsonObj));
        var configuration = new ConfigurationBuilder().AddJsonFile(targetJson).Build();
        containerBuilder.RegisterInstance(configuration).AsImplementedInterfaces();
        return configuration;
    }
    
    private void RunDbUpIfRequired()
    {
        if (!ShouldRunDbUpDatabases.GetValueOrDefault(_databaseName, true))
            return;

        new DbUpRunner(new FClubConnectionString(CurrentConfiguration).Value).Run();

        ShouldRunDbUpDatabases[_databaseName] = false;
    }
    
    private void ClearDatabaseRecord()
    {
        try
        {
            var connection = new MySqlConnection(new FClubConnectionString(CurrentConfiguration).Value);

            var deleteStatements = new List<string>();

            connection.Open();

            using var reader = new MySqlCommand(
                    $"SELECT table_name FROM INFORMATION_SCHEMA.tables WHERE table_schema = '{_databaseName}';",
                    connection)
                .ExecuteReader();

            deleteStatements.Add($"SET SQL_SAFE_UPDATES = 0");
            while (reader.Read())
            {
                var table = reader.GetString(0);

                if (!_tableRecordsDeletionExcludeList.Contains(table))
                {
                    deleteStatements.Add($"DELETE FROM `{table}`");
                }
            }

            deleteStatements.Add($"SET SQL_SAFE_UPDATES = 1");

            reader.Close();

            var strDeleteStatements = string.Join(";", deleteStatements) + ";";

            new MySqlCommand(strDeleteStatements, connection).ExecuteNonQuery();

            connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cleaning up data, {_testTopic}, {ex}");
        }
    }
    
    private void RegisterSugarTalkBackgroundJobClient(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<MockingBackgroundJobClient>().As<IFClubBackgroundJobClient>().InstancePerLifetimeScope();
    }
    
    public void Dispose()
    {
        ClearDatabaseRecord();
    }

    public async Task InitializeAsync()
    {
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}