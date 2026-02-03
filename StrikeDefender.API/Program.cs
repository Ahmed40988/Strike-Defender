using Serilog;
using StrikeDefender.API;
using StrikeDefender.Application;
using StrikeDefender.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDependencies(builder.Configuration);

builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((Context, configuration) =>
configuration.ReadFrom.Configuration(Context.Configuration)
);
var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Strike Defender v1");
    c.RoutePrefix = "swagger";
});

app.UseSerilogRequestLogging();

app.UseCors();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
        