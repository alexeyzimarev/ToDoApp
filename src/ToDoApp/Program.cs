using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ToDoApp
{
    static class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}