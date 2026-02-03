
using EFCoreDemoApp.DatabaseInfra;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;

namespace EFCoreDemoApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // Configure Newtonsoft.Json options here as needed
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    // You can add more settings like NullValueHandling, Formatting, etc.
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register the DbContext with SQLite

            //builder.Services.AddDbContext<AppDbContext>(options =>
            //    //options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
            //    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")),
            //    ServiceLifetime.Transient
            //    );

            builder.Services.AddDbContext<AppDbContext>(ServiceLifetime.Transient); // Do not configure optionsBuilder here. Instead, configure it in OnConfiguring of AppDbContext.

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Create Database
            using (var scope = app.Services.CreateAsyncScope())
            {
                /*
                 * To be able to create a DB in Postgres with custom schema, you can't just do
                 *      var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                 *      await dbContext.Database.MigrateAsync();
                 * 
                 * Instead, get the dbContext as below, then create DB using relationalDbCreator.Create();
                 * And finally call MigrateAsync()
                 * 
                 * */


                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();

                //var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
                //var dataSource = new NpgsqlDataSourceBuilder(connectionString)
                //    .EnableDynamicJson()
                //    .Build();
                //const string EFMIGRATIONSHISTORY_TABLE_NAME = "__EFMigrationsHistory";
                //var dbContextOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

                //dbContextOptionsBuilder.UseNpgsql(dataSource,
                //    builder => builder.MigrationsHistoryTable(
                //        EFMIGRATIONSHISTORY_TABLE_NAME,
                //        "efcoredemoapp"));

                //var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
                //var dbContext = new AppDbContext(logger, dbContextOptionsBuilder.Options);

                //var canConnect = false;
                //try
                //{
                //    canConnect = dbContext.Database.CanConnect();
                //}
                //catch
                //{
                //    canConnect = false;
                //}

                ////_logger.LogInformation($"Database already exists: {canConnect}");

                //if (!canConnect)
                //{
                //    try
                //    {
                //        //logger.LogInformation($"Creating database {dbContext.Database.GetDbConnection().Database}");

                //        var relationalDbCreator = dbContext.Database.GetService<IRelationalDatabaseCreator>();
                //        relationalDbCreator.Create();
                //        //logger.LogInformation($"Created database {dbContext.Database.GetDbConnection().Database}");
                //    }
                //    catch (Exception /*ex*/)
                //    {
                //        //logger.LogError($"Failed to create/connect to database {dbContext.Database.GetDbConnection().Database}: {ex}");
                //        throw;
                //    }
                //}

                //await dbContext.Database.MigrateAsync();
            }

            app.MapControllers();

            app.Run();
        }
    }
}
