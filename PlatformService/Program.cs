using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;

var builder = WebApplication.CreateBuilder(args);

// Get environment details
var environment = builder.Environment;

// Add services conditionally based on environment
if (environment.IsProduction())
{
    var connStr = builder.Configuration.GetConnectionString("PlatformConn");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connStr));
    Console.WriteLine("--> Use SQLdb");
}
else
{
    Console.WriteLine("--> Use InMem");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new Microsoft.OpenApi.Models.OpenApiInfo { Title = "PlatformService", Version = "1" }
    );
});

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddScoped<IMessageBusClient, MessageBusClient>();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
}

//app.UseHttpsRedirection();
app.UseRouting();
app.UseHttpsRedirection();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    endpoints.MapGet("/Protos/platform.proto", async context =>
    {
        await context.Response.WriteAsync(File.ReadAllText("Protos/platform.proto"));
    });
});
PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();
