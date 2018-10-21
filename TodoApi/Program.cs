using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Data;

namespace TodoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Build host
            var host = CreateWebHostBuilder(args).Build();

            // Initialize the database
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetService<TodoContext>();
                DbInitializer.Initialize(dbContext);
            }

            // Run host
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
