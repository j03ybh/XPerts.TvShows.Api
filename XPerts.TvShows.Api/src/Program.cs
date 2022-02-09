using Microsoft.OpenApi.Models;
using System.Reflection;
using XPerts.TvShows.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Host.ConfigureServices((hostContext, services) =>
{
    services
        .RegisterCoreServices()
        .RegisterOptions(hostContext);
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "TvShow API",
        Description = "A Web API for managing the most recent, and, the most awesome tv shows in the universe",
        TermsOfService = new Uri("https://we-rock-your-world.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Brought to you by Joey B.",
            Url = new Uri("https://linkedin.com/j03y")
        },
        License = new OpenApiLicense
        {
            Name = "Built with TechMinimalists© software",
            Url = new Uri("https://tech-minimalists.com/license")
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.ResolveConflictingActions(x => x.First());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DisplayRequestDuration();
    });
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();