using Dotnet_Keycloak_Admin.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth(builder.Configuration);

builder.Services.ConfigureAuthorization();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureRepopsitories();
builder.Services.ConfigureServices();
builder.Services.ConfigureOptions(builder.Configuration);
builder.Services.ConfigureHttpClients();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
