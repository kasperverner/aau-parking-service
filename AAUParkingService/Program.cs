using AAUParkingService;
using AAUParkingService.Helpers;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting AAU Parking Service");

    var builder = WebApplication.CreateBuilder(args);

    builder.ConfigureLogging();
    builder.ConfigureHangfire();
    
    var app = builder.Build();

    app.UseDashboard();
    app.UseJobScheduling();

    app.Run();
} 
catch (Exception ex)
{
    Log.Fatal(ex, "AAU Parking Service terminated unexpectedly");
} 
finally
{
    Log.Information("Stopping AAU Parking Service");
    Log.CloseAndFlush();
}