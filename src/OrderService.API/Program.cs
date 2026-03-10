using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Persistence;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Repositories;
using OrderService.API.Middlewares;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using OrderService.Application.Validators;
using OrderService.Application.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OrderService.Infrastructure.Services;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Hangfire;
// 1. Loglama Ayarý (En baþta kalabilir)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITokenService, TokenService>();

// Serilog'u sisteme tanýt
builder.Host.UseSerilog();

// --- 2. SERVÝS KAYITLARI (builder.Services) ---
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHangfireServer();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Management System API", Version = "v1" });

    // 1. Swagger'da "Authorize" butonu oluþturur
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Giriþ formatý: 'Bearer {token}' \r\n\r\n
                      Örnek: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // 2. Tüm API isteklerine bu güvenlik gereksinimini ekler
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

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<AppDbContext>();


// Veritabaný
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Katmanlar Arasý Baðýmlýlýklar (Dependency Injection)
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();

// AutoMapper ve MediatR
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IOrderRepository).Assembly));

// JWT Ayarlarý (Burasý builder.Build'den ÖNCE olmalý)
var jwtSettings = builder.Configuration.GetSection("JwtSettings"); // JSON'daki isimlendirmeyle ayný olmalý
var secretKey = jwtSettings["Secret"];

if (string.IsNullOrEmpty(secretKey))
{
    throw new Exception("JWT Secret anahtarý appsettings.json içinde bulunamadý!");
}

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();


// Program.cs içerisine ekle
//builder.Services.AddStackExchangeRedisCache(options =>
//{
// Eðer bilgisayarýnda Redis kurulu deðilse þimdilik localhost:6379 kalsýn.
// Hata almamak için kodu "In-Memory" gibi de çalýþtýrabiliriz ama Redis standarttýr.
//options.Configuration = builder.Configuration.GetConnectionString("Redis");
//options.InstanceName = "OrderService_";
//});

builder.Services.AddDistributedMemoryCache();
// --- 3. UYGULAMA ÝNÞASI (BUILD) ---
var app = builder.Build();

app.UseHangfireDashboard(); // Tarayýcýdan iþleri izlemek için: /hangfire
// --- 4. MIDDLEWARE HATTI (app.Use...) ---
// Ýstekler bu sýrayla iþlenir.

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

// Global Hata Yönetimi (Artýk app deðiþkeni var, güvenle kullanabiliriz)
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

// ÖNEMLÝ: Auth sýralamasý
app.UseAuthentication(); // Kimsin?
app.UseAuthorization();  // Yetkin ne?

app.MapControllers();

app.Run();