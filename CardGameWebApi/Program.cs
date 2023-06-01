using CardGameWebApi.BAL.Implementations;
using CardGameWebApi.BAL.Interfaces;
using CardGameWebApi.BAL;
using CardGameWebApi.DAL;
using CardGameWebApi.PL.Controllers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddTransient<IUserRep, EfUser>();
builder.Services.AddTransient<IGameSessionsRep, EfGameSession>();
builder.Services.AddTransient<ILobbyRep, EfLobby>();
builder.Services.AddScoped<DataManager>();

var connectionString = builder.Configuration.GetConnectionString("MyAspNetConnection");
builder.Services.AddDbContext<CardGameDbContext>(options =>
{
	options.UseSqlServer(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapControllers();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
	endpoints.MapHub<WebSocketController>("/ws");
});

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<DataManager>();

	if (context.Lobbies.GetAllLobbies().FirstOrDefault() != null)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine("Lobby detected");
		Console.ResetColor();
	}
}

app.Run();
