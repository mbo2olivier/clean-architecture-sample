using Cnss.Affiliation.Infrastructure;
using Cnss.Cotisation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString("Database")
    ?? "Host=localhost;Port=5432;Database=cnss;Username=cnss;Password=cnss";

builder.Services.AddAffiliationInfrastructureLayer(connectionString);
builder.Services.AddCotisationInfrastructureLayer(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
