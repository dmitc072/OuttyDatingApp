using Microsoft.EntityFrameworkCore;
using Outty.Api.Data;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddAzureKeyVault(
    new Uri("https://outty-kv.vault.azure.net/"),
    new DefaultAzureCredential());

builder.Services.AddOpenApi();

builder.Services.AddDbContext<OuttyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OuttyDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("/states", async (OuttyDbContext db) =>
{
    var states = await db.States
        .OrderBy(s => s.Name)
        .Select(s => new { s.Abbreviation, s.Name })
        .ToListAsync();

    return states;
})
.WithName("GetStates");

app.Run();


