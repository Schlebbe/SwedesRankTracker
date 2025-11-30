using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using SwedesRankTracker.Models;
using SwedesRankTracker.Services.Startup;
using SwedesRankTracker.Services.Temple;

namespace SwedesRankTracker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
            .AddNegotiate();

            builder.Services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy.
                options.FallbackPolicy = options.DefaultPolicy;
            });
            builder.Services.AddRazorPages();
            builder.Services.AddControllers(); // enable API controllers

            // Register TempleService as a typed HttpClient client, read BaseUrl from configuration
            var templeBase = builder.Configuration.GetValue<string>("TempleApi:BaseUrl");
            if (string.IsNullOrWhiteSpace(templeBase))
                throw new InvalidOperationException("Configuration 'TempleApi:BaseUrl' is missing. Add it to appsettings.json.");

            builder.Services.AddHttpClient<ITempleService, TempleService>(client =>
            {
                client.BaseAddress = new Uri(templeBase);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            builder.Services.AddDbContext<ClanDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IStartupInitializer, StartupInitializer>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // Run one-time startup tasks BEFORE the app starts accepting requests
            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<IStartupInitializer>();
                await initializer.RunAsync();
            }

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();
            app.MapControllers();

            app.Run();
        }
    }
}
