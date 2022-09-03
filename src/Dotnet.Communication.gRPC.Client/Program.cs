using Dotnet.Communication.gRPC.Client.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddOpenTelemetry(builder.Environment.ApplicationName, builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddDocumentation();
builder.Services.AddgRpcClients(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();