using BirdiTMS.Context;
using BirdiTMS.Middlewares;
using BirdiTMS.Models.Entities;
using BirdiTMS.Repository;
using BirdiTMS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Configuration;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Configure NLog
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
});

builder.Services.AddTransient<ClaimsPrincipal>(
    s =>
    {
        var claims = s.GetService<IHttpContextAccessor>()?.HttpContext?.User ?? new ClaimsPrincipal();
        return claims;
    });
builder.Services.AddHttpContextAccessor();

// Add NLog as the logger provider
builder.Services.AddSingleton<ILoggerProvider, NLogLoggerProvider>();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var securityScheme = new OpenApiSecurityScheme()
{
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JSON Web Token based security",
};

var securityReq = new OpenApiSecurityRequirement()
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
};

var contact = new OpenApiContact()
{
    Name = "Mohamad Lawand",
    Email = "hello@mohamadlawand.com",
    Url = new Uri("http://www.mohamadlawand.com")
};

var license = new OpenApiLicense()
{
    Name = "Free License",
    Url = new Uri("http://www.mohamadlawand.com")
};

var info = new OpenApiInfo()
{
    Version = "v1",
    Title = "Minimal API - JWT Authentication with Swagger demo",
    Description = "Implementing JWT Authentication in Minimal API",
    TermsOfService = new Uri("http://www.example.com"),
    Contact = contact,
    License = license
};
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", info);
    o.AddSecurityDefinition("Bearer", securityScheme);
    o.AddSecurityRequirement(securityReq);
});
//use ApplicationDbContext and get sql connection string 

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BirdiTMSCon"));
    options.EnableSensitiveDataLogging();
});
builder.Services.AddAuthorization();
//add identity to project
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//register dependencies
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

builder.Services.AddScoped<IBirdiTask, BirdiTaskService>();



//jwt validation
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddResponseCompression();
builder.Services.AddHsts(opt =>
{
    opt.MaxAge = new TimeSpan(2592000);
    opt.Preload = true;
});
LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nLog.config"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler(o => { });

using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    await dbContext.Database.MigrateAsync();
}
app.UseHsts();
app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseStaticFiles();
app.UseCors();

app.UseAuthorization();
app.MapControllers();

app.Run();
