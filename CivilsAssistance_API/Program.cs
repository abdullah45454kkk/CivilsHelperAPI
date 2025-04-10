using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Models.Mapper;
using Microsoft.Extensions.DependencyInjection;
using DataAccess.Repository.IRepository;
using DataAccess.Repository;
using Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DataAccess.EmailService;
using DataAccess.EmailServices.IEmailService;
using CivilsAssistance_API.Middlewares;
using DataAccess.PaymentService.IPaymentService;
using DataAccess.PaymentService;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});
Console.WriteLine($"Connection String: {builder.Configuration.GetConnectionString("ConString")}");

builder.Services.AddDbContext<CivilsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConString")));

//var smtpPassword = builder.Configuration["Email:Smtp:Password"] ?? Environment.GetEnvironmentVariable("SMTP_PASSWORD_ENV");

builder.Services.AddIdentity<LocalUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = true; // Require email confirmation
})
.AddEntityFrameworkStores<CivilsDbContext>()
.AddDefaultTokenProviders(); builder.Services.AddAutoMapper(typeof(MapperConfig));

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPaymentServices, PaymentService>();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});





var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "your-secure-key-here-32-chars-min");
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddHostedService<SeedDataHostedService>();



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Add this to show detailed error pages
    app.UseSwagger();
    app.UseSwaggerUI();
}

else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\": \"An unexpected error occurred. Please try again later.\"}");
        });
    });
}



app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();
app.Run();