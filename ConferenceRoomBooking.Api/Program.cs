using ConferenceRoomBooking.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);
var app = builder.Build();

app.UseApplicationPipeline();

app.Run();