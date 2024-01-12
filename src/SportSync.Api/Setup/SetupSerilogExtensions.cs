using System.Data;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace sport_sync.Setup;

public static class SetupSerilogExtensions
{
    public static IHostBuilder SetupSerilog(this IHostBuilder hostBuilder, string dbConnectionString)
    {
        hostBuilder.UseSerilog((context, lc) =>
        {
            //var telemetryClient = (TelemetryClient)serviceProvider.GetService(typeof(TelemetryClient));
            lc
                .ReadFrom.Configuration(context.Configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                //.WriteTo.ApplicationInsights(telemetryClient, TelemetryConverter.Traces)
                .WriteTo.MSSqlServer(dbConnectionString,
                    new MSSqlServerSinkOptions
                    {
                        TableName = "Log",
                        AutoCreateSqlTable = true
                    },
                    restrictedToMinimumLevel: LogEventLevel.Warning,
                    columnOptions: new ColumnOptions
                    {
                        AdditionalColumns = new List<SqlColumn>
                        {
                            new ("SourceContext", SqlDbType.VarChar)
                        }
                    })
                .Enrich.FromLogContext();
        });

        return hostBuilder;
    }
}