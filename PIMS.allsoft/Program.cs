
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PIMS.allsoft.Configurations;
using PIMS.allsoft.Context;
using PIMS.allsoft.Interfaces;
using PIMS.allsoft.Services;
using Serilog;
using Serilog.Exceptions;
using System.Reflection;
using System.Text;
using System.Text.Json;

try
{
    var configuration = new ConfigurationBuilder()
                      .AddJsonFile("appsettings.json")
                      .Build();
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.WithExceptionDetails()
       .CreateLogger();

    Log.Logger.Information("Logging is working fine");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();
    builder.Services.AddDbContext<PIMSContext>
        (options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
    builder.Services.AddTransient<IAuthService, AuthService>();
    builder.Services.AddTransient<ICategoryService, CategoryService>();
    builder.Services.AddTransient<IProductService, ProductService>();
    builder.Services.AddTransient<IInventoryService, InventoryService>();
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
    builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                // Skip the default logic.
                context.HandleResponse();

                var result = JsonSerializer.Serialize(new { status = 401, message = "Unauthorized access." });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 401;
                return context.Response.WriteAsync(result);
            },
            OnForbidden = context =>
            {
                var result = JsonSerializer.Serialize(new { status = 403, message = "Forbidden. You do not have permission to access this resource." });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 403;
                return context.Response.WriteAsync(result);
            }
        };
    });
    builder.Services.AddAuthorization(options =>
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder(
            JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(opt => // Specify the MIME types that the API can consume
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), xmlFile);
        opt.IncludeXmlComments(xmlPath);

        opt.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "MyAPI",
            Description= "Product Inventory Management System using .NET Core for the backend, SQL Server for persistent storage, and expose functionality through a secure, versioned RESTful Web API. This system will not only manage product and inventory records but also include user authentication, role-based access control, and logging !",
            Version = "v1"
        });
        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });

        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
        });
    });
    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            // new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("x-version")//,
                                                   // new MediaTypeApiVersionReader("ver")
            );
    });
    // Configure API Versioning Explorer
    builder.Services.AddVersionedApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
    builder.Services.AddMemoryCache();
    // builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(240); });
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger();
        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
        app.UseSwaggerUI(c => // UseSwaggerUI Protected by if (env.IsDevelopment())
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); //This is default selected
                                                                        //c.SwaggerEndpoint("/swagger/v1.1/swagger.json", "V1.1"); //for swagger versioning
                                                                        //c.SwaggerEndpoint("/swagger/v1.2/swagger.json", "V1.2");
                                                                        //c.SupportedSubmitMethods(); // by using this  remove "try it now option"
                                                                        // c.RoutePrefix = string.Empty; // Set Swagger UI at the root   ex:- Access Swagger UI at: https://localhost:7123/
            c.RoutePrefix = "swagger"; // Serve Swagger UI at /swagger ex:- Access Swagger UI at: https://localhost:7123/swagger
        });
    }
    app.AddGlobalErrorHandeler();
    //app.UseSession();
    app.UseHttpsRedirection();
    app.UseAuthentication();

    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}