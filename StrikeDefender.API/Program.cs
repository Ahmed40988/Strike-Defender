using Serilog;
using StrikeDefender.API;
using StrikeDefender.Application;
using StrikeDefender.Infrastructure;
using StrikeDefender.Infrastructure.Common.Persistence.Data;
using StrikeDefender.Infrastructure.Common.Persistence.Seeding;

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

//Database seeding for plans, this is required for the application to work properly,
//it will only seed if there are no plans in the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<StrikeDefenderDbContext>();

    // seed plans
    await PlanSeeder.SeedAsync(db);

    // seed roles ??
    var roleSeeder = services.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedAsync();
}
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
        