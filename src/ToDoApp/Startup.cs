using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ToDoApp.Infrastructure;
using ToDoApp.Modules.ToDos;
using ToDoApp.Modules.ToDos.Persistence;
using ToDoApp.Plumbing.Carter;
using ToDoApp.Plumbing.Persistence;

namespace ToDoApp
{
    public class Startup
    {
        IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCarter();

            services
                .AddSingleton(GetMongo(Configuration["Mongo:ConnectionString"]))
                .AddSingleton<IStore<ToDoDocument>>(ctx => new MongoStore<ToDoDocument>(ctx.GetService<IMongoDatabase>()));

            services.AddSingleton<ToDoCommandService>();

            services.AddSingleton<ExecuteCommand>(
                ctx =>
                    async (cmd, user, ct) => await ctx.GetService<ToDoCommandService>().Handle(cmd, ct)
            );
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(x => x.MapCarter());
        }

        static IMongoDatabase GetMongo(string connectionString)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);

            var client   = new MongoClient(settings);
            var database = MongoUrl.Create(connectionString).DatabaseName;

            return client.GetDatabase(database);
        }
    }
}