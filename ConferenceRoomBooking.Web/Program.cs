using ConferenceRoomBooking.Dal.SqlRepositories.Migrations;
using ConferenceRoomBooking.Web.Configurations;
using ConferenceRoomBooking.Web.Startup;
using DotNetEnv;

Env.Load(options: LoadOptions.TraversePath().NoClobber());

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);
var app = builder.Build();

app.UseApplicationPipeline();

DatabaseMigrator.Migrate(app.Configuration);
await DataSeeder.SeedAsync(app.Services);

app.Run();
