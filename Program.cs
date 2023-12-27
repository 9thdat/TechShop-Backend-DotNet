using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PhoneShopManagementBackend.Models;
using PhoneShopManagementBackend.Token;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS configuration
var myAllowSpecificOrigins = "AllowAllOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins, builder =>
    {
        builder.WithOrigins("http://localhost:3000", "https://tech-shop-frontend-react.vercel.app")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// SwaggerGen configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhoneShopManagementBackend", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new string[]{}
        }
    });
});

// Configure authorization and authentication
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // If you're not using HTTPS in development
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:SecretKey"])),
            ClockSkew = TimeSpan.Zero
        };
    });

// DbContext configuration
builder.Services.AddDbContext<TechShopContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DevConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DevConnection"))),
    ServiceLifetime.Scoped);

// Load AppSettings from secret.json
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);
var appSettings = appSettingsSection.Get<AppSettings>();

// Register AppSettings as a singleton
builder.Services.AddSingleton(appSettings);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhoneShopManagementBackend v1");
        c.RoutePrefix = string.Empty; // Swagger UI at the root
        c.DisplayRequestDuration();
    });

    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors(myAllowSpecificOrigins); // Make sure this is before other middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
