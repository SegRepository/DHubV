using DHubV.Application.Helper;
using DHubV.Core.Extensions;
using DHubV_.Extension;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var jwtOptions = new JWTOptions();
var jwtConfig = new JWTConfig();

builder.Configuration.GetSection("JWTOptions").Bind(jwtOptions);
builder.Configuration.GetSection("JWT").Bind(jwtConfig);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 builder.Services.CoreExtension();
builder.Services.AddDHubVServices(builder.Configuration);
builder.Services.AddServiceDI();

builder
               .Services.AddAuthentication(options =>
               {
                   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               })
               .AddJwtBearer(options =>
               {
                   options.RequireHttpsMetadata = jwtOptions.RequireHttpsMetadata;
                   options.SaveToken = jwtOptions.SaveToken;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = jwtOptions.TokenValidationConfigParameters.ValidateIssuer,
                       ValidateAudience = jwtOptions
                           .TokenValidationConfigParameters
                           .ValidateAudience,
                       ValidateLifetime = jwtOptions
                           .TokenValidationConfigParameters
                           .ValidateLifetime,
                       ValidIssuer = jwtOptions.TokenValidationConfigParameters.ValidIssuer,
                       ValidAudience = jwtOptions.TokenValidationConfigParameters.ValidAudience,
                       IssuerSigningKey = new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(jwtConfig.Key)
                       ),
                       ClockSkew =
                           TimeSpan.Zero // Optional: adjust if needed
                       ,
                   };
                   options.Events = new JwtBearerEvents
                   {
                       OnChallenge = context =>
                       {
                           // Return 401 status code
                           context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                           context.Response.ContentType = "application/json";
                           return context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
                       },
                       /* This for signalR */
                       OnMessageReceived = context =>
                       {
                           var accessToken = context.Request.Query["access_token"];
                           //var path = context.HttpContext.Request.Path;
                           if (!string.IsNullOrEmpty(accessToken))
                           {
                               context.Token = accessToken;
                           }
                           return Task.CompletedTask;
                       },
                   };
               });


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .WithOrigins(
                "http://localhost:4200"

            )
            .AllowAnyHeader()                
            .AllowAnyMethod()               
            .AllowCredentials()
            );           
});
//Configure Serilog
//var logFilePath = Path.Combine(AppContext.BaseDirectory, "Logs", "log-.txt");
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration) // Read configuration from appsettings.json
//    .Enrich.FromLogContext()
//    .CreateLogger();
//builder.Host.UseSerilog();


var app = builder.Build();
app.UseSwagger();
//app.UseSwaggerUI();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DHubV API V1");
   // c.RoutePrefix = ""; // This makes Swagger available at `/`
});
//if (app.Environment.IsDevelopment())
//{

//    app.UseDeveloperExceptionPage();
//}
//else
//{
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();

//}



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseCors("AllowFrontend");
//app.UseStaticFiles(
//    new StaticFileOptions
//    {
//        FileProvider = new PhysicalFileProvider(
//            Path.Combine(Directory.GetCurrentDirectory(), "Logs")
//        ),
//        RequestPath = $"/DHubVLogs_{DateTime.Now.Year}",
//    }
//);
app.MapControllers();

app.Run();
