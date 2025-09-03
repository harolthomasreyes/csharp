using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository.GenericRepository;
using Repository.Interface;
using Services;
using System.Reflection;
using System.Text;
using testing.DbContext;
using testing.Services;
using testing.Services.Interfaces;

// Get version from assembly
var assemblyVersion = Assembly.GetEntryAssembly()?
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
    .InformationalVersion ?? "1.0.0";


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("MyInMemoryDb"));


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = assemblyVersion.ToString() });

    // Add JWT Authentication support to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});


builder.Services.AddAuthorization();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));



// Add JWT Authentication (if not already added)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

        // Enable the "Authorize" button in Swagger UI
        c.OAuthClientId("swagger-ui");
        c.OAuthAppName("Swagger UI");
        c.OAuthUsePkce();
    });
}
app.UseOpenApi();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
