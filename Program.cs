using ABCRetail_Cloud_.Services;
using Azure.Storage.Files.Shares;
using Microsoft.Extensions.Configuration;
using ABCRetail_Cloud_.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ABCRetail_Cloud_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ------------------------------
            // 1. Azure Storage Configuration
            // ------------------------------
            var azureStorageConnectionString = builder.Configuration["AzureStorage:ConnectionString"];

            if (string.IsNullOrEmpty(azureStorageConnectionString))
            {
                throw new InvalidOperationException("Azure Storage connection string is not configured. Check Azure App Service Application Settings: AzureStorage__ConnectionString");
            }

            // ------------------------------
            // 2. Identity / EF Core Configuration
            // ------------------------------
            var identityConnectionString = builder.Configuration.GetConnectionString("ABCRetail_Cloud_ContextConnection")
                ?? throw new InvalidOperationException("Connection string 'ABCRetail_Cloud_ContextConnection' not found. Check Azure App Service Application Settings: ConnectionStrings__ABCRetail_Cloud_ContextConnection");

            builder.Services.AddDbContext<ABCRetail_Cloud_Context>(options =>
                options.UseSqlServer(identityConnectionString));

            builder.Services.AddDefaultIdentity<ABCRetail_Cloud_User>(options =>
                    options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ABCRetail_Cloud_Context>();

            // ------------------------------
            // 3. Register Application Services
            // ------------------------------

            // Blob Service
            builder.Services.AddScoped(sp => new BlobService(azureStorageConnectionString));

            // Table Storage Service
            builder.Services.AddSingleton(sp => new TableStorageService(azureStorageConnectionString));

            // Queue Service
            builder.Services.AddSingleton(sp => new QueueService(azureStorageConnectionString, "order"));

            // File Share Client
            builder.Services.AddSingleton(sp => new ShareServiceClient(azureStorageConnectionString));

            // File Share Service
            builder.Services.AddSingleton<FileShareService>(sp =>
            {
                var shareClient = sp.GetRequiredService<ShareServiceClient>();
                return new FileShareService(shareClient, "product");
            });

            // Cart Service
            builder.Services.AddScoped<CartService>();

            // ------------------------------
            // 4. Add Controllers / Views
            // ------------------------------
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages(); // Identity UI

            // ------------------------------
            // 5. Build and Configure App
            // ------------------------------
            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();  // Required for Identity
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages(); // Identity UI pages (Login/Register)

            app.Run();
        }
    }
}

