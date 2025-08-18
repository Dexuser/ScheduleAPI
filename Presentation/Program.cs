using System.Text;
using System.Text.Json.Serialization;
using Application.Services;
using Application.Validations;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSqlServer<ScheduleAppContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<UserValidator>();
builder.Services.AddScoped<UserServices>();

builder.Services.AddScoped<IEnabledDateRepository, EnabledDateRepository>();
builder.Services.AddScoped<EnabledDateServices>();
builder.Services.AddScoped<EnabledDateValidator>();

builder.Services.AddScoped<ScheduleServices>();
builder.Services.AddScoped<ScheduleValidator>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();

builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<ShiftServices>();
builder.Services.AddScoped<ShiftValidator>();

builder.Services.AddScoped<IAppointmentsRepository, AppointmentsRepository>();
builder.Services.AddScoped<AppointmentServices>();
builder.Services.AddScoped<AppointmentValidator>();

builder.Services.AddScoped<Application.Services.IEmailSender, EmailSender>();

builder.Logging.AddConsole();
builder.Logging.AddProvider(new SingletonLoggerProvider());


// This Adds the JSON to ENUM value parser.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


// This adds the JSON to DateOnly Parser
builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter()); });


// Those are the Rules that Authenticate a JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.MapInboundClaims = false;
        var config = builder.Configuration;

        var key = Encoding.UTF8.GetBytes(config["jwt:Secret"]!);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = config["jwt:Issuer"],
            ValidAudience = config["jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyMethod();
        policy.AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAnyOrigin");

app.UseHttpsRedirection();

app.UseAuthentication(); // Authen
app.UseAuthorization(); // Authoriz

app.MapControllers();

app.Run();