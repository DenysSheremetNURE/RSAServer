using RSAServer.Options;
using RSAServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<UsersOptions>(builder.Configuration.GetSection(UsersOptions.SectionName));
builder.Services.AddSingleton<IRsaKeyProvider, RsaKeyProvider>();
builder.Services.AddScoped<ILoginService, LoginService>();

var app = builder.Build();

// Instant generation of RSA keys on startup to avoid delays on first request
app.Services.GetRequiredService<IRsaKeyProvider>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();