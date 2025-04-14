using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Products.RBA;
using Products.Services;

var builder = WebApplication.CreateBuilder(args);

var certPath = Path.Combine(builder.Environment.ContentRootPath, "Properties", "TLS", "localhost.crt");
var keyPath = Path.Combine(builder.Environment.ContentRootPath, "Properties", "TLS", "localhost.key");

var certificate = X509Certificate2.CreateFromPemFile(certPath, keyPath);

builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<IAuthorizationHandler, ProductOwnershipHandler>();

builder.Services.AddAuthentication(BasicDefaults.AuthenticationScheme)
    .AddBasic<BasicAuthService>(options =>
    {
        options.Realm = "Products API";
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = JwtService.Issuer,
            ValidAudience = JwtService.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtService.SecretKey))
        };
    })
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", options => { });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Oldfags", policy =>
        policy.RequireRole("Software Engineer")
            .RequireClaim("department", "EPD")
            .RequireAssertion(context =>
            {
                var birthDate = context.User.Claims.FirstOrDefault(c => c.Type == "birthDate")?.Value;
                if (birthDate != null)
                {
                    var age = (DateTime.Now - DateTime.Parse(birthDate)).Days / 365;
                    return age > 18;
                }

                return false;
            }));
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(3000); // HTTP (no HTTPS)
    options.ListenAnyIP(3001, listenOptions => listenOptions.UseHttps(certificate)); // HTTPS only
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();