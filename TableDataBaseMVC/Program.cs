using Microsoft.Extensions.DependencyInjection;
using TableDataBase.Interfaces;
using TableDataBase.Services;
using ElectronNET.API;

namespace TableDataBaseMVC;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddScoped<IDataBaseSchemaServiceFactory, DataBaseSchemaClientServiceFactory>();
        builder.Services.AddScoped<IDataBaseServiceFactory, DataBaseClientServiceFactory>();

        builder.WebHost.UseElectron(args);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=DataBases}/{action=Index}/{id?}");

        if (HybridSupport.IsElectronActive)
        {
            CreateElectronWindow();
        }

        app.Run();

        async void CreateElectronWindow()
        {
            var window = await Electron.WindowManager.CreateWindowAsync();
            window.OnClosed += () => Electron.App.Quit();
        }
    }

}

