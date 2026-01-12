12 Jan 2026
===========

EF Core Demo App - Lazy Loading Demo
-----------------------------------------

-------------------------
Create New ASP.Net Core 8.0 Web API Project
-------------------------
Key Concepts
	Code First Approach (vs Database First)
		We create model classes by hand
		Migrations are primarily used in the code-first approach, where your C# classes (entities and DbContext) are the source of truth for the database schema.
	Model classes
		Typically POCO
		However, the navigation properties that you want to be lazy-loaded must be marked as virtual. 
	DBContext derived class
		A table in database is represented by the DBSet<> class in DBContext
		So, typically, against each model class that you want to persist in DB, create a DBSet<Book> property in the DBContext

		Override OnConfiguring() and OnModelCreating()
	CLI: 
		dotnet ef migrations add InitialCreate
		dotnet ef database update
		dotnet ef migrations remove
		dotnet ef database update [name] (Revert to a specific migration)
		dotnet ef migrations script (Generate SQL Scripts)
		dotnet ef migrations list

	Migration Files: When you add a migration, EF Core generates a C# file with Up() and Down() methods.
		Up(): Contains the logic to apply the schema changes (e.g., create a table, add a column).
		Down(): Contains the logic to revert those changes, allowing for rollback.

	Model Snapshot: A ModelSnapshot file is created to represent the current state of your entire data model.
		EF Core compares the current model to this snapshot when creating a new migration to determine what has changed.

	__EFMigrationsHistory Table: EF Core automatically creates a history table in your database 
		to track which migrations have already been applied, ensuring the correct sequence of updates. 

	Package Manager:
		dotnet add package Microsoft.EntityFrameworkCore
		dotnet tool install Microsoft.EntityFrameworkCore.Tools or the dotnet ef global tool (for CLI)
		dotnet add package Devart.Data.PostgreSql.EFCore (If Postgres)

		dotnet add package Microsoft.EntityFrameworkCore.Sqlite (If SQLite)
		dotnet add package Microsoft.EntityFrameworkCore.Proxies
		dotnet add package Microsoft.EntityFrameworkCore.Design

	To apply migrations programmatically, call context.Database.MigrateAsync(). 
		public static async Task Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			using (var scope = host.Services.CreateScope())
			{
				var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				await db.Database.MigrateAsync();
			}

			host.Run();
		}
		
		Note that MigrateAsync() builds on top of the IMigrator service, which can be used for more advanced scenarios. Use myDbContext.GetInfrastructure().GetService<IMigrator>() to access it.

	Using a Separate Migrations Project
		You may want to store your migrations in a different project than the one containing your DbContext. 
		You can also use this strategy to maintain multiple sets of migrations, for example, one for development and another for release-to-release upgrades

			services.AddDbContext<ApplicationDbContext>(
				options =>
					options.UseSqlServer(
						Configuration.GetConnectionString("DefaultConnection"),
						x => x.MigrationsAssembly("WebApplication1.Migrations")));

			<ItemGroup>
			  <ProjectReference Include="..\WebApplication1.Migrations\WebApplication1.Migrations.csproj" />
			</ItemGroup>

			If this causes a circular dependency, you can update the base output path of the migrations project instead:

			<PropertyGroup>
			  <BaseOutputPath>..\WebApplication1\bin\</BaseOutputPath>
			</PropertyGroup>

		CLI:
			dotnet ef migrations add NewMigration --project WebApplication1.Migrations
-------------------------
Add Book, Author classes, DbContext, SQLite
-------------------------
-------------------------
-------------------------
-------------------------
-------------------------
-------------------------
-------------------------
-------------------------
-------------------------
-------------------------
-------------------------
-------------------------

