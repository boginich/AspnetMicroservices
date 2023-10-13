using Catalog.API.Data;
using Catalog.API.Data.Interfaces;
using Catalog.API.Repositories;
using Catalog.API.Repositories.Interfaces;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddHealthChecks().AddMongoDb(
    builder.Configuration["DatabaseSettings:ConnectionString"] ?? string.Empty, "MongoDb Health", HealthStatus.Degraded);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Catalog.API", Version = "v1"}); });


var app = builder.Build(); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));
}


app.UseRouting();
 
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();                
//    endpoints.MapHealthChecks("/hc", new HealthCheckOptions
//    {
//        Predicate = _ => true,
//        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//    });
//});


app.Run();