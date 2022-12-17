using ClinicService.Data;
using ClinicService.Services;
using ClinicService.Services.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps("C:/developmentcert.pfx", "1234");
    });
});

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
    logging.RequestHeaders.Add("Authorization");
    logging.RequestHeaders.Add("X-Real-IP");
    logging.RequestHeaders.Add("X-Forwarded-For");
});


builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();

}).UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = true });


builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IConsultationRepository, ConsultationRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

builder.Services.AddSingleton<IAuthenticateService, AuthenticateService>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme =
    JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme =
    JwtBearerDefaults.AuthenticationScheme;
})
           .AddJwtBearer(x =>
           {
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new
               TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthenticateService.SecretKey)),
                   ValidateIssuer = false, // true
                   ValidateAudience = false, // true
                   ValidIssuer = builder.Configuration["Settings:Security:Issuer"],
                   ValidAudience = builder.Configuration["Settings:Security:Audience"],
                   ClockSkew = TimeSpan.Zero
               };
           });

builder.Services.AddGrpc();

builder.Services.AddDbContext<ClinicServiceDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["Settings:DatabaseOptions:ConnectionString"]);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "������ �������",
        Version = "v1",
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseWhen( 
               ctx => ctx.Request.ContentType != "application/grpc",
               builder =>
               {
                   builder.UseHttpLogging();
               }
           );
app.UseAuthorization();

app.MapControllers();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<ClinicClientService>();
    endpoints.MapGrpcService<AuthService>();
});

app.Run();
