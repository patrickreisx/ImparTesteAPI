using System.Text.Json.Serialization;
using Impar.Infra.Context;
using ImparTesteAPI.Services;
using ImparTesteAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.WriteTo.File(new CompactJsonFormatter(), "./logs/log-.txt", rollingInterval: RollingInterval.Day)
	.CreateLogger();
builder.Host.UseSerilog();

const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: myAllowSpecificOrigins,
		policy => { policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod().Build(); });
});

builder.Services.AddDbContext<EntityContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!));

builder.Services.AddControllers().AddJsonOptions(x =>
{
	x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICarService, CarService>();

var app = builder.Build();

app.UseCors(myAllowSpecificOrigins);

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
