using TableDataBase.Interfaces;
using TableDataBase.Services;
using TableDataBaseServerService.Services;

namespace TableDataBaseServerService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddGrpc();

        var filePath = "TableServer";
        var fileName = "schema.json";
        builder.Services.AddTransient<IDataBaseSchemaService, DataBaseSchemaService>(
            serviceProvider => new DataBaseSchemaService(serviceProvider.GetService<IWebHostEnvironment>().WebRootPath + '/' + filePath, fileName)
            );


        var app = builder.Build();

        app.UseStaticFiles();

        // Configure the HTTP request pipeline.
        app.MapGrpcService<DataBaseApiService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }
}
