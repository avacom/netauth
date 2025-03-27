using System.Reflection;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

var certPath = Path.Combine(builder.Environment.ContentRootPath, "Properties", "TLS", "localhost.crt");
var keyPath = Path.Combine(builder.Environment.ContentRootPath, "Properties", "TLS", "localhost.key");

var certificate = X509Certificate2.CreateFromPemFile(certPath, keyPath);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(3000); // HTTP (no HTTPS)
    options.ListenAnyIP(3001, listenOptions => listenOptions.UseHttps(certificate)); // HTTPS only
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Generate XML file path
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Enforce HTTPS
app.UseAuthorization();

app.MapControllers();

app.Run();