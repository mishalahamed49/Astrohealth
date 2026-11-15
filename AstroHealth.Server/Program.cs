using AstroHealth.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddScoped<ThresholdEvaluator>();
builder.Services.AddScoped<DemoDataSeeder>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    if (builder.Configuration.GetValue<bool>("SeedData:Enabled"))
    {
        using var scope = app.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<DemoDataSeeder>().SeedAsync();
    }
}

app.UseHttpsRedirection();
app.UseCors("BlazorClient");
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
