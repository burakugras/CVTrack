using System.Text;
using CVTrack.Persistence.Data;
using CVTrack.Persistence.Repositories;
using CVTrack.Application.Interfaces;
using CVTrack.Application.Users.Services;
using CVTrack.Application.CVs.Services;
using CVTrack.Application.JobApplications.Services;
using CVTrack.Infrastructure.Services;
using CVTrack.Application.CVs.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CVTrack.Api.Swagger;
using Serilog; // FileUploadOperationFilter için


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// 1) DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Repositories & Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<RegisterUserService>();
// builder.Services.AddScoped<LoginUserService>();
builder.Services.AddScoped<ILoginUserService, LoginUserService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder.Services.AddScoped<IAdminUserService, AdminUserService>();

builder.Services.AddScoped<IAdminJobApplicationService, AdminJobApplicationService>();


builder.Services.AddScoped<ICVRepository, CVRepository>();
builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
builder.Services.AddScoped<JobApplicationService>();
builder.Services.AddScoped<CvService>();

builder.Services.AddScoped<IFileService, LocalFileService>();

// 3) JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services
    .AddAuthentication(options =>
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

// 4) FluentValidation (Service registration)
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCvCommandValidator>();

// 5) Controllers
builder.Services.AddControllers();

// 6) Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // File upload için custom operation filter
    c.OperationFilter<FileUploadOperationFilter>();

    // Swagger dokümanı
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CVTrack API",
        Version = "v1",
        Description = "CVTrack İş Başvuru Yönetim Sistemi API’si"
    });

    // JWT Bearer konfigürasyonu
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    c.OperationFilter<CVTrack.Api.Swagger.FileUploadOperationFilter>();
});

var app = builder.Build();



// **EN BAŞTA** exception page
app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

// Swagger UI’ı köke taşı
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CVTrack API V1");
    c.RoutePrefix = string.Empty;
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
